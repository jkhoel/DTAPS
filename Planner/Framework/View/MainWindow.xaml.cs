using Planner.Framework.ViewModel;
using System.Windows;

namespace Planner.Framework.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(MainWindowViewModel vm)
		{
			InitializeComponent();
			DataContext = vm;
		}
	}
}