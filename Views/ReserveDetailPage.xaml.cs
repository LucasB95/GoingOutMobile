using GoingOutMobile.ViewModels;
using System.ComponentModel;

namespace GoingOutMobile.Views;

public partial class ReserveDetailPage : ContentPage
{
    public ReserveDetailPage(ReserveDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

}