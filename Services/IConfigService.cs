using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 记账App.Model;

namespace 记账App.Services
{
    public interface IConfigService
    {
        public Config Config { get; set; }
        public ObservableCollection<Types> List { get; set; }
        public void GetConfig();
        public void GetComboItems();
        public void UpdateComboItems(string[] types);
    }
}
