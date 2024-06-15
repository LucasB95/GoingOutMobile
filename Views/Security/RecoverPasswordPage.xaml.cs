using GoingOutMobile.ViewModels.Security;

namespace GoingOutMobile.Views.Security;

public partial class RecoverPasswordPage : ContentPage
{
	public RecoverPasswordPage(RecoverPasswordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}