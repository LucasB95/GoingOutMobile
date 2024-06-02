using GoingOutMobile.ViewModels.Reserve;

namespace GoingOutMobile.Views.LastVisited;

public partial class LastVisitedDetailPage : ContentPage
{
	public LastVisitedDetailPage(LastVisitedDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}