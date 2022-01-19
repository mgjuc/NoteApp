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
using System.Windows.Shapes;
using 记账App.Model;
using System.IO;
using 记账App.Services;
using 记账App.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace 记账App.View
{
    /// <summary>
    /// configEdit.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigDialog : Window
    {
        public ConfigDialog(ConfigDialogViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CloseView = new Action(this.Close);
            this.DataContext = viewModel;
        }
    }
}
