using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Domi.UpCore.Utilities
{
    public class SingleInstance : IDisposable
    {
        private Mutex mutex;
        private readonly bool ownsMutex;
        private readonly Guid identifier;
        
        /// <summary>
        /// Indicates whether this is the first instance of this application.
        /// </summary>
        public bool IsFirstInstance => this.ownsMutex;
        
        /// <summary>
        /// Event raised when arguments are received from successive instances.
        /// </summary>
        public event EventHandler<ArgumentsReceivedEventArgs> ArgumentsReceived;

        /// <summary>
        /// Enforces single instance for an application.
        /// </summary>
        /// <param name="identifier">An identifier unique to this application.</param>
        public SingleInstance(Guid identifier)
        {
            this.identifier = identifier;
            this.mutex = new Mutex(true, identifier.ToString(), out this.ownsMutex);
        }

        /// <summary>
        /// Passes the given arguments to the first running instance of the application.
        /// </summary>
        /// <param name="arguments">The arguments to pass.</param>
        /// <returns>Return true if the operation succeded, false otherwise.</returns>
        public bool PassArgumentsToFirstInstance(string[] arguments)
        {
            if (this.IsFirstInstance)
            {
                throw new InvalidOperationException("This is the first instance.");
            }

            try
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(this.identifier.ToString()))
                {
                    using (StreamWriter writer = new StreamWriter(client))
                    {
                        client.Connect(200);

                        foreach (string argument in arguments)
                        {
                            writer.WriteLine(argument);
                        }
                    }
                }

                return true;
            }
            catch (TimeoutException)
            {
                //Couldn't connect to server
            }
            catch (IOException)
            {
                //Pipe was broken
            }

            return false;
        }

        /// <summary>
        /// Listens for arguments being passed from successive instances of the application.
        /// </summary>
        public void ListenForArgumentsFromSuccessiveInstances()
        {
            if (!this.IsFirstInstance)
            {
                throw new InvalidOperationException("This is not the first instance.");
            }

            ThreadPool.QueueUserWorkItem(this.ListenForArguments);
        }

        /// <summary>
        /// Listens for arguments on a named pipe.
        /// </summary>
        /// <param name="state">State object required by WaitCallback delegate.</param>
        private void ListenForArguments(object state)
        {
            try
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream(this.identifier.ToString()))
                {
                    using (StreamReader reader = new StreamReader(server))
                    {
                        server.WaitForConnection();

                        List<string> arguments = new List<string>();

                        while (server.IsConnected)
                        {
                            arguments.Add(reader.ReadLine());
                        }

                        arguments.RemoveAt(arguments.Count - 1);

                        ThreadPool.QueueUserWorkItem(this.CallOnArgumentsReceived, arguments.ToArray());
                    }
                }
            }
            catch (IOException)
            {
                //Pipe was broken
            }
            finally
            {
                ThreadPool.QueueUserWorkItem(this.ListenForArguments);
            }
        }

        /// <summary>
        /// Calls the OnArgumentsReceived method casting the state Object to String[].
        /// </summary>
        /// <param name="state">The arguments to pass.</param>
        private void CallOnArgumentsReceived(object state)
        {
            this.ArgumentsReceived?.Invoke(this, new ArgumentsReceivedEventArgs { Args = (string[])state });
        }
        
        public void Dispose()
        {
            if (this.ownsMutex)
            {
                this.mutex.ReleaseMutex();
            }

            this.mutex = null;
        }
    }

    public class ArgumentsReceivedEventArgs : EventArgs
    {
        public string[] Args { get; set; }
    }
}
