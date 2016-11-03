﻿using System;
using System.Linq;

using CarsFactory.Data;
using CarsFactory.Models.Enums;
using CarsFactory.Reports.Documents.Contracts;
using CarsFactory.Reports.Reports.Contracts;

namespace CarsFactory.Reports.Reports
{
    public class TotalRevenueInLastYearByDealerReport : IReport
    {
        public void Generate(IDocumentAdapter document, CarsFactoryDbContext dbContext)
        {
            var lastYear = DateTime.Now.Year - 1;
            var totalRevenueForThePastMonth = (from dealer in dbContext.Dealers
                                               let totalRevenue =
                                                   (decimal?)dealer.Cars.Where(
                                                                               car =>
                                                                                   car.Order != null &&
                                                                                   car.Order.Date.Year > lastYear &&
                                                                                   car.Order.OrderStatus == OrderStatus.Closed)
                                                                   .Sum(c => c.Price)
                                               let town = dealer.Town.Name
                                               let orderCount = dealer.Cars.Count(car => car.Order != null
                                                                                         && car.Order.Date.Year > lastYear
                                                                                         && car.Order.OrderStatus == OrderStatus.Closed)
                                               orderby totalRevenue descending
                                               select new
                                                      {
                                                          Dealer = dealer.Name,
                                                          TotalRevenue = totalRevenue,
                                                          Town = town,
                                                          TotalOrders = orderCount
                                                      })
                .ToList();

            document.AddMetadata()
                    .AddRow($"Total revenue by dealer for the last year.")
                    .AddTabularData(totalRevenueForThePastMonth)
                    .Save();
        }
    }
}
