using CSharpMobileApp.Models;
using CSharpMobileApp.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        public ObservableCollection<TodoTask> Tasks { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }

        public ToDoListViewModel(TaskService taskService)
        {
            _taskService = taskService;
            Tasks = _taskService.GetTasks();

            AddTaskCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
            DeleteTaskCommand = new Command<TodoTask>(DeleteTask);
            EditTaskCommand = new Command<TodoTask>(GoToEditTask);
        }

        private void DeleteTask(TodoTask task)
        {
            if (task != null)
            {
                _taskService.DeleteTask(task.Id);
            }
        }


        private async void GoToEditTask(TodoTask task)
        {
            if (task == null)
                return;

            await Shell.Current.GoToAsync("EditToDoPage", new Dictionary<string, object>
    {
        { "TaskToEdit", task }
    });
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}