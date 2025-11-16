using CSharpMobileApp.Services;
using CSharpMobileApp.ViewModels;

namespace CSharpMobileApp.Views;

public partial class AddToDoPage : ContentPage
{
    public AddToDoPage(TaskService taskService)
    {
        InitializeComponent();
        BindingContext = new AddToDoViewModel(taskService);
    }
}