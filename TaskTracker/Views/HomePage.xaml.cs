using CSharpMobileApp.ViewModels;

namespace CSharpMobileApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        BindingContext = new HomeViewModel();
    }
}