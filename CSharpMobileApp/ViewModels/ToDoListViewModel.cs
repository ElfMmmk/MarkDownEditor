using CSharpMobileApp.Models;
using CSharpMobileApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CSharpMobileApp.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;

        public ObservableCollection<TodoTask> Tasks { get; }

        private int _totalCount;
        private int _completedCount;
        private int _remainingCount;

        public int TotalCount
        {
            get => _totalCount;
            private set
            {
                if (_totalCount == value) return;
                _totalCount = value;
                OnPropertyChanged(nameof(TotalCount));
                OnPropertyChanged(nameof(TaskSummary));
            }
        }

        public int CompletedCount
        {
            get => _completedCount;
            private set
            {
                if (_completedCount == value) return;
                _completedCount = value;
                OnPropertyChanged(nameof(CompletedCount));
                OnPropertyChanged(nameof(TaskSummary));
            }
        }

        public int RemainingCount
        {
            get => _remainingCount;
            private set
            {
                if (_remainingCount == value) return;
                _remainingCount = value;
                OnPropertyChanged(nameof(RemainingCount));
                OnPropertyChanged(nameof(TaskSummary));
            }
        }

        public string TaskSummary =>
            $"Всего: {TotalCount} • Выполнено: {CompletedCount} • Осталось: {RemainingCount}";

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }

        public ToDoListViewModel(TaskService taskService)
        {
            _taskService = taskService;

            Tasks = _taskService.GetTasks();

            Tasks.CollectionChanged += OnTasksCollectionChanged;

            foreach (var task in Tasks)
            {
                task.PropertyChanged += OnTaskPropertyChanged;
            }

            AddTaskCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
            DeleteTaskCommand = new Command<TodoTask>(DeleteTask);
            EditTaskCommand = new Command<TodoTask>(GoToEditTask);
            UpdateCounts();
        }

        private void OnTasksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TodoTask task in e.NewItems)
                {
                    task.PropertyChanged += OnTaskPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (TodoTask task in e.OldItems)
                {
                    task.PropertyChanged -= OnTaskPropertyChanged;
                }
            }

            UpdateCounts();
        }

        private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoTask.Completed))
            {
                UpdateCounts();
            }
        }

        private void UpdateCounts()
        {
            var total = Tasks?.Count ?? 0;
            var completed = Tasks?.Count(t => t.Completed) ?? 0;
            var remaining = total - completed;

            TotalCount = total;
            CompletedCount = completed;
            RemainingCount = remaining;
        }

        private void DeleteTask(TodoTask task)
        {
            if (task == null)
                return;

            _taskService.DeleteTask(task.Id);
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
