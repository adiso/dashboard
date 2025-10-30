using System;
namespace DapperDashboard.ViewModels
{
    public class OrderViewModel
    {
        public string OrderReference { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}