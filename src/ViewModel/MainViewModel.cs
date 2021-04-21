using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace TestNETFCanvas.ViewModel
{
	public class MyItem : ObservableObject
	{
		private int _X;
		public int X { get => _X; set => Set(ref _X, value); }

		private int _Y;
		public int Y { get => _Y; set => Set(ref _Y, value); }

		public MyItem(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </para>
	/// <para>
	/// You can also use Blend to data bind with the tool's support.
	/// </para>
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
    {
		private List<MyItem> _CanvasItems;
		public List<MyItem> CanvasItems { get => _CanvasItems; set => Set(ref _CanvasItems, value); }

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
        {
            CanvasItems = new List<MyItem>() { new MyItem(100,100), new MyItem(300, 300) };
        }


    }
}