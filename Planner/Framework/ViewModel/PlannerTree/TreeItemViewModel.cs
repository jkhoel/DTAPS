﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planner.Core.Class;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Planner.Framework.ViewModel.PlannerTree;

public partial class TreeItemViewModel : ViewModelBase
{
    public string Header { get; set; } = string.Empty;

    public int GroupIndex { get; set; }

    public ContextMenu? ContextMenu { get; set; }

    [ObservableProperty]
    private bool isExpanded;

    public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();
    public IRelayCommand OnSelectCommand { get; set; }

    public TreeItemViewModel(int groupIndex, string header, IEnumerable<TreeItemViewModel>? children = null, Action<TreeItemViewModel>? onItemSelection = null, ContextMenu? contextMenu = null)
    {
        GroupIndex = groupIndex;
        Header = header;
        Children = new ObservableCollection<TreeItemViewModel>(children ?? Enumerable.Empty<TreeItemViewModel>());
        OnSelectCommand = new RelayCommand<TreeItemViewModel>(OnSelected);
        ContextMenu = contextMenu;

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
