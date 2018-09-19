//Developed and implemented Klim Vladislav Olegovich, BSUIR, FITU, gr. 521702 for internship IHS Market
//18.02.2018
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace CityStats
{
    public class OperatingMode
    {
        public static Cities cities;
        private static Semaphore semaphor;

        public OperatingMode()
        {
            cities = new Cities();
        }

        public void FileSystem(string sourceDirectory, int numberOfThreads)
        {
            if (Directory.Exists(sourceDirectory))
            {
                string[] files = Directory.GetFiles(sourceDirectory, "*.txt", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Substring(files[i].LastIndexOf('\\') + 1) == "output.txt")
                    {
                        for (int j = i; j < files.Length - 1; j++)
                        {
                            string temp = files[j];
                            files[j] = files[j + 1];
                            files[j + 1] = temp;
                        }
                        Array.Resize(ref files, files.Length - 1);
                        break;
                    }
                }
                if (files.Length != 0)
                {                    
                    Console.WriteLine("Найдено {0} файла(-ов).", files.Length);
                    Stopwatch stopWatch = new Stopwatch();
                    List<Thread> threads = new List<Thread>();
                    semaphor = new Semaphore(1, numberOfThreads);
                    int numberOfReadedFiles = 0;
                    stopWatch.Start();
                    do
                    {
                        for (int i = numberOfReadedFiles; i < numberOfThreads + numberOfReadedFiles && i < files.Length; i++)
                        {
                            threads.Add(new Thread(new ParameterizedThreadStart(ReadFileAndProcessing)));
                            threads[i].Start(files[i]);
                        }
                        for (int i = numberOfReadedFiles; i < threads.Count; i++)
                        {
                            threads[i].Join();
                        }
                        numberOfReadedFiles += numberOfThreads;
                    } while (numberOfReadedFiles < files.Length);
                    stopWatch.Stop();

                    WriteResultToFile(sourceDirectory);

                    Console.WriteLine("Файлы считаны и обработаны за " + stopWatch.ElapsedMilliseconds + " мс. Результат в output.txt.");
                }
                else
                {
                    Console.WriteLine("Ошибка: директория не содержит *.txt файлов.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Ошибка: директория не найдена.");
                return;
            }
        }

        private static void ReadFileAndProcessing(object data)
        {
            string path = (string)data;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        semaphor.WaitOne();
                        cities.Add(line);
                        semaphor.Release();
                        line = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка чтения исходного файла: " + e.Message);
                Environment.Exit(0);
            }
        }

        public void Http(string httpAdressesFile, int numberOfThreads)
        {
            List<string> httpAdresses = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(httpAdressesFile))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        httpAdresses.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка чтения файла с адресами: " + e.Message);
                httpAdresses.Clear();
                return;
            }
            finally
            {
                Console.WriteLine("Адреса считаны.");                
            }
            if (httpAdresses.Count != 0)
            {
                Console.WriteLine("Найдено {0} файла(-ов).", httpAdresses.Count);
                Stopwatch stopWatch = new Stopwatch();
                List<Thread> threads = new List<Thread>();
                semaphor = new Semaphore(1, numberOfThreads);
                int numberOfDownloadedFiles = 0;
                stopWatch.Start();
                do
                {
                    for (int i = numberOfDownloadedFiles; i < numberOfThreads + numberOfDownloadedFiles && i < httpAdresses.Count; i++)
                    {
                        threads.Add(new Thread(new ParameterizedThreadStart(DownloadFileAndProcessing)));
                        threads[i].Start(httpAdresses[i]);
                    }
                    for (int i = numberOfDownloadedFiles; i < threads.Count; i++)
                    {
                        threads[i].Join();
                    }
                    numberOfDownloadedFiles += numberOfThreads;
                } while (numberOfDownloadedFiles < httpAdresses.Count);
                stopWatch.Stop();

                int index = httpAdressesFile.LastIndexOf('/');
                if (index == -1)
                    index = httpAdressesFile.LastIndexOf('\\');
                WriteResultToFile(httpAdressesFile.Substring(0, index));

                Console.WriteLine("Файлы скачаны и обработаны за " + stopWatch.ElapsedMilliseconds + " мс. Результат в output.txt.");
            }
            else
            {
                Console.WriteLine("Ошибка: файл с адресами пустой.");
                return;
            }
        }

        private static void DownloadFileAndProcessing(object data)
        {
            string httpAdresses = (string)data;
            try
            {
                WebClient webClient = new WebClient();
                byte[] downloadedData = null;
                webClient.DownloadDataCompleted +=
                    delegate (object sender, DownloadDataCompletedEventArgs eventArgs)
                    {
                        try
                        {
                            downloadedData = eventArgs.Result;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Ошибка при загрузке/обработке файла: " + eventArgs.Error.Message);
                            Environment.Exit(0);
                        }
                    };
                webClient.DownloadDataAsync(new Uri(httpAdresses));

                while (webClient.IsBusy)
                {
                    Thread.Sleep(100);
                }
                
                Encoding enc8 = Encoding.UTF8;
                string decodedData = enc8.GetString(downloadedData);
                decodedData = decodedData.Remove(0, 1);  //Костыль, без него строка начинается с '?', который портит всю дальнейшую обработку
                
                do
                {
                    int index = decodedData.IndexOf("\r");
                    if (index != -1)
                    {
                        semaphor.WaitOne();
                        cities.Add(decodedData.Substring(0, index));
                        semaphor.Release();
                        decodedData = decodedData.Remove(0, index + 2);
                    }
                    else
                    {
                        semaphor.WaitOne();
                        cities.Add(decodedData);
                        semaphor.Release();
                        decodedData = decodedData.Remove(0);
                    }
                } while (decodedData != "");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при загрузке/обработке файла: " + e.Message);
                Environment.Exit(0);
            }
        }

        private static void WriteResultToFile(string sourceDirectory)
        {
            try
            {                
                using (StreamWriter sw = new StreamWriter(sourceDirectory + "/output.txt"))
                {
                    int index = 0;
                    string line = cities.Get(index);
                    while (line != null)
                    {
                        sw.WriteLine(line);
                        index++;
                        line = cities.Get(index);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при записи результата в файл: " + e.Message);
                Environment.Exit(0);
            }
        }
    }
}
