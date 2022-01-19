using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 记账App.Services;

namespace 记账App.ViewModel
{
    public class ConfigDialogViewModel : ViewModelBase
    {
        public Action CloseView;
        private string typeString;
        public string TypeString { get => typeString; set { typeString = value; RaisePropertyChanged(); } }

        public ConfigDialogViewModel(string[] types)
        {
            TypeString = string.Join(",", types);
            UpdateTypes = new RelayCommand(Update);
        }
        public RelayCommand UpdateTypes { get; set; }
        public void Update()
        {
            CloseView();
            Messenger.Default.Send<string[]>(TypeString.Split(","), "updateTypes");
        }


    }
}
