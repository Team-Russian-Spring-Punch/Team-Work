﻿using CarsFactory.MongoDb.Data;
using Dealership.Data;
using System;
using System.Data;
using System.Linq;

namespace Cars.Client
{
    public class Startup
    {
        public static void Main()
        {
            using (var dbContext = new CarsFactoryDbContext())
            {
                dbContext.Database.CreateIfNotExists();
            }

            Startup.GetMongoData();
        }

        private async static void GetMongoData()
        {
            try
            {
                var repo = new MongoDbRepository();

                var cars = (await repo.GetCarsData()).ToList();

                var ctx = new CarsFactoryDbContext();
                using (ctx)
                {
                    foreach (var car in cars)
                    {
                        if (!ctx.Cars.Any(pl => pl.Id == car.Id))
                        {
                            ctx.Cars.Add(car);
                        }
                    }
                    ctx.SaveChanges();
                }
            }
            catch (DataException)
            {
                throw new ArgumentException("MongoDb is not set up correctly.");
            }
        }
    }
}