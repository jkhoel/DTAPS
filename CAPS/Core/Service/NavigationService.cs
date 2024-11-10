using CommunityToolkit.Mvvm.ComponentModel;

namespace CAPS.Core.Service;

public interface INavigationService
{
	ViewModel CurrentView { get; }
	void NavigateTo<T>() where T : ViewModel;
}

public partial class NavigationService : ObservableObject, INavigationService
{
	[ObservableProperty]
	private ViewModel currentView;
	private Func<Type, ViewModel> _viewModelFactory;

	public NavigationService(Func<Type, ViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModel
	{
		CurrentView = _viewModelFactory.Invoke(typeof(TViewModel));
	}
}
