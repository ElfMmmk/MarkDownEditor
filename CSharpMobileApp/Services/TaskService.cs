using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using CSharpMobileApp.Models;


namespace CSharpMobileApp.Services
{
    public class TaskService
    {
        private List<TodoTask> _tasks = new List<TodoTask>();
        private int _nextId = 1;

        public TaskService()
        {
            LoadTasks();
        }

        public List<TodoTask> GetTasks()
        {
            return _tasks;
        }

        public void AddTask(string text)
        {
            var newTask = new TodoTask { Id = _nextId++, Text = text, Completed = false };
            _tasks.Add(newTask);
            SaveTasks();
        }

        public void DeleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                SaveTasks();
            }
        }

        public void ToggleCompleted(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Completed = !task.Completed;
                SaveTasks();
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
                _tasks = JsonSerializer.Deserialize<List<TodoTask>>(tasksJson);
                if (_tasks.Any())
                {
                    _nextId = _tasks.Max(t => t.Id) + 1;
                }
            }
        }
    }
}