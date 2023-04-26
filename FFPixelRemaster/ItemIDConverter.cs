using System;
using System.Globalization;
using System.Windows.Data;

namespace FFPixelRemaster
{
	internal class ItemIDConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var ID = (uint)value;
			return Info.Instance().Search(Info.Instance().Item, ID)?.Name ?? value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
