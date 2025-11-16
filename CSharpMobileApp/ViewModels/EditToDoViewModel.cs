using CSharpMobileApp.Models;
using CSharpMobileApp.Services;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    [QueryProperty(nameof(TaskToEdit), "Task")]
    public class EditToDoViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private Task _taskToEdit;
        private string _taskText;

        public Task TaskToEdit
        {
            get => _taskToEdit;
            set
            {
                _taskToEdit = value;
                TaskText = value?.Text;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskToEdit)));
            }
        }

        public string TaskText
        {
            get => _taskText;
            set
            {
                _taskText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskText)));
            }
        }

        public ICommand EditTaskCommand { get; }

        public EditToDoViewModel(TaskService taskService)
        {
            _taskService = taskService;
            EditTaskCommand = new Command(EditTask);
        }

        private void EditTask()
        {
            if (TaskToEdit != null && !string.IsNullOrWhiteSpace(TaskText))
            {
                _taskService.EditTask(TaskToEdit.Id, TaskText);
                Shell.Current.GoToAsync("..");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}