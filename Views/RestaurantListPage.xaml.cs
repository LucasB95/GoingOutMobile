using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class RestaurantListPage : ContentPage
{
	public RestaurantListPage(RestaurantListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}