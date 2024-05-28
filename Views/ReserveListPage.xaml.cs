using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class ReserveListPage : ContentPage
{
	public ReserveListPage(ReserveListViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}