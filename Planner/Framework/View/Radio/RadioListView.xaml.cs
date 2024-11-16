using Planner.Framework.ViewModel.Radio;
using Planner.Framework.ViewModel.Waypoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Planner.Framework.View.Radio
{
	/// <summary>
	/// Interaction logic for RadioListView.xaml
	/// </summary>
	public partial class RadioListView : UserControl
	{
		public RadioListView()
		{
			InitializeComponent();
			DataContext = App.GetService<RadioListViewModel>();
		}
	}
}
