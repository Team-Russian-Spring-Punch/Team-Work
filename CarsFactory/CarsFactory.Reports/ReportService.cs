﻿namespace CarsFactory.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ReportManagers;
    using ReportManagers.Abstract;

    using Reports.Contracts;

    public class ReportService
    {
        public void SaveAllReports(string directoryPath)
        {
            var allReports = this.GetAllReports();
            ReportManager manager = new PdfReportManager();
            manager.Add(allReports);
            manager.GenerateReports(directoryPath);
        }

        private ICollection<IReport> GetAllReports()
        {
            //TODO: filter differend kinds of reports
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var typeInfos = assembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(IReport)));

            var reports = new List<IReport>();
            foreach (var typeInfo in typeInfos)
            {
                var report = Activator.CreateInstance(typeInfo) as IReport;
                reports.Add(report);
            }

            return reports;
        }
    }
}
