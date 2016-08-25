using System;
using System.IO;
using System.Reflection;

namespace domi1819.UpCliClient
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", new AssemblyName(eventArgs.Name).Name + ".dll");

                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            };

            Run(args);
        }

        private static void Run(string[] args)
        {
            Console.WriteLine("================================");
            Console.WriteLine("UpCliClient " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("https://up.domi1819.xyz");
            Console.WriteLine("2016 domi1819");
            Console.WriteLine("All rights reserved");
            Console.WriteLine("================================\n");

            UpCliClient client = new UpCliClient();

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-config":
                    case "-c":
                    {
                        client.Configure();
                        break;
                    }
                    case "-password":
                    case "-p":
                    {
                        client.ChangePassword();
                        break;
                    }
                    case "-test":
                    case "-t":
                    {
                        client.TestConnection();
                        break;
                    }
                    case "-upload":
                    case "-u":
                    {
                        client.Upload(args);
                        break;
                    }
                    case "-info":
                    case "-i":
                    {
                        client.GetStorageInfo();
                        break;
                    }
                    case "-storage":
                    case "-s":
                    {
                        client.ShowStoredFiles(false);
                        break;
                    }
                    case "-filter":
                    case "-f":
                    {
                        client.ShowStoredFiles(true);
                        break;
                    }
                    case "-delete":
                    case "-d":
                    {
                        client.Delete(args);
                        break;
                    }
                    default:
                    {
                        PrintHelp();
                        break;
                    }
                }

                return;
            }

            PrintHelp();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Parameters:");
            Console.WriteLine("-h / -help      show a list of arguments");
            Console.WriteLine("-c / -config    configure ^up settings");
            Console.WriteLine("-t / -test      test server connection and credentials");
            Console.WriteLine("-p / -password  change password");
            Console.WriteLine("-u / -upload    upload one or more file");
            Console.WriteLine("-s / -storage   show stored files");
            Console.WriteLine("-f / -filter    filter stored files");
            Console.WriteLine("-d / -delete    delete one or more files");
        }
    }
}
