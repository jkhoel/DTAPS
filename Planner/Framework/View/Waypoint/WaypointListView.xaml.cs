using Planner.Framework.ViewModel.Waypoint;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
