//Developed and implemented Klim Vladislav Olegovich, BSUIR, FITU, gr. 521702 for internship IHS Market
//18.02.2018
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityStats.Tests
{
    [TestClass]
    public class CitiesTest
    {
        private Cities cities;
        private List<Cities.City> listCity;

        [TestInitialize]
        public void TestInitialize()
        {
            cities = new Cities();
            listCity = new List<Cities.City>();
            listCity.Add(new Cities.City("Минск", 1687000));
        }

        [TestMethod]
        public void Add_CheckOut_ContainsItems()
        {
            cities.Add("Минск,1687000");

            Assert.AreEqual(listCity.First().Name, cities.cities.First().Name, "Ошибка выделения из входной строки имени.");
            Assert.AreEqual(listCity.First().Population, cities.cities.First().Population, "Ошибка выделения из входной строки населения.");
        }

        [TestMethod]
        public void Add_CheckOut_WithoutRepeatitionOfItems()
        {
            cities.Add("Минск,1600000");
            cities.Add("минск,18000");
            cities.Add("Минск,69000");

            Assert.AreEqual(listCity.Count, cities.cities.Count, "Ошибка в обработке повторяющихся входных данных.");
            Assert.AreEqual(listCity.First().Name, cities.cities.First().Name, "Ошибка выделения из входной строки имени.");
            Assert.AreEqual(listCity.First().Population, cities.cities.First().Population, "Ошибка выделения из входной строки населения.");
        }

        [TestMethod]
        public void Get_CheckOut_OutputItems()
        {
            string expected = "Минск,1600000";

            cities.Add(expected);
            string actual = cities.Get(0);

            Assert.AreEqual(expected, actual, "Ошибка объединения имени и населения в одну строку.");
        }

        [TestMethod]
        public void Get_CheckOut_OutOfBoundsList()
        {
            string expected = null;

            cities.Add("Минск,1600000");
            cities.Add("Брест,980000");

            string actual = cities.Get(2);

            Assert.AreEqual(expected, actual, "Ошибка проверки границ списка.");
        }
    }
}
