using CommunityToolkit.Mvvm.Input;
using Planner.Core.Class;
using System.Collections.ObjectModel;

namespace Planner.Framework.ViewModel.PlannerTree;

public class TreeItemViewModel : ViewModelBase
{
    public string Header { get; set; } = string.Empty;
    public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
    public IRelayCommand OnSelectCommand { get; set; }

    public TreeItemViewModel(string header, IEnumerable<TreeItemViewModel>? children = null, Action<TreeItemViewModel>? onItemSelection = null)
    {
        Header = header;
        Children = new ObservableCollection<TreeItemViewModel>(children ?? Enumerable.Empty<TreeItemViewModel>());
        OnSelectCommand = new RelayCommand<TreeItemViewModel>(OnSelected);

        if (onItemSelection != null)
        {
            OnSelectCommand = new RelayCommand<TreeItemViewModel>(param => onItemSelection(this));
        }
    }

    private void OnSelected(TreeItemViewModel? model)
    {
        System.Diagnostics.Debug.WriteLine($"Selected: {Header}");
    }
}
