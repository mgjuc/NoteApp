using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 记账App.Converters
{
    public class InsertConverters : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            //2021年7月30日
            //string dt = TransStrToDateFormatStr(values[0].ToString());
            //string dt = string.Format("{0:yyyy-MM-dd}", "2021/7/29");
            //values.SetValue(dt, 0);
            return values.Clone();
        }

        public static string TransStrToDateFormatStr(string strDateTime)
        {
            DateTime dt;
            string[] format = { "yyyy年M月dd号" };
            IFormatProvider culture = new CultureInfo("zh-CN", true);
            //DateTime.TryParseExact() 编译生成日期函数
            if (DateTime.TryParseExact(strDateTime, format, culture, DateTimeStyles.AllowInnerWhite, out dt))
            {
                return dt.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
