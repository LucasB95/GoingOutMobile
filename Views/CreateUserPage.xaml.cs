using GoingOutMobile.ViewModels;

namespace GoingOutMobile.Views;

public partial class CreateUserPage : ContentPage
{
    public CreateUserPage(CreateUserViewModel createUserViewModel)
    {
        InitializeComponent();
        BindingContext = createUserViewModel;
    }
}