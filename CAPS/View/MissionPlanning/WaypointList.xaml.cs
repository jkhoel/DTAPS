using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CAPS.ViewModel.WaypointList
{
	/// <summary>
	/// Interaction logic for WaypointList.xaml
	/// </summary>
	public partial class WaypointList : UserControl
	{
		public WaypointList()
		{
			InitializeComponent();

			DataContext = App.GetService<WaypointListViewModel>();
		}
	}
}
