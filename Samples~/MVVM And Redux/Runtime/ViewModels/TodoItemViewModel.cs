using Unity.AppUI.MVVM;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class TodoItemViewModel : ObservableObject
    {
        public RelayCommand toggleCompletedCommand { get; }

        public RelayCommand deleteCommand { get; }

        public RelayCommand<string> editCommand { get; }

        public event System.Action<TodoItemViewModel> deleteRequested;

        public event System.Action<(TodoItemViewModel, string)> renamed;

        public bool completed
        {
            get => todo.completed;
            set => SetProperty(ref todo.completed, value);
        }

        public Todo todo { get; }

        public TodoItemViewModel(Todo todo)
        {
            this.todo = todo;
            toggleCompletedCommand = new RelayCommand(ToggleCompleted);
            deleteCommand = new RelayCommand(Delete);
            editCommand = new RelayCommand<string>(Edit);
        }

        void Delete()
        {
            deleteRequested?.Invoke(this);
        }

        void Edit(string newName)
        {
             renamed?.Invoke((this, newName));
        }

        void ToggleCompleted()
        {
            completed = !completed;
        }
    }
}
