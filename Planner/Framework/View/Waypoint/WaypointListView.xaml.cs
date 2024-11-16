using Planner.Framework.ViewModel.Waypoint;
using System.Windows.Controls;

namespace Planner.Framework.View.Waypoint
{
	/// <summary>
	/// Interaction logic for WaypointListView.xaml
	/// </summary>
	public partial class WaypointListView : UserControl
	{
		public WaypointListView()
		{
			InitializeComponent();
			DataContext = App.GetService<WaypointListViewModel>();
		}
	}
}
