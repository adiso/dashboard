using System;
using System.Collections.Generic;
using System.Linq;
namespace DapperDashboard.ViewModels
{
    public class InvoiceDetailViewModel
    {
        public int InvoiceID { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<InvoiceLineItemViewModel> LineItems { get; set; }
        public string CustomerAddress => "123 Customer Lane<br>Suite 200<br>New York, NY 10001";
        public string OurCompanyName => "My ASP.NET Company";
        public string OurAddress => "456 Developer Ave<br>Techville, TX 75001<br>info@mycompany.com";
        public decimal Subtotal => LineItems?.Sum(line => line.Total) ?? 0;
        public decimal TaxRate => 0.0825m;
        public decimal Tax => Subtotal * TaxRate;
        public decimal Total => Subtotal + Tax;
        public InvoiceDetailViewModel()
        {
            LineItems = new List<InvoiceLineItemViewModel>();
        }
    }
}