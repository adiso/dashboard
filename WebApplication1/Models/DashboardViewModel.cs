using System.Collections.Generic;
namespace DapperDashboard.ViewModels
{
    public class DashboardViewModel
    {
        public KpiViewModel Kpis { get; set; }
        public IEnumerable<OrderViewModel> CurrentOrders { get; set; }
        public IEnumerable<InvoiceViewModel> RecentInvoices { get; set; }
        public IEnumerable<TopProductViewModel> TopProducts { get; set; }
        public IEnumerable<ContactViewModel> KeyContacts { get; set; }
        public SalesChartViewModel SalesChart { get; set; }
    }
}