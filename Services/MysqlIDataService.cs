using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 记账App.Model;

namespace 记账App.Services
{
    class MysqlIDataService : ViewModelBase, IDataService
    {
        //LiveCharts X坐标轴数据
        private List<string> lables1 = new List<string>();
        private List<string> lables2 = new List<string>();
        public List<string> Lables1
        {
            get => lables1;
            set { lables1 = value; RaisePropertyChanged(); }
        }
        public List<string> Lables2
        {
            get => lables2;
            set { lables2 = value; RaisePropertyChanged(); }
        }
        //LiveCharts 图表数据
        public SeriesCollection seriesCollection1 = new SeriesCollection();
        public SeriesCollection seriesCollection2 = new SeriesCollection();
        public SeriesCollection SeriesCollection1
        {
            get => seriesCollection1;
            set { seriesCollection1 = value; RaisePropertyChanged(); }
        }
        public SeriesCollection SeriesCollection2
        {
            get => seriesCollection2;
            set { seriesCollection2 = value; RaisePropertyChanged(); }
        }
        //统计过去10天数日消费
        private readonly string queryStr2 = "SELECT  SUM(`cost`) AS sum, `date` FROM jizhang WHERE DATE_SUB(CURDATE(), INTERVAL 10 DAY) <= `date` group by `date`";
        //当月分类统计
        private readonly string queryStr1 = "SELECT  SUM(`cost`) AS sum, `class` FROM jizhang WHERE MONTH(`date`) = MONTH(CURDATE()) GROUP BY `class`";

        //数据库接口
        private IConfigService config;
        //private string connectStr = $"server=localhost;user=root;database=db1;port=3306;password=root";
        private string connectStr;
        private static MySqlConnection conn;

        //创建数据库
        private readonly string createDb = "CREATE TABLE if not exists `jizhang` (" +
                  "`ID` int unsigned NOT NULL AUTO_INCREMENT," +
                  "`date` varchar(10) NOT NULL," +
                  "`goods` varchar(32) NOT NULL," +
                  "`cost` float (5,1) NOT NULL," +
                   "`class` varchar(16) NOT NULL," +
                   "`comments` varchar(128) DEFAULT NULL," +
                   "PRIMARY KEY(`ID`)" +
                ") ENGINE=InnoDB AUTO_INCREMENT = 287 DEFAULT CHARSET = utf8mb3;";
        private string queryStrAll = "select * from `jizhang` where MONTH(`date`) = MONTH(CURDATE()) order by `Id` desc";

        private ObservableCollection<DailyCost> dailyCosts = new ObservableCollection<DailyCost>();
        public ObservableCollection<DailyCost> DailyCosts
        {
            get => dailyCosts;
            set { dailyCosts = value; RaisePropertyChanged(); }
        }

        //查询数据库，刷新UI显示
        public void ShowData()
        {
            //建立数据库连接
            conn = new MySqlConnection(connectStr);
            //新建数据库表
            MySqlCommand cdb = new MySqlCommand(createDb, conn);
            //查询当月总数据
            MySqlCommand showAll = new MySqlCommand(queryStrAll, conn);
            //新建查询结果变量
            MySqlDataReader reader;
            try
            {
                //打开连接
                conn.Open();
                cdb.ExecuteNonQuery();  //如果表不存在则创建表
                reader = showAll.ExecuteReader();   //查询结果的第一行
                DailyCosts.Clear();    //先清空缓存数据

                while (reader.Read())   //执行一次返回一条记录（一行数据），再次执行返回下一条，直到最后无返回值
                {
                    DailyCost result = new DailyCost();
                    Console.WriteLine("查询成功");
                    result.Date = reader["date"].ToString();            //"2021-07-08"
                    result.Comments = reader["comments"].ToString();    //"测试"
                    result.Goods = reader["goods"].ToString();          //"goods"
                    result.Cost = reader.GetFloat(3);                   //3表示列column, 0.2F
                    result.Type = reader[4].ToString();                 //"类型"
                    DailyCosts.Add(result);                             //修改了属性，触发事件PropertyChanged，系统自动调用                    
                }
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
                return;
                //System.Close();
            }
            finally
            {
                reader = null;
                conn.Close();
            }
            //更新图表LiveChart
            UpdateChart(queryStr1, SeriesCollection1, Lables1, "ColumnSeries");
            UpdateChart(queryStr2, SeriesCollection2, Lables2, "LineSeries");
        }

