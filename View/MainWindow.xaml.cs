using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using 记账App.Model;
using 记账App.ViewModel;
using 记账App.View;
using LiveCharts;
using LiveCharts.Wpf;
using System.Text.RegularExpressions;

namespace 记账App.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Config ConfigController = new Config();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            //this.Left = 0.0;
            //this.Top = 0.0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        //private void showMegBox(object sender, RoutedEventArgs e)
        //{
        //    //源文件Config.json UTF-8编码保存，省去转码麻烦
        //    //byte[] buffer = Encoding.GetEncoding("GB18030").GetBytes(ConfigController.config.Setting.Type[0]);
        //    //byte[] buffers = Encoding.Convert(Encoding.GetEncoding("GB18030"), Encoding.UTF8, buffer);
        //    //MessageBox.Show("版本："+ConfigController.config.Version+"\n变量："+ Encoding.UTF8.GetString(buffers), "弹窗标题");
        //    //MessageBox.Show("版本：" + ConfigController.config.Version + "\n说明：" + "支持Sqlite运行，免去数据库配置", "帮助");
        //    ConfigDialog configDialog = new ConfigDialog();
        //    configDialog.ShowDialog();
        //}

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //拖动效果dragMove()
            base.DragMove();
        }

        private void cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //添加正则表达式，限制输入字符
            //Regex re = new Regex("(([1-9]{1}\\d*)|([0]{1}))(\\.(\\d){0,1})?$");
            Regex re = new Regex("[^0-9\\.]+");
            //e.Handled = true ==》事件已处理，直接结果，会清空输入
            e.Handled = re.IsMatch(e.Text);
            //System.Windows.MessageBox.Show(e.Handled.ToString());
        }
    }
}
