using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class RestaurantDetailPage : ContentPage
{
    public RestaurantDetailPage(RestaurantDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void HandleScrollViewFocused(object sender, FocusEventArgs e)
    {
        // Evitar que el ScrollView reciba el enfoque
        if (sender is ScrollView scrollView)
        {
            scrollView.Unfocus();
        }
    }

}