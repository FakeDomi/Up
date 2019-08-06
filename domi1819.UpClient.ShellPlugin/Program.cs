using System;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace domi1819.UpClient.ShellPlugin
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("*", true);
                key?.Close();

                key = Registry.ClassesRoot.OpenSubKey("*\\shell\\Up", true);

                if (key == null)
                {
                    Console.WriteLine("The Up Shell Plugin is currently not installed. Press ENTER to install it.");

                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        key = Registry.ClassesRoot.CreateSubKey("*\\shell\\Up\\command");
                        key.SetValue(null, $"\"{Directory.GetCurrentDirectory()}\\up.exe\" -upload \"%1\"");
                        key.Close();

                        Console.WriteLine("Successfully installed the Up Shell Plugin.");
                    }
                    else
                    {
                        Console.WriteLine("Canceled by user.");
                    }
                }
                else
                {
                    key.Close();

                    Console.WriteLine("The Up Shell Plugin is already installed.");
                    Console.WriteLine("Press ENTER to uninstall it, or R to reinstall it.");
                    ConsoleKeyInfo keyPress = Console.ReadKey(true);

                    if (keyPress.Key == ConsoleKey.Enter)
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree("*\\shell\\Up");

                        Console.WriteLine("Successfully uninstalled the Up Shell Plugin.");
                    }
                    else if (keyPress.Key == ConsoleKey.R)
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree("*\\shell\\Up", false);
                        
                        key = Registry.ClassesRoot.CreateSubKey("*\\shell\\Up\\command");
                        key.SetValue(null, $"\"{Directory.GetCurrentDirectory()}\\up.exe\" -upload \"%1\"");
                        key.Close();

                        Console.WriteLine("Successfully reinstalled the Up Shell Plugin.");
                    }
                    else
                    {
                        Console.WriteLine("Canceled by user.");
                    }
                }
            }
            catch (SecurityException)
            {
                Console.WriteLine("Could not access the registry.");
                Console.WriteLine("Make sure you have write access (run as admin).");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
