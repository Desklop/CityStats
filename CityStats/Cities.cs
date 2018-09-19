//Developed and implemented Klim Vladislav Olegovich, BSUIR, FITU, gr. 521702 for internship IHS Market
//18.02.2018
using System;
using System.Collections.Generic;

namespace CityStats
{
    public class Cities
    {
        public class City
        {
            public string Name { get; set; }
            public long Population { get; set; }

            public City(string name, long population)
            {
                Name = name;
                Population = population;
            }
        }

        public List<City> cities;

        public Cities()
        {
            cities = new List<City>();
        }

        public void Add(string nameAndPopulation)
        {
            bool checkToAddNewCity = true;

            string name = nameAndPopulation.Substring(0, nameAndPopulation.IndexOf(','));
            name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
            int population = Convert.ToInt32(nameAndPopulation.Substring(nameAndPopulation.IndexOf(',') + 1));

            for (int i = 0; i < cities.Count; i++)
            {
                if (string.Equals(cities[i].Name, name))
                {
                    cities[i].Population += population;
                    checkToAddNewCity = false;
                }
            }
            if (checkToAddNewCity)
            {
                cities.Add(new City(name, population));
            }
        }

        public string Get(int index)
        {
            string nameAndPopulation = null;
            if (index < cities.Count && index >= 0)
            {
                nameAndPopulation = cities[index].Name + "," + cities[index].Population;
            }
            return nameAndPopulation;
        }
    }
}
