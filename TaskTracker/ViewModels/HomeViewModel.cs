using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    public class HomeViewModel
    {
        public ICommand GoToToDoListCommand { get; }
        public ICommand GoToAddToDoCommand { get; }

        public HomeViewModel()
        {
            GoToToDoListCommand = new Command(async () => await Shell.Current.GoToAsync("ToDoListPage"));
            GoToAddToDoCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
        }
    }
}