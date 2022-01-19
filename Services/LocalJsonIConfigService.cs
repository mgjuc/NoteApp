using GalaSoft.MvvmLight;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using 记账App.Model;

namespace 记账App.Services
{
    public class LocalJsonIConfigService : ViewModelBase, IConfigService
    {
        private string fileName = "config.json";
        private ILogger<LocalJsonIConfigService> logger;
        private ConfigurationBuilder configBuilder = new ConfigurationBuilder();

        private Config config;
        public Config Config
        {
            get => config;
            set { config = value; RaisePropertyChanged(); }
        }

        //XMAL 数据绑定只能绑定属性，并且是有{get;}的属性，绑定 ItemsSource；用ObservableCollection才能通知UI动态更新！
        private ObservableCollection<Types> list = new ObservableCollection<Types>();
        public ObservableCollection<Types> List
        {
            get => list;
            set { list = value; RaisePropertyChanged(); }
        }

        public LocalJsonIConfigService(ILogger<LocalJsonIConfigService> _logger)    //构造函数注入
        {
            ////服务容器
            //ServiceCollection services = new ServiceCollection();   //创建服务容器
            //services.AddScoped<IDataService, SqliteIDataService>(); //容器里添加服务方法<服务接口，实现服务接口的方法>
            //using (var sp = services.BuildServiceProvider())
            //{
            //    var getData = sp.GetRequiredService<IDataService>();    //从容器提取想要的接口服务
            //    getData.GetDailyCost();                             //运行服务
            //}
            this.logger = _logger;
            GetConfig();
            GetComboItems();
        }
        public void GetConfig()
        {
            configBuilder.AddJsonFile("config.json", optional: false, reloadOnChange: true);
            logger.LogInformation($"打开配置文件{fileName}");
            IConfigurationRoot configRoot = configBuilder.Build();

            //==> 直接访问元素
            //config.Version = configRoot["version"];
            //config.Setting.Type = configRoot.GetSection("setting:type").Value;

            //GetSection().Get<T> 结果解析到泛型类，需添加Mincosoft.Extensions.Configuration.Binder包
            Config = configRoot.Get<Config>();  //直接绑定根目录，按类解析
            logger.LogInformation($"读取配置信息到{Config.ToString()}");
            //config.Version = "返写JSON测试"; ==》只是改变了查询后的结果，没有更新源JSON文件
        }
        public void GetComboItems()
        {
            //生成list绑定到ComboBox的itemSource 
            List.Clear();
            if (List.Count == 0)
            {
                for (int i = 0; i < Config.Setting.Type.Length; i++)
                {
                    List.Add(new Types { ID = i + 1, Value = Config.Setting.Type[i] });
                }
            }
            logger.LogInformation("获取ComboItems列表成功");
        }

        public void UpdateComboItems(string[] types)
        {
            //更新配置文件
            var option = new JsonSerializerOptions { WriteIndented = true };
            Config.Setting.Type = types;
            string jsonString = JsonSerializer.Serialize<Config>(Config, option);
            File.WriteAllText(fileName, jsonString);
            logger.LogInformation($"更新ComboItems列表，写入到{fileName}");
            //更新List刷新UI
            GetComboItems();
        }
    }
}
