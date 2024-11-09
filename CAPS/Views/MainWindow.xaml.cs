using CAPS.Views;
using System.Windows;

namespace CAPS;

public partial class MainWindow : Window
{
	public MainWindow(MainWindowViewModel vm)
	{
		InitializeComponent();
		DataContext = vm;
	}
}