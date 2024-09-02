using System;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVMRedux
{
    [Serializable]
    public record AppState
    {
        [SerializeField]
        public Todo[] todos;

        [SerializeField]
        public string searchInput;
    }

    public class Reducers
    {
        public static AppState CreateTodoReducer(AppState state, Redux.Action<string> action)
        {
            var newTodo = new Todo
            {
                id = Guid.NewGuid().ToString(),
                text = action.payload,
                completed = false
            };
            var newTodos = new Todo[state.todos.Length + 1];
            Array.Copy(state.todos, newTodos, state.todos.Length);
            newTodos[^1] = newTodo;
            return state with {todos = newTodos};
        }

        public static AppState DeleteTodoReducer(AppState state, Redux.Action<string> action)
        {
            var newTodos = new Todo[state.todos.Length - 1];
            var index = 0;
            foreach (var todo in state.todos)
            {
                if (todo.id != action.payload)
                {
                    newTodos[index] = todo;
                    index++;
                }
            }
            return state with {todos = newTodos};
        }

        public static AppState CompleteTodoReducer(AppState state, Redux.Action<(string id, bool completed)> action)
        {
            var newTodos = new Todo[state.todos.Length];
            var index = 0;
            foreach (var todo in state.todos)
            {
                if (todo.id == action.payload.id)
                {
                    newTodos[index] = new Todo
                    {
                        id = todo.id,
                        text = todo.text,
                        completed = action.payload.completed
                    };
                }
                else
                {
                    newTodos[index] = todo;
                }
                index++;
            }
            return state with {todos = newTodos};
        }

        public static AppState EditTodoReducer(AppState state, Redux.Action<(string id, string text)> action)
        {
            var newTodos = new Todo[state.todos.Length];
            var index = 0;
            foreach (var todo in state.todos)
            {
                if (todo.id == action.payload.id)
                {
                    newTodos[index] = new Todo
                    {
                        id = todo.id,
                        text = action.payload.text,
                        completed = todo.completed
                    };
                }
                else
                {
                    newTodos[index] = todo;
                }
                index++;
            }
            return state with {todos = newTodos};
        }

        public static AppState SetSearchInputReducer(AppState state, Redux.Action<string> action)
        {
            return state with {searchInput = action.payload};
        }
    }

    public static class Actions
    {
        public const string createTodo = "app/CreateTodo";
        public const string deleteTodo = "app/DeleteTodo";
        public const string completeTodo = "app/CompleteTodo";
        public const string editTodo = "app/EditTodo";
        public const string setSearchInput = "app/SetSearchInput";
    }
}
