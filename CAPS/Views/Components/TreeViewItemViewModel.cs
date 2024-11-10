using CAPS.Core;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CAPS.Views.Components;

public class TreeViewItemViewModel : ViewModel	
{
	public string Header { get; set; } = string.Empty;
	public ObservableCollection<TreeViewItemViewModel> Children { get; set; } = new ObservableCollection<TreeViewItemViewModel>();
	public IRelayCommand OnSelectCommand { get; set; }

	public TreeViewItemViewModel(string header, IEnumerable<TreeViewItemViewModel>? children = null, Action<TreeViewItemViewModel>? onItemSelection = null)
	{
		Header = header;
		Children = new ObservableCollection<TreeViewItemViewModel>(children ?? Enumerable.Empty<TreeViewItemViewModel>());
		OnSelectCommand = new RelayCommand<TreeViewItemViewModel>(OnSelected);

		if (onItemSelection != null)
		{
			OnSelectCommand = new RelayCommand<TreeViewItemViewModel>(param => onItemSelection(this));
		}
	}

	private void OnSelected(TreeViewItemViewModel? model)
	{
		System.Diagnostics.Debug.WriteLine($"Selected: {Header}");
	}
}