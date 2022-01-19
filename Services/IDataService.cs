using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 记账App.Model;

namespace 记账App.Services
{
    public interface IDataService
    {
        public SeriesCollection SeriesCollection1 { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public List<string> Lables1 { get; set; }
        public List<string> Lables2 { get; set; }
        public ObservableCollection<DailyCost> DailyCosts { get; set; }
        public void InsertData(object[] values);
        //public void ShowData();
    }
}
