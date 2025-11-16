using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using CSharpMobileApp.Models;

namespace CSharpMobileApp.Services
{
    public class TaskService
    {
        private ObservableCollection<TodoTask> _tasks = new ObservableCollection<TodoTask>();
        private int _nextId = 1;

        public TaskService()
        {
            LoadTasks();
        }

        public ObservableCollection<TodoTask> GetTasks()
        {
            return _tasks;
        }

        public void AddTask(string text)
        {
            var newTask = new TodoTask
            {
                Id = _nextId++,
                Text = text,
                Completed = false
            };

            _tasks.Add(newTask);
            SortTasks();
            SaveTasks();
        }

        public void DeleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                SortTasks();
                SaveTasks();
            }
        }

        public void ToggleCompleted(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Completed = !task.Completed;
                UpdateTasksState();
            }
        }

        public void EditTask(int id, string newText)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Text = newText;
                SaveTasks();
            }
        }

        // Удалить все выполненные задачи
        public void DeleteCompleted()
        {
            var completed = _tasks.Where(t => t.Completed).ToList();
            if (completed.Count == 0)
                return;

            foreach (var task in completed)
            {
                _tasks.Remove(task);
            }

            SortTasks();
            SaveTasks();
        }

        // Вызывается, когда задача изменилась (например, Completed переключился)
        public void UpdateTasksState()
        {
            SortTasks();
            SaveTasks();
        }

        private void SaveTasks()
        {
            var tasksJson = JsonSerializer.Serialize(_tasks);
            Preferences.Set("tasks", tasksJson);
        }

        private void LoadTasks()
        {
            var tasksJson = Preferences.Get("tasks", string.Empty);
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var list = JsonSerializer.Deserialize<List<TodoTask>>(tasksJson) ?? new List<TodoTask>();

                _tasks = new ObservableCollection<TodoTask>(list);

                if (_tasks.Any())
                {
                    _nextId = _tasks.Max(t => t.Id) + 1;
                }

                SortTasks();
            }
        }

        // Невыполненные (Completed == false) сверху, выполненные снизу
        private void SortTasks()
        {
            if (_tasks == null || _tasks.Count == 0)
                return;

            var sorted = _tasks
                .OrderBy(t => t.Completed)   // false (0) → true (1)
                .ThenBy(t => t.Id)
                .ToList();

            _tasks.Clear();
            foreach (var task in sorted)
            {
                _tasks.Add(task);
            }
        }
    }
}