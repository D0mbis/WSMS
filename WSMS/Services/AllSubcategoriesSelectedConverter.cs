using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WSMS.Models;

namespace WSMS.Services
{
    public class AllSubcategoriesSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return false;

            var subcategories = values[0] as ObservableCollection<SubCategory>;
            if (subcategories == null)
                return false;

            return subcategories.All(c => c.IsChecked);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
