using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using domi1819.UpCore.Config;
using domi1819.UpCore.Network;
using domi1819.UpCore.Utilities;

namespace domi1819.UpCliClient
{
    internal class UpCliClient
    {
        internal void Configure()
        {
            Config settings = Config.Load();

            Console.WriteLine("Server    " + settings.ServerAddress + (settings.ServerPort == Constants.DefaultPort ? string.Empty : ":" + settings.ServerPort));
            Console.WriteLine("User ID   " + settings.UserId);
            Console.WriteLine("Password  " + new string('*', settings.Password.Length) + "\n");

            Console.Write("New server address (leave empty to skip): ");

            string serverAddress = Console.ReadLine();

            if (serverAddress != string.Empty)
            {
                // ReSharper disable once PossibleNullReferenceException
                string[] addressSplit = serverAddress.Split(':');

                if (addressSplit.Length == 1)
                {
                    settings.ServerAddress = addressSplit[0];
                    settings.ServerPort = Constants.DefaultPort;
                }
                else if (addressSplit.Length == 2)
                {
                    int port;

                    if (!int.TryParse(addressSplit[1], out port))
                    {
                        Console.WriteLine("Invalid address, skipping.");
                    }
                    else
                    {
                        settings.ServerAddress = addressSplit[0];
                        settings.ServerPort = port;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid address, skipping.");
                }
            }

            Console.Write("New user ID (leave empty to skip: ");

            string userId = Console.ReadLine();

            if (userId != string.Empty)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (userId.Length <= Constants.Database.MaxUsernameLength)
                {
                    settings.UserId = userId;
                }
                else
                {
                    Console.WriteLine("Invalid user ID, skipping.");
                }
            }

            Console.Write("New password (leave empty to skip): ");

            string password = ReadPassword();

            if (password != string.Empty)
            {
                Console.Write("Enter password again:               ");

                if (password != ReadPassword())
                {
                    Console.WriteLine("Passwords do not match, skipping.");
                }
                else
                {
                    settings.Password = password;
                }
            }

            settings.Save();

            Console.WriteLine("Settings saved.");
        }

        internal void TestConnection()
        {
            Console.WriteLine("Connection test...");

            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                Console.WriteLine(client.Login(settings.UserId, settings.Password) ? "Successfully connected and logged in." : "Connection succeeded but login failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection unsuccessful:");
                Console.WriteLine(ex.Message);
            }

            client.Disconnect();
        }

