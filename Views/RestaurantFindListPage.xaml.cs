using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class RestaurantFindListPage : ContentPage
{
	public RestaurantFindListPage(RestaurantFindListViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}