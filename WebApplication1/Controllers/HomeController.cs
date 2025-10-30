using Dapper;
using DapperDashboard.ViewModels;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DapperDashboard.Extensions;
using System.Collections.Generic;

namespace DapperDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private class ChartDataPoint
        {
            public string Label { get; set; }
            public decimal DataPoint { get; set; }
        }

        public async Task<ActionResult> Index()
        {
            var loggedInCustomerId = User.Identity.GetCustomerId();

            var sql = @"
                -- 1. KPI: Sales YTD
                SELECT ISNULL(SUM(Amount), 0) AS SalesYtd FROM Invoices WHERE CustomerID = @Id AND [Status] = 'Paid' AND YEAR(IssueDate) = YEAR(GETDATE());
                -- 2. KPI: Current Orders
                SELECT COUNT(OrderID) AS CurrentOrders FROM Orders WHERE CustomerID = @Id AND [Status] IN ('Processing', 'Shipped');
                -- 3. KPI: Open Invoices
                SELECT COUNT(InvoiceID) AS OpenInvoices FROM Invoices WHERE CustomerID = @Id AND [Status] IN ('Draft', 'Overdue');
                -- 4. KPI: Key Contacts
                SELECT COUNT(ContactID) AS KeyContacts FROM Contacts WHERE CustomerID = @Id;
                -- 5. Top Products (Top 5)
                SELECT TOP 5 p.ProductName, p.SKU, SUM(ol.Quantity) AS UnitsSold
                FROM OrderLines ol JOIN Orders o ON ol.OrderID = o.OrderID JOIN Products p ON ol.ProductID = p.ProductID
                WHERE o.CustomerID = @Id GROUP BY p.ProductID, p.ProductName, p.SKU ORDER BY UnitsSold DESC;
                -- 6. Current Orders (Top 4 recent)
                SELECT TOP 4 OrderReference, OrderDate, TotalAmount, [Status] FROM Orders WHERE CustomerID = @Id ORDER BY OrderDate DESC;
                -- 7. Recent Invoices (Top 3 recent)
                SELECT TOP 3 InvoiceReference, IssueDate, DueDate, Amount, [Status] FROM Invoices WHERE CustomerID = @Id ORDER BY IssueDate DESC;
                -- 8. Key Contacts (Top 3)
                SELECT TOP 3 ContactName, Title FROM Contacts WHERE CustomerID = @Id;
                -- 9. Chart Data (Last 12 Months)
                ;WITH Months AS (SELECT 0 AS M UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11),
                MonthSeries AS (SELECT DATEFROMPARTS(YEAR(DATEADD(month, -M, GETDATE())), MONTH(DATEADD(month, -M, GETDATE())), 1) AS MonthStart FROM Months)
                SELECT FORMAT(MS.MonthStart, 'MMM yyyy') AS Label, ISNULL(SUM(I.Amount), 0) AS DataPoint
                FROM MonthSeries MS LEFT JOIN Invoices I ON I.CustomerID = @Id AND I.[Status] = 'Paid' AND DATEFROMPARTS(YEAR(I.IssueDate), MONTH(I.IssueDate), 1) = MS.MonthStart
                GROUP BY MS.MonthStart ORDER BY MS.MonthStart;
            ";

            var viewModel = new DashboardViewModel();

            using (var connection = new SqlConnection(_connStr))
            {
                await connection.OpenAsync();
                using (var multi = await connection.QueryMultipleAsync(sql, new { Id = loggedInCustomerId }))
                {
                    var kpis = new KpiViewModel
                    {
                        SalesYtd = await multi.ReadSingleAsync<decimal>(),
                        CurrentOrders = await multi.ReadSingleAsync<int>(),
                        OpenInvoices = await multi.ReadSingleAsync<int>(),
                        KeyContacts = await multi.ReadSingleAsync<int>()
                    };
                    viewModel.Kpis = kpis;
                    viewModel.TopProducts = (await multi.ReadAsync<TopProductViewModel>()).ToList();
                    viewModel.CurrentOrders = (await multi.ReadAsync<OrderViewModel>()).ToList();
                    viewModel.RecentInvoices = (await multi.ReadAsync<InvoiceViewModel>()).ToList();
                    viewModel.KeyContacts = (await multi.ReadAsync<ContactViewModel>()).ToList();

                    var chartData = (await multi.ReadAsync<ChartDataPoint>()).ToList();
                    viewModel.SalesChart = new SalesChartViewModel
                    {
                        Labels = chartData.Select(d => d.Label).ToList(),
                        DataPoints = chartData.Select(d => d.DataPoint).ToList()
                    };
                }
            }
            ViewBag.CustomerName = await GetCustomerName(loggedInCustomerId);
            return View(viewModel);
        }

        private async Task<string> GetCustomerName(int customerId)
        {
            using (var connection = new SqlConnection(_connStr))
            {
                return await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT CustomerName FROM Customers WHERE CustomerID = @Id", new { Id = customerId });
            }
        }
    }
}