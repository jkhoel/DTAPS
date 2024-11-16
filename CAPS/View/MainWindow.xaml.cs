using CAPS.ViewModel;
using System.Windows;

namespace CAPS.View;

public partial class MainWindow : Window
{
	public MainWindow(MainWindowViewModel vm)
	{
		InitializeComponent();
		DataContext = vm;
	}
}