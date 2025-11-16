using CSharpMobileApp.Services;
using CSharpMobileApp.ViewModels;

namespace CSharpMobileApp.Views;

public partial class ToDoListPage : ContentPage
{
    public ToDoListPage(TaskService taskService)
    {
        InitializeComponent();
        BindingContext = new ToDoListViewModel(taskService);
    }
}