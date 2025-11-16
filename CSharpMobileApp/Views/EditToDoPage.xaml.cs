using CSharpMobileApp.Services;
using CSharpMobileApp.ViewModels;

namespace CSharpMobileApp.Views;

public partial class EditToDoPage : ContentPage
{
    public EditToDoPage(TaskService taskService)
    {
        InitializeComponent();
        BindingContext = new EditToDoViewModel(taskService);
    }
}