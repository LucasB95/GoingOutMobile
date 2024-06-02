using GoingOutMobile.ViewModels.Reserve;

namespace GoingOutMobile.Views;

public partial class LastVisitedPage : ContentPage
{
	public LastVisitedPage(LastVisitedViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}