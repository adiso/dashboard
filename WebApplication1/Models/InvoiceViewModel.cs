using System;
namespace DapperDashboard.ViewModels
{
    public class InvoiceViewModel
    {
        public string InvoiceReference { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}