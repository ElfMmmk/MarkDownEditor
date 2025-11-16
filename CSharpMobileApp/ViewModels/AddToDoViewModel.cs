using CSharpMobileApp.Services;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    public class AddToDoViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private string _taskText;

        public string TaskText
        {
            get => _taskText;
            set
            {
                _taskText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskText)));
            }
        }

        public ICommand AddTaskCommand { get; }

        public AddToDoViewModel(TaskService taskService)
        {
            _taskService = taskService;
            AddTaskCommand = new Command(AddTask);
        }

        private void AddTask()
        {
            if (!string.IsNullOrWhiteSpace(TaskText))
            {
                _taskService.AddTask(TaskText);
                Shell.Current.GoToAsync("..");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}