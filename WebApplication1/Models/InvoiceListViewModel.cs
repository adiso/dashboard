using System.Collections.Generic;
namespace DapperDashboard.ViewModels
{
    public class InvoiceListViewModel
    {
        public IEnumerable<InvoiceViewModel> Invoices { get; set; }
        public PaginationInfoViewModel Pagination { get; set; }
    }
}