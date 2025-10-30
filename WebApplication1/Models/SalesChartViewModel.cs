using System.Collections.Generic;
namespace DapperDashboard.ViewModels
{
    public class SalesChartViewModel
    {
        public List<string> Labels { get; set; }
        public List<decimal> DataPoints { get; set; }
        public SalesChartViewModel()
        {
            Labels = new List<string>();
            DataPoints = new List<decimal>();
        }
    }
}