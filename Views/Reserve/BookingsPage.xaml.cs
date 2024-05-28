using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class BookingsPage : ContentPage
{
    public BookingsPage(BookingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}