//#define MYSQL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using 记账App.Model;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using 记账App.Converters;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections;
using System.Data.SQLite;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using 记账App.Services;
using Microsoft.Extensions.DependencyInjection;
using 记账App.View;
using NLog.Extensions.Logging;

namespace 记账App.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ServiceCollection services = new ServiceCollection();   //创建服务容器
        public MainWindowViewModel()
        {
            //容器里添加服务方法<服务接口，实现服务接口的方法>，链式函数，一直点下去添加新服务
            services.AddScoped<IDataService, MysqlIDataService>()   //添加mysql服务
                .AddScoped<IDataService, SqliteIDataService>()      //添加sqlite服务
                .AddScoped<IConfigService, LocalJsonIConfigService>()   //添加JsonConfig服务
                .AddLogging(build => build.AddNLog()  //添加Nlog输出通道
                                        .SetMinimumLevel(LogLevel.Debug));
            using (var sp = services.BuildServiceProvider())
            {
                DataService = sp.GetRequiredService<IDataService>();    //从容器提取想要的接口服务
                ConfigService = sp.GetRequiredService<IConfigService>();
                //UpdateComboItems.UpdateComboItems();                             //运行服务
            }
            //ConfigService = new LocalJsonIConfigService();
            //DataService = new SqliteIDataService();
            //注册消息接收器，触发更新 消费类ItemsSources
            Messenger.Default.Register<string[]>(this, "updateTypes", ConfigService.UpdateComboItems);
            OpenDialog = new RelayCommand(openDialog);
            
        }
        public IConfigService ConfigService { get; set; }   //定义的是接口变量，使用服务容器提取方式生成实现，不管具体是用什么方法实现的。
        public IDataService DataService { get; set; }
        public RelayCommand OpenDialog { get; set; }
        public void openDialog()
        {
            ConfigDialogViewModel configDialogViewModel = new ConfigDialogViewModel(ConfigService.Config.Setting.Type);
            ConfigDialog configDialog = new ConfigDialog(configDialogViewModel);
            configDialog.ShowDialog();
        }
    }
}
