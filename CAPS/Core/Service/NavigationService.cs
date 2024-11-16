using CommunityToolkit.Mvvm.ComponentModel;

namespace CAPS.Core.Service;

public interface INavigationService
{
	ViewModelBase CurrentView { get; }
	void NavigateTo<T>() where T : ViewModelBase;
}

public partial class NavigationService : ObservableObject, INavigationService
{
	[ObservableProperty]
	private ViewModelBase currentView;
	private Func<Type, ViewModelBase> _viewModelFactory;

	public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
	{
		CurrentView = _viewModelFactory.Invoke(typeof(TViewModel));
	}
}