        public void UpdateChart(string queryStr, SeriesCollection series, List<string> lables, string seriesType)
        {
            MySqlCommand cmd = new MySqlCommand(queryStr, conn);

            ChartValues<double> values = new ChartValues<double>();
            try
            {
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                lables.Clear();
                //查询各消费类当月总数
                while (reader.Read())
                {
                    values.Add(reader.GetFloat(0));
                    lables.Add(reader[1].ToString());
                    //System.Windows.MessageBox.Show(reader["class"].ToString());
                }
                series.Clear();
                switch (seriesType)
                {
                    case "ColumnSeries":
                        series.Add(new ColumnSeries { Values = values, Title = "月" });
                        //TotleCost = $"合计：{(int)values.Sum()}";
                        break;
                    case "LineSeries":
                        series.Add(new LineSeries { Values = values, Title = "日" });
                        break;
                    default:
                        break;
                }
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
            }
            finally
            {
                conn.Close();
            }
        }

        public void InsertData(object[] values)
        {
            //Console.WriteLine($"insert into jizhang(`date`,`goods`,`cost`,`class`,`comments`) value('2021-07-09','测试goods',1.2,'类别',{comments})");
            //string insertString = $"insert into jizhang(`date`,`goods`,`cost`,`class`,`comments`) value('2021-07-09','测试goods',1.2,'类别','{comments}')";
            /*
            string insertString = $"insert into jizhang(`date`,`goods`,`cost`,`class`,`comments`) values ('{Convert.ToDateTime(values.GetValue(0)).ToString("yyyy-MM-dd")}'," +
                $"'{values[1].ToString()}','{values[2].ToString()}','{values[3].ToString()}','{values[4].ToString()}')";
             */
            string insertString = $"insert into jizhang(`date`,`goods`,`cost`,`class`,`comments`) values ('{Convert.ToDateTime(values.GetValue(0)).ToString("yyyy-MM-dd")}'," +
                $"'{values[1]}','{values[2]}','{values[3]}','{values[4]}')";

            MySqlCommand insert = new MySqlCommand(insertString, conn);
            try
            {
                //排除空值，" "
                for (int i = 0; i < 4; i++)
                {
                    if (values[i].Equals(null) || values[i].Equals("")) throw new ArgumentNullException(nameof(values));
                }
                conn.Open();
                insert.ExecuteNonQuery();   //插入数据库                
            }
            catch (MySqlException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
                return;
            }
            catch (ArgumentNullException ex)
            {
                System.Windows.MessageBox.Show($"数据不能为空!\n{ex}");
                return;
            }
            finally
            {
                conn.Close();
            }
            ShowData();
        }

        public RelayCommand<object[]> InsertCommand { get; set; }


        public ObservableCollection<DailyCost> GetDailyCost()
        {
            //throw new NotImplementedException();
            return DailyCosts;
        }

        public MysqlIDataService(IConfigService config)
        {
            this.config = config;   //构造函数注入
            connectStr = $"server={this.config.Config.Setting.MysqlCon.Server};" +
                        $"user={this.config.Config.Setting.MysqlCon.User};" +
                        $"database={this.config.Config.Setting.MysqlCon.Database};" +
                        $"port={this.config.Config.Setting.MysqlCon.Port};" +
                        $"password={this.config.Config.Setting.MysqlCon.Pwd}";
            ShowData();
            InsertCommand = new RelayCommand<object[]>(t => InsertData(t));
        }
    }
}
