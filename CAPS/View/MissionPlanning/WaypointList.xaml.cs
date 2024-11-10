using CAPS.ViewModel.WaypointList;
using System.Windows.Controls;

namespace CAPS.View.MissionPlanning
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
