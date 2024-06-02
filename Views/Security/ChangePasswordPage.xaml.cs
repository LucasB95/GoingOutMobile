using GoingOutMobile.ViewModels.Security;

namespace GoingOutMobile.Views.Security;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage(ChangePasswordViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}