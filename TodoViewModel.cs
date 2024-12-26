using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ToDoList
{
    public class TodoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TodoItem> _todoItems;
        private string _newTodoText;

        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set
            {
                _todoItems = value;
                OnPropertyChanged();
            }
        }

        public string NewTodoText
        {
            get => _newTodoText;
            set
            {
                _newTodoText = value;
                OnPropertyChanged();
            }
        }

        public Command AddTodoCommand { get; }
        public Command<TodoItem> DeleteTodoCommand { get; }
        public Command<TodoItem> EditTodoCommand { get; }

        private const string StorageKey = "todos";

        public TodoViewModel()
        {
            TodoItems = [];
            AddTodoCommand = new Command(AddTodo);
            DeleteTodoCommand = new Command<TodoItem>(DeleteTodo);
            EditTodoCommand = new Command<TodoItem>(EditTodo);

            LoadTodos(); // Загружаем задачи при создании ViewModel

            // Подписываемся на изменения в каждом элементе
            foreach (var todo in TodoItems)
            {
                todo.PropertyChanged += Todo_PropertyChanged;
            }
        }

        private void Todo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoItem.IsCompleted))
            {
                SaveTodos(); // Сохраняем состояние при изменении IsCompleted
            }
        }

        public void SaveTodos()
        {
            string jsonTodos = JsonSerializer.Serialize(TodoItems);
            Preferences.Set(StorageKey, jsonTodos);
        }

        private void LoadTodos()
        {
            string jsonTodos = Preferences.Get(StorageKey, "[]");
            var loadedTodos = JsonSerializer.Deserialize<List<TodoItem>>(jsonTodos);
            TodoItems = new ObservableCollection<TodoItem>(loadedTodos);

            foreach (var todo in TodoItems)
            {
                todo.PropertyChanged += Todo_PropertyChanged;
            }
        }

        private void AddTodo()
        {
            if (string.IsNullOrWhiteSpace(NewTodoText))
                return;

            var newTodo = new TodoItem { Text = NewTodoText };
            newTodo.PropertyChanged += Todo_PropertyChanged;
            TodoItems.Add(newTodo);
            NewTodoText = string.Empty;
            SaveTodos();
        }

        private void DeleteTodo(TodoItem item)
        {
            if (item != null)
            {
                TodoItems.Remove(item);
                SaveTodos();
            }
        }

        private async void EditTodo(TodoItem item)
        {
            if (item == null || item.IsCompleted)
                return;

            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Edit Todo",
                "Enter new text:",
                initialValue: item.Text);

            if (!string.IsNullOrWhiteSpace(result))
            {
                item.Text = result;
                SaveTodos();
                LoadTodos();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}