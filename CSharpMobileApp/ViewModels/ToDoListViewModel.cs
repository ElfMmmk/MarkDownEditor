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

        // Полный список задач из сервиса
        public ObservableCollection<TodoTask> Tasks { get; }

        // Отфильтрованный список для отображения
        public ObservableCollection<TodoTask> FilteredTasks { get; } = new();

        private int _totalCount;
        private int _completedCount;
        private int _remainingCount;

        private TaskFilter _currentFilter = TaskFilter.All;

        private enum TaskFilter
        {
            All,
            Active,
            Completed
        }

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
        public ICommand DeleteCompletedCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand ShowActiveCommand { get; }
        public ICommand ShowCompletedCommand { get; }

        // Состояния для RadioButton'ов
        public bool IsAllFilterSelected
        {
            get => _currentFilter == TaskFilter.All;
            set
            {
                if (value)
                    SetFilter(TaskFilter.All);
            }
        }

        public bool IsActiveFilterSelected
        {
            get => _currentFilter == TaskFilter.Active;
            set
            {
                if (value)
                    SetFilter(TaskFilter.Active);
            }
        }

        public bool IsCompletedFilterSelected
        {
            get => _currentFilter == TaskFilter.Completed;
            set
            {
                if (value)
                    SetFilter(TaskFilter.Completed);
            }
        }

        public ToDoListViewModel(TaskService taskService)
        {
            _taskService = taskService;

            // Общая коллекция задач из сервиса
            Tasks = _taskService.GetTasks();

            // Подписываемся на изменения коллекции
            Tasks.CollectionChanged += OnTasksCollectionChanged;

            // Подписываемся на изменения уже существующих задач
            foreach (var task in Tasks)
            {
                task.PropertyChanged += OnTaskPropertyChanged;
            }

            AddTaskCommand = new Command(async () => await Shell.Current.GoToAsync("AddToDoPage"));
            DeleteTaskCommand = new Command<TodoTask>(DeleteTask);
            EditTaskCommand = new Command<TodoTask>(GoToEditTask);
            DeleteCompletedCommand = new Command(DeleteCompleted);

            // Команды фильтра (можно не использовать в XAML)
            ShowAllCommand = new Command(() => SetFilter(TaskFilter.All));
            ShowActiveCommand = new Command(() => SetFilter(TaskFilter.Active));
            ShowCompletedCommand = new Command(() => SetFilter(TaskFilter.Completed));

            UpdateCounts();
            ApplyFilter();

            // Обновим биндинги RadioButton'ов
            OnPropertyChanged(nameof(IsAllFilterSelected));
            OnPropertyChanged(nameof(IsActiveFilterSelected));
            OnPropertyChanged(nameof(IsCompletedFilterSelected));
        }

        private void SetFilter(TaskFilter filter)
        {
            if (_currentFilter == filter)
                return;

            _currentFilter = filter;

            // Обновляем RadioButton'ы
            OnPropertyChanged(nameof(IsAllFilterSelected));
            OnPropertyChanged(nameof(IsActiveFilterSelected));
            OnPropertyChanged(nameof(IsCompletedFilterSelected));

            ApplyFilter();
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
            ApplyFilter();
        }

        private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoTask.Completed))
            {
                // Обновляем счётчики и даём сервису пересортировать и сохранить
                UpdateCounts();
                _taskService.UpdateTasksState();
                ApplyFilter();
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

        private void ApplyFilter()
        {
            if (Tasks == null)
                return;

            IEnumerable<TodoTask> source = Tasks;

            switch (_currentFilter)
            {
                case TaskFilter.Active:
                    source = source.Where(t => !t.Completed);
                    break;
                case TaskFilter.Completed:
                    source = source.Where(t => t.Completed);
                    break;
                case TaskFilter.All:
                default:
                    break;
            }

            FilteredTasks.Clear();
            foreach (var task in source)
            {
                FilteredTasks.Add(task);
            }
        }

        private async void DeleteTask(TodoTask task)
        {
            if (task == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Удалить задачу",
                $"Вы точно хотите удалить задачу:\n\"{task.Text}\"?",
                "Удалить",
                "Отмена");

            if (!confirm)
                return;

            _taskService.DeleteTask(task.Id);
            // Коллекция и фильтр обновятся через события
        }

        private async void DeleteCompleted()
        {
            if (!Tasks.Any(t => t.Completed))
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Удалить выполненные задачи",
                "Вы уверены, что хотите удалить все выполненные задачи?",
                "Удалить",
                "Отмена");

            if (!confirm)
                return;

            _taskService.DeleteCompleted();
            // Коллекция и фильтр обновятся через события
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
