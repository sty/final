using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Object;
using System.Globalization;


namespace Calculator.CheckBook
{
    class ColorConverter: IValueConverter
    {
        public Object Convert (Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            
            var Amount = Transaction.Amount Where ;
            Amount = (double)value;
           
            double compareToValue = (double)parameter;
            return Amount < compareToValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
