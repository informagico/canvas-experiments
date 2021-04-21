using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace TestNETFCanvas
{
	public class MouseButtonEventArgsToPointConverter : IEventArgsConverter
	{
		public object Convert(object value, object parameter)
		{
			var args = (MouseButtonEventArgs)value;
			var element = (FrameworkElement)parameter;

			var point = args.GetPosition(element);
			return point;
		}
	}

	public class MouseEventArgsToPointConverter : IEventArgsConverter
	{
		public object Convert(object value, object parameter)
		{
			var args = (MouseEventArgs)value;
			var element = (FrameworkElement)parameter;

			var point = args.GetPosition(element);
			return point;
		}
	}
}
