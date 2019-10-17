using System;
using System.IO;
using System.Reflection;
using domi1819.UpCore.Utilities;

namespace domi1819.UpClient
{
    internal static class Program
    {
        private static UpClient instance;

        [STAThread]
        internal static void Main(string[] cmdArgs)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "libs", new AssemblyName(args.Name).Name + ".dll");

                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            Run(cmdArgs);
        }

        private static void Run(string[] cmdArgs)
        {
            CrashHandler.Run(() => CheckSingleInstance(cmdArgs));
        }

        private static void CheckSingleInstance(string[] cmdArgs)
        {
            using (SingleInstance singleInst = new SingleInstance(new Guid("{3ca99fee-c832-4156-a721-959df4ffa704}")))
            {
                if (singleInst.IsFirstInstance)
                {
                    singleInst.ArgumentsReceived += (sender, e) =>
                    {
                        instance.ProcessLiveArgs(e.Args, true);
                    };
                    singleInst.ListenForArgumentsFromSuccessiveInstances();

                    // ReSharper disable once PossibleNullReferenceException
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", ""));
                    
                    instance = new UpClient();
                    instance.LaunchApplication(cmdArgs);
                }
                else
                {
                    singleInst.PassArgumentsToFirstInstance(cmdArgs);
                }
            }
        }
    }
}
