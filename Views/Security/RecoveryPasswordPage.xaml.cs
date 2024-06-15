using GoingOutMobile.ViewModels.Security;

namespace GoingOutMobile.Views.Security;

public partial class RecoveryPasswordPage : ContentPage
{
	public RecoveryPasswordPage(RecoveryPasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}