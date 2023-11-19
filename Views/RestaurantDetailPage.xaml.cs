using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class RestaurantDetailPage : ContentPage
{
	public RestaurantDetailPage(RestaurantDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}