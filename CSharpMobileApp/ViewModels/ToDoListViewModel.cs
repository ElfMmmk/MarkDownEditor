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
        public ObservableCollection<TodoTask> Tasks { get; set; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }

        public ToDoListViewModel(TaskService taskService)
        {
            _taskService = taskService;
            Tasks = new ObservableCollection<TodoTask>(_taskService.GetTasks());
            AddTaskCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
            DeleteTaskCommand = new Command<TodoTask>(DeleteTask);
            EditTaskCommand = new Command<TodoTask>(GoToEditTask);
        }

        private void DeleteTask(TodoTask task)
        {
            if (task != null)
            {
                _taskService.DeleteTask(task.Id);
                Tasks.Remove(task);
            }
        }

        private async void GoToEditTask(TodoTask task)
        {
            if (task != null)
            {
                await Shell.Current.GoToAsync($"EditToDoPage?TodoTask={task.Id}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}