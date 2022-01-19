using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 记账App.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;

namespace 记账App.Services
{
    public class SqliteIDataService : ViewModelBase, IDataService
    {
        private ILogger<SqliteIDataService> logger;
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
        private readonly string queryStr2 = "SELECT  SUM(`cost`) AS sum, `date` FROM jizhang WHERE DATE(`date`) > DATE(CURRENT_DATE, '-10 days') group by `date`";
        //当月分类统计
        private readonly string queryStr1 = "SELECT  SUM(`cost`) AS sum, `class` FROM jizhang WHERE strftime('%m', `date`) = strftime('%m', DATE()) GROUP BY `class`";

        //数据库接口
        private IConfigService config;
        private string connectStr;// = $"DataSource=sqlite.db;Version=3;";
        private static SQLiteConnection conn;

        //创建数据库
        private readonly string createDb = "CREATE TABLE if not exists \"jizhang\" (" +
              "\"ID\" INTEGER PRIMARY KEY AUTOINCREMENT," +
              "\"date\" varchar(10) NOT NULL," +
              "\"goods\" varchar(32) NOT NULL," +
              "\"cost\" float(5, 1) NOT NULL," +
              "\"class\" varchar(16) NOT NULL," +
              "\"comments\" varchar(128) DEFAULT NULL); ";
        private string queryStrAll = "select * from `jizhang` where strftime('%m',`date`) = strftime('%m',DATE()) order by `Id` desc";

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
            conn = new SQLiteConnection(connectStr);
            //新建数据库表
            SQLiteCommand cdb = new SQLiteCommand(createDb, conn);
            //查询当月总数据
            SQLiteCommand showAll = new SQLiteCommand(queryStrAll, conn);
            //新建查询结果变量
            SQLiteDataReader reader;
            try
            {
                //打开连接
                conn.Open();
                logger.LogInformation($"数据库{config.Config.Setting.Sqlite}连接成功！");
                cdb.ExecuteNonQuery();  //如果表不存在则创建表
                reader = showAll.ExecuteReader();   //查询结果的第一行
                DailyCosts.Clear();    //先清空缓存数据
                logger.LogInformation("开始查询本月所有数据...");
                while (reader.Read())   //执行一次返回一条记录（一行数据），再次执行返回下一条，直到最后无返回值
                {
                    DailyCost result = new DailyCost();
                    result.Date = reader["date"].ToString();            //"2021-07-08"
                    result.Comments = reader["comments"].ToString();    //"测试"
                    result.Goods = reader["goods"].ToString();          //"goods"
                    result.Cost = reader.GetFloat(3);                   //3表示列column, 0.2F
                    result.Type = reader[4].ToString();                 //"类型"
                    DailyCosts.Add(result);                             //修改了属性，触发事件PropertyChanged，系统自动调用                    
                }
                logger.LogInformation("本月所有数据查询成功！");
            }
            catch (SQLiteException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
                logger.LogError(ex, $"查询本月所有数据失败！");
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
            SQLiteCommand cmd = new SQLiteCommand(queryStr, conn);

            ChartValues<double> values = new ChartValues<double>();
            try
            {
                conn.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                
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
                logger.LogInformation($"查询图表{seriesType}数据成功！");
            }
            catch (SQLiteException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
                logger.LogError(ex,$"查询图表{series.ToString()}数据失败！");
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

            SQLiteCommand insert = new SQLiteCommand(insertString, conn);
            try
            {
                //排除空值，" "
                for (int i = 0; i < 4; i++)
                {
                    if (values[i].Equals(null) || values[i].Equals("")) throw new ArgumentNullException(nameof(values));
                }
                conn.Open();
                insert.ExecuteNonQuery();   //插入数据库                
                logger.LogInformation($"插入数据成功");
            }
            catch (SQLiteException ex)
            {
                System.Windows.MessageBox.Show($"Database Error!\n{ex}", "错误");
                logger.LogError(ex, $"插入数据失败，查询数据库出错！");
                return;
            }
            catch (ArgumentNullException ex)
            {
                System.Windows.MessageBox.Show($"数据不能为空!\n{ex}");
                logger.LogError(ex, $"插入数据失败，数据不能为空！");
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

        public SqliteIDataService(IConfigService config, ILogger<SqliteIDataService> _logger)
        {
            this.config = config;
            this.logger = _logger;
            connectStr = $"DataSource={this.config.Config.Setting.Sqlite};Version=3;";
            ShowData();
            InsertCommand = new RelayCommand<object[]>(t => InsertData(t));
        }
    }
}
