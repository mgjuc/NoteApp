using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 记账App.Model
{
    public class DailyCost
    {
        //自动属性可以省略定义 id
        public string Comments { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Goods { get; set; }
        public float Cost { get; set; }
        public string Id { get; set; }
    }
}
