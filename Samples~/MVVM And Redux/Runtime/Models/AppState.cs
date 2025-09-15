using System;
using System.Linq;
using Unity.AppUI.Redux;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVMRedux
{
    [Serializable]
    public record AppState
    {
        [SerializeField]
        public Todo[] todos = Array.Empty<Todo>();

        [SerializeField]
        public string searchInput;

        public override string ToString()
        {
            return @$"{{
    searchInput: {searchInput},
    todos: {(todos != null ? todos.Select(todo => todo.ToString()).Aggregate((a, b) => $"        {a},\n        {b}") : "null")}
}}";
        }
    }

    public class Reducers
    {
        public static AppState CreateTodoReducer(AppState state, IAction<string> action)
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

        public static AppState DeleteTodoReducer(AppState state, IAction<string> action)
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

        public static AppState CompleteTodoReducer(AppState state, IAction<(string id, bool completed)> action)
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

        public static AppState EditTodoReducer(AppState state, IAction<(string id, string text)> action)
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

        public static AppState SetSearchInputReducer(AppState state, IAction<string> action)
        {
            return state with {searchInput = action.payload};
        }
    }

    public static class Actions
    {
        internal static readonly ActionCreator<string> createTodo = "app/CreateTodo";
        internal static readonly ActionCreator<string> deleteTodo = "app/DeleteTodo";
        internal static readonly ActionCreator<(string, bool)> completeTodo = "app/CompleteTodo";
        internal static readonly ActionCreator<(string,string)> editTodo = "app/EditTodo";
        internal static readonly ActionCreator<string> setSearchInput = "app/SetSearchInput";
    }
}
