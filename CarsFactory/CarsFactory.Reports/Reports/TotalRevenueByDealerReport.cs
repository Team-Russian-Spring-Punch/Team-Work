﻿using System;
using System.Linq;

using CarsFactory.Data;
using CarsFactory.Models.Enums;
using CarsFactory.Reports.Documents.Contracts;
using CarsFactory.Reports.Reports.Contracts;

namespace CarsFactory.Reports.Reports
{
    public class TotalRevenueByDealerReport : IReport
    {
        public void Generate(IDocumentAdapter document)
        {
            var dbContext = new CarsFactoryDbContext();

            using (dbContext)
            {
                var totalRevenueByDealers = (from dealer in dbContext.Dealers
                                             let totalRevenue =
                                                 (decimal?)dealer.Cars.Where(
                                                                             y =>
                                                                                 y.OrderId != null &&
                                                                                 y.Order.OrderStatus == OrderStatus.Closed)
                                                                 .Sum(x => x.Price)
                                             let town = dealer.Town.Name
                                             let orderCount =
                                                 dealer.Cars.Count(
                                                                   y =>
                                                                       y.OrderId != null &&
                                                                       y.Order.OrderStatus == OrderStatus.Closed)
                                             select new
                                                    {
                                                        Dealer = dealer.Name,
                                                        TotalRevenue = totalRevenue,
                                                        Town = town,
                                                        TotalOrders = orderCount
                                                    })
                    .Take(10)
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                document.AddMetadata()
                        .AddRow($"Top 10 total revenue by dealer for all time. Generated on {DateTime.Now}")
                        .AddTabularData(totalRevenueByDealers)
                        .Save();
            }
        }
    }
}