        internal void GetStorageInfo()
        {
            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    StorageInfo storageInfo = client.GetStorageInfo();

                    Console.WriteLine("Available: {0} byte ({1})", storageInfo.MaxCapacity, Util.GetByteSizeText(storageInfo.MaxCapacity));
                    Console.WriteLine("Occupied:  {0} byte ({1})", storageInfo.UsedCapacity, Util.GetByteSizeText(storageInfo.UsedCapacity));
                    Console.WriteLine("Usage:     {0} %", 100 * storageInfo.UsedCapacity / storageInfo.MaxCapacity);
                    Console.WriteLine("Stored:    {0} files", storageInfo.FileCount);
                }
                else
                {
                    Console.WriteLine("Error: Server rejected username/password.");
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection unsuccessful:");
                Console.WriteLine(ex.Message);
            }

        }

        internal void ChangePassword()
        {
            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    Console.Write("New password: ");

                    string password = ReadPassword();

                    if (password != string.Empty)
                    {
                        Console.Write("Repeat:       ");

                        if (password != ReadPassword())
                        {
                            Console.WriteLine("Error: Passwords do not match!");
                        }
                        else
                        {
                            if (client.SetPassword(password))
                            {
                                settings.Password = password;
                                settings.Save();

                                Console.WriteLine("Settings saved.");
                            }
                            else
                            {
                                Console.WriteLine("Error: Password change failed. The server rejected the new password.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Password may not be empty!");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Server rejected username/password.");
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection unsuccessful:");
                Console.WriteLine(ex.Message);
            }
        }

        internal void Upload(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: no file specified");
            }

            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    byte[] fileBuf = new byte[Constants.Network.FileBufferSize];

                    for (int i = 1; i < args.Length; i++)
                    {
                        string path = args[i];

                        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            Console.WriteLine("Uploading file " + Path.GetFileName(path) + "...");

                            string transferKey = client.InitiateUpload(Path.GetFileName(path), stream.Length);

                            if (transferKey == null)
                            {
                                Console.WriteLine("Error: Not enough space available in your storage.");
                                continue;
                            }

                            Console.Write("0 %");

                            int bytesRead;
                            long unread = stream.Length, totalRead = 0;
                            string fileSize = Util.GetByteSizeText(stream.Length);
                            Stopwatch stopwatch = Stopwatch.StartNew();

                            while ((bytesRead = stream.Read(fileBuf, 0, (int)Math.Min(Constants.Network.FileBufferSize, unread))) > 0)
                            {
                                client.UploadPacket(transferKey, fileBuf, 0, bytesRead);

                                unread -= bytesRead;
                                totalRead += bytesRead;

                                Console.CursorLeft = 0;
                                Console.Write("{0} % ({1} of {2}, {3}/s)    ", (100 * totalRead / stream.Length).ToString().PadLeft(3), Util.GetByteSizeText(totalRead).PadLeft(7), fileSize.PadLeft(7), Util.GetByteSizeText(1000 * totalRead / (stopwatch.ElapsedMilliseconds + 1)).PadLeft(7));
                            }

                            if (stream.Length == 0)
                            {
                                Console.CursorLeft = 0;
                                Console.Write("100 %");
                            }

                            //Console.WriteLine(Util.GetByteSizeText(1000 * totalRead / (stopwatch.ElapsedMilliseconds + 1)));

                            Console.WriteLine();
                            Console.WriteLine();

                            string result = client.FinishUpload(transferKey);

                            Console.WriteLine(result);
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error: Server rejected username/password.");
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Message);
            }
        }

        internal void ShowStoredFiles(bool shouldFilter)
        {
            if (Console.BufferWidth < 50)
            {
                Console.WriteLine("Please run that command with a buffer width of at least 50 characters.");
            }

            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    DateTime fromDate = DateTime.MinValue;
                    DateTime toDate = DateTime.MaxValue;
                    long fromSize = 0;
                    long toSize = long.MaxValue;
                    string filter = string.Empty;
                    int filterMatchMode = 0;

                    if (shouldFilter)
                    {
                        Console.Write("Filter: ");

                        string[] filterArgs = Console.ReadLine().Split('|');

                        for (int i = 0; i < filterArgs.Length; i++)
                        {
                            string arg = filterArgs[i];

                            if (arg.StartsWith("size>="))
                            {
                                fromSize = Util.GetRevSize(arg.Substring(6)).Value;
                            }
                            else if (arg.StartsWith("size>"))
                            {
                                fromSize = Util.GetRevSize(arg.Substring(5)).Value + 1;
                            }
                            else if (arg.StartsWith("size<="))
                            {
                                toSize = Util.GetRevSize(arg.Substring(6)).Value;
                            }
                            else if (arg.StartsWith("size<"))
                            {
                                toSize = Util.GetRevSize(arg.Substring(5)).Value - 1;
                            }
                            else if (arg.StartsWith("date>="))
                            {
                                fromDate = DateTime.Parse(arg.Substring(6));
                            }
                            else if (arg.StartsWith("date>"))
                            {
                                fromDate = DateTime.Parse(arg.Substring(5)).AddSeconds(1);
                            }
                            else if (arg.StartsWith("date<="))
                            {
                                toDate = DateTime.Parse(arg.Substring(6));
                            }
                            else if (arg.StartsWith("date<"))
                            {
                                toDate = DateTime.Parse(arg.Substring(5)).AddSeconds(-1);
                            }
                            else if (arg.StartsWith("name="))
                            {
                                filter = arg.Substring(5);
                                filterMatchMode = 1;
                            }
                            else if (arg.StartsWith("name~"))
                            {
                                filter = arg.Substring(5);
                                filterMatchMode = 2;
                            }
                            else if (arg.StartsWith("name<"))
                            {
                                filter = arg.Substring(5);
                                filterMatchMode = 3;
                            }
                            else if (arg.StartsWith("name>"))
                            {
                                filter = arg.Substring(5);
                                filterMatchMode = 4;
                            }
                            else if (arg.StartsWith("name/"))
                            {
                                StringBuilder builder = new StringBuilder();

                                builder.Append(arg.EndsWith("//") ? arg.Substring(5, arg.Length - 7) : arg.Substring(5));
                                i++;

                                while (i < filterArgs.Length)
                                {
                                    arg = filterArgs[i];

                                    builder.Append('|');

                                    if (arg.EndsWith("//"))
                                    {
                                        builder.Append(arg.Substring(0, arg.Length - 2));
                                        break;
                                    }

                                    builder.Append(arg);
                                }

                                filter = builder.ToString();
                                filterMatchMode = 5;
                            }
                        }

                        foreach (string arg in filterArgs)
                        {
                        }
                    }

                    List<FileDetails> fileList = new List<FileDetails>(250);
                    int read = client.ListFiles(fileList, 0, fromDate, toDate, fromSize, toSize, filter, filterMatchMode);

                    foreach (FileDetails file in fileList)
                    {
                        string size = Util.GetByteSizeText(file.FileSize).PadLeft(7);
                        string date = file.UploadDate.ToString("yyyy-MM-dd HH:mm:ss");

                        string fileName;
                        int textWidth = Console.BufferWidth - 33;


                        if (file.FileName.Length <= textWidth)
                        {
                            fileName = file.FileName;
                        }
                        else
                        {
                            int lastDot = file.FileName.LastIndexOf('.');

                            if (lastDot < file.FileName.Length - 12)
                            {
                                fileName = file.FileName.Substring(0, textWidth - 3) + "...";
                            }
                            else
                            {
                                fileName = file.FileName.Substring(0, (textWidth - 3) - 2 * (file.FileName.Length - lastDot)) + ".." + file.FileName.Substring(lastDot);
                            }

                            //fileName = file.FileName.Substring(0, file.FileName.Length);
                            //fileName = file.FileName.Substring(0, 32) + "..." + file.FileName.Substring(file.FileName.Length - 7);
                        }

                        Console.WriteLine("{0} {1} {2} {3}", file.FileId, date, size, fileName);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Server rejected username/password.");
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while listing files:");
                Console.WriteLine(ex.Message);
            }
        }

        internal void Delete(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: no file specified");
            }

            Config settings = Config.Load();
            NetClient client = new NetClient(settings.ServerAddress, settings.ServerPort);

            try
            {
                client.Connect();

                if (client.Login(settings.UserId, settings.Password))
                {
                    for (int i = 1; i < args.Length; i++)
                    {
                        string fileId = args[i];

                        Console.WriteLine(client.DeleteFile(fileId) ? "File {0} successfully deleted." : "File {0} was not deleted. Does that file really belong to you?", fileId);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Server rejected username/password.");
                }

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection unsuccessful:");
                Console.WriteLine(ex.Message);
            }
        }

        private static string ReadPassword()
        {
            List<char> chars = new List<char>(32);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && chars.Count > 0)
                {
                    chars.RemoveAt(chars.Count - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    while (chars.Count > 0)
                    {
                        chars.RemoveAt(chars.Count - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    chars.Add(key.KeyChar);
                    Console.Write('*');
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return new string(chars.ToArray());
        }
    }
}
