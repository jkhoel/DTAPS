using Planner.Framework.ViewModel.MissionSettings;
using System.Windows.Controls;

namespace Planner.Framework.View.MissionSettings
{
	/// <summary>
	/// Interaction logic for MissionSettingsView.xaml
	/// </summary>
	public partial class MissionSettingsView : UserControl
	{
		public MissionSettingsView()
		{
			InitializeComponent();
			DataContext = App.GetService<MissionSettingsViewModel>();
		}
	}
}
