using CSharpMobileApp.Models;
using CSharpMobileApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        public ObservableCollection<Task> Tasks { get; set; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }

        public ToDoListViewModel(TaskService taskService)
        {
            _taskService = taskService;
            Tasks = new ObservableCollection<Task>(_taskService.GetTasks());
            AddTaskCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
            DeleteTaskCommand = new Command<Task>(DeleteTask);
            EditTaskCommand = new Command<Task>(GoToEditTask);
        }

        private void DeleteTask(Task task)
        {
            if (task != null)
            {
                _taskService.DeleteTask(task.Id);
                Tasks.Remove(task);
            }
        }

        private async void GoToEditTask(Task task)
        {
            if (task != null)
            {
                await Shell.Current.GoToAsync($"EditToDoPage?Task={task.Id}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}