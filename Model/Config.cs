using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 记账App.Model
{
    public class Config
    {
        public string Version { get; set; }
        public string Readme { get; set; }
        public Setting Setting { get; set; }
    }

    public class Setting
    {
        public string[] Type { get; set; }
        public MysqlCon MysqlCon { get; set; }
        public string Sqlite { get; set; }
    }
    public class MysqlCon
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Database { get; set; }
    }

    //新建类创建List<T>进行数据绑定到ComboBox的ItemSource
    public class Types
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
}
