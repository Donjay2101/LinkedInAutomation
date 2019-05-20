using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace LinkedInRequestSolution
{
    class Program
    {
        static void Main()
        {
            int _choice = 0;
            string search_word = string.Empty;



            var appsettings = GetCredentials();
            if (appsettings == null)
            {
                Console.WriteLine("enter your registered linked in email ");
                RequestSender.Instance.Username = Console.ReadLine();

                Console.WriteLine("Enter password");
                string pass = "";
                do
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    // Backspace Should Not Work
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                        {
                            pass = pass.Substring(0, (pass.Length - 1));
                            Console.Write("\b \b");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            RequestSender.Instance.Password = pass;
                            Console.WriteLine("\n");
                            break;
                        }
                    }
                } while (true);
            }
            else
            {
                RequestSender.Instance.Username = appsettings.Username;
                RequestSender.Instance.Password = appsettings.Password;
                int.TryParse(appsettings.Choice, out _choice);
                search_word = appsettings.Keyword;
            }

            if (!(string.IsNullOrWhiteSpace(RequestSender.Instance.Username) && string.IsNullOrWhiteSpace(RequestSender.Instance.Password)))
            {
                RequestSender.Instance.Init();
            }
            Console.WriteLine("\n\n\nWelcome to linkedIn automation. !!!!!  choose any of below numbers to start magic  !!!!!!");
            char resume;
            do
            {
                if (_choice <= 0)
                {
                    Console.WriteLine("1. send request to those who comes as suggestion in \"Recommanded for you\".  ");
                    Console.WriteLine("2. Search in first and then send request to output.");
                    var ch = Console.ReadLine();
                    int.TryParse(ch.ToString(), out _choice);
                }

                //_choice = Convert.ToInt32(Console.ReadLine());
                switch (_choice)
                {
                    case 1:
                        RequestSender.Instance.GotoNetworks();
                        RequestSender.Instance.SendRequest();
                        break;
                    case 2:

                        if (string.IsNullOrWhiteSpace(search_word))
                        {
                            Console.WriteLine("Enter the search keyword.");
                            search_word = Console.ReadLine();
                        }
                        RequestSender.Instance.SearchInWeb(search_word);
                        break;
                    default:

                        Console.WriteLine("not a valid choice.");

                        break;
                }

                Console.WriteLine("do you wish to continue? press Y for yes.");
                resume = Convert.ToChar(Console.ReadLine());

            } while (resume == 'Y' || resume == 'y');






            Console.ReadLine();
        }

        public static AppSettings GetCredentials()
        {
            var filePath = Assembly.GetExecutingAssembly().Location;
            var appsettingsPath = filePath.Substring(0, filePath.LastIndexOf("\\"));
            var appSettingsfile = $"{appsettingsPath}\\appSettings.json";
            using (var file = File.Open(appSettingsfile, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    var jsonData = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<AppSettings>(jsonData);
                }

            }
        }
    }




}
