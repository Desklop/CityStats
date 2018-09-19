//Developed and implemented Klim Vladislav Olegovich, BSUIR, FITU, gr. 521702 for internship IHS Market
//18.02.2018
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Configuration;

namespace CityStats
{
    public class Programm
    {        
        static void Main(string[] args)
        {
            int maximumNumberOfThreads = 0;
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings.Count != 0)
                {
                    maximumNumberOfThreads = Convert.ToInt32(appSettings["N"]);
                }
                else
                {
                    Console.WriteLine("Ошибка: App.config не содержит параметров. Использовано значение по умолчанию.");
                    maximumNumberOfThreads = 5;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка чтения параметров из App.config: " + e.Message);
                Console.WriteLine("Использовано значение по умолчанию.");
                maximumNumberOfThreads = 5;
            }

            OperatingMode operatingMode = new OperatingMode();

            if (args.Length == 2)
            {
                if (args[0] == "filesystem")
                {
                    operatingMode.FileSystem(args[1], maximumNumberOfThreads);
                }
                else if (args[0] == "http")
                {
                    operatingMode.Http(args[1], maximumNumberOfThreads);
                }
                else
                {
                    Console.WriteLine("Ошибка: неверный первый параметр.");
                    Console.WriteLine("Первый параметр может быть:");
                    Console.WriteLine("\t'filesystem', если второй параметр указывает путь к директории, в которой хранятся текстовые файлы с исходными данными.");
                    Console.WriteLine("\t'http', если второй параметр указывает путь к файлу, в котором перечислены http адреса входных документов (один адрес в каждой строке).");
                }
            }
            else
            {
                Console.WriteLine("Ошибка: введено неверное количество параметров.");
                Console.WriteLine("Первый параметр может быть:");
                Console.WriteLine("\t'filesystem', если второй параметр указывает путь к директории, в которой хранятся текстовые файлы с исходными данными.");
                Console.WriteLine("\t'http', если второй параметр указывает путь к файлу, в котором перечислены http адреса входных документов (один адрес в каждой строке).");
            }
            Console.ReadLine();
        }
    }
}