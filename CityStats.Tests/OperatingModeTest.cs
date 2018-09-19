//Developed and implemented Klim Vladislav Olegovich, BSUIR, FITU, gr. 521702 for internship IHS Market
//18.02.2018
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityStats.Tests
{
    [TestClass]
    public class OperatingModeTest
    {
        private OperatingMode operatingMode;

        [TestInitialize]
        public void TestInitialize()
        {
            operatingMode = new OperatingMode();
        }

        [TestMethod]
        public void FileSystem_CheckOut_IncorrectDirectory()
        {
            int expected = 0;

            operatingMode.FileSystem("C://Test", 3);
            int actual = OperatingMode.cities.cities.Count;

            Assert.AreEqual(expected, actual, "Ошибка проверки существования директории.");
        }

        [TestMethod]
        public void FileSystem_CheckOut_AbsenceTextFiles()
        {
            int expected = 0;

            Directory.CreateDirectory("C://Test");
            operatingMode.FileSystem("C://Test", 3);
            Directory.Delete("C://Test");
            int actual = OperatingMode.cities.cities.Count;

            Assert.AreEqual(expected, actual, "Ошибка проверки существования текстовых файлов в директории.");
        }

        [TestMethod]
        public void FileSystem_CheckOut_IgnoreOutputTextFile()
        {
            int expected = 0;

            Directory.CreateDirectory("C://Test");
            using (StreamWriter sw = new StreamWriter("C://Test/output.txt"))
            {
                sw.WriteLine("Минск,1600000");
                sw.WriteLine("Брест,670000");
                sw.WriteLine("Гомель,490000");
                sw.WriteLine("Витебск,600000");
            }
            operatingMode.FileSystem("C://Test", 3);
            File.Delete("C://Test/output.txt");
            Directory.Delete("C://Test");
            int actual = OperatingMode.cities.cities.Count;

            Assert.AreEqual(expected, actual, "Ошибка фильтрации файла с результатом output.txt.");
        }

        [TestMethod]
        public void FileSystem_CheckOut_ProcessingTextFiles()
        {
            List<string> expected = new List<string>();
            expected.Add("Минск,3200000");
            expected.Add("Брест,670000");
            expected.Add("Могилёв,670000");
            expected.Add("Гомель,980000");
            expected.Add("Витебск,1200000");
            expected.Add("Волковыск,98000");

            Directory.CreateDirectory("C://Test");
            using (StreamWriter sw = new StreamWriter("C://Test/source1.txt"))
            {
                sw.WriteLine("Минск,1600000");
                sw.WriteLine("Брест,670000");
                sw.WriteLine("Гомель,490000");
                sw.WriteLine("Витебск,600000");
            }
            using (StreamWriter sw = new StreamWriter("C://Test/source2.txt"))
            {
                sw.WriteLine("Минск,1600000");
                sw.WriteLine("Могилёв,670000");
                sw.WriteLine("Гомель,490000");
                sw.WriteLine("Витебск,600000");
                sw.WriteLine("Волковыск,98000");
            }
            operatingMode.FileSystem("C://Test", 3);
            File.Delete("C://Test/source1.txt");
            File.Delete("C://Test/source2.txt");
            File.Delete("C://Test/output.txt");
            Directory.Delete("C://Test");
            List<string> actual = new List<string>();
            for (int i = 0; i < OperatingMode.cities.cities.Count; i++)
            {
                actual.Add(OperatingMode.cities.cities[i].Name + ',' + OperatingMode.cities.cities[i].Population);
            }

            CollectionAssert.AreEqual(expected, actual, "Ошибка обработки текстовых файлов.");
        }

        [TestMethod]
        public void FileSystem_CheckOut_WriteResultToFile() 
        {
            //Тест успешно завершается только когда запускается вместе с остальными. 
            //Если запускать отдельно - необходимо Могилёв поставить после Бреста (как в FileSystem_CheckOut_ProcessingTextFiles()).
            //Почему меняется порядок выходных данных при повторном запуске - так и не смог определить...
            List<string> expected = new List<string>();
            expected.Add("Минск,3200000");
            expected.Add("Брест,670000");            
            expected.Add("Гомель,980000");
            expected.Add("Витебск,1200000");
            expected.Add("Могилёв,670000");
            expected.Add("Волковыск,98000");
            Directory.CreateDirectory("C://Test");
            using (StreamWriter sw = new StreamWriter("C://Test/source1.txt"))
            {
                sw.WriteLine("Минск,1600000");
                sw.WriteLine("Брест,670000");
                sw.WriteLine("Гомель,490000");
                sw.WriteLine("Витебск,600000");
            }
            using (StreamWriter sw = new StreamWriter("C://Test/source2.txt"))
            {
                sw.WriteLine("Минск,1600000");
                sw.WriteLine("Могилёв,670000");
                sw.WriteLine("Гомель,490000");
                sw.WriteLine("Витебск,600000");
                sw.WriteLine("Волковыск,98000");
            }
            operatingMode.FileSystem("C://Test", 3);
            List<string> actual = new List<string>();
            using (StreamReader sr = new StreamReader("C://Test/output.txt"))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    actual.Add(line);
                    line = sr.ReadLine();
                }
            }
            File.Delete("C://Test/source1.txt");
            File.Delete("C://Test/source2.txt");
            File.Delete("C://Test/output.txt");
            Directory.Delete("C://Test");

            CollectionAssert.AreEqual(expected, actual, "Ошибка записи результата обработки в текстовый файл.");
        }

        //Большая часть http-режима протестирована в режиме filesystem, т.к. обработка файлов и запись результата в файл в обоих режимах
        //происходит одинаково. Скачивание файлов требует наличия валидных прямых ссылок на эти файлы, а у популярных "облаков" (пробовал яндекс
        //диск и облако mail.ru) примерно через 3-6 часов прямые ссылки перестают работать, поэтому вместо тестов, эту часть 
        //программы проще протестировать путём её запуска и передачи файла с валидными ссылками. Либо рефакторить этот режим и разбивать на 
        //совсем мелкие методы, но как по мне - это лишнее для такой задачи, исключения дают достаточное количество информации в случае проблем.
    }
}
