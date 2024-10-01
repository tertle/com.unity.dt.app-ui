// This file draws upon the concepts found in the ObservableObject implementation from the MVVM Toolkit library (CommunityToolkit/dotnet),
// more information in Third Party Notices.md

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A base class for objects of which the properties must be observable.
    /// </summary>
    /// <remarks>
    /// Starting Unity 6, you can implement the <see cref="UnityEngine.UIElements.IDataSourceViewHashProvider"/> interface
    /// in your derived <see cref="ObservableObject"/> class to provide a hash for the data source view.
    /// That will give you the ability to control the data source view's update behavior
    /// (instead of always updating the view when the data source changes).
    /// </remarks>
    public abstract class ObservableObject :
        System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
#if ENABLE_RUNTIME_DATA_BINDINGS
        , UnityEngine.UIElements.INotifyBindablePropertyChanged
#endif
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="e"/> is <see langword="null"/>. </exception>
        protected virtual void OnPropertyChanging(System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            PropertyChanging?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName"> The name of the property that is changing. </param>
        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="e"/> is <see langword="null"/>. </exception>
        protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            PropertyChanged?.Invoke(this, e);
#if ENABLE_RUNTIME_DATA_BINDINGS
            // a bit ugly, but we need to notify the UI Toolkit runtime binding system that a property has changed.
            propertyChanged?.Invoke(this, new UnityEngine.UIElements.BindablePropertyChangedEventArgs(e.PropertyName));
#endif
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <param name="field"> The field to set. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <returns> <see langword="true"/> if the property has changed, <see langword="false"/> otherwise. </returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <param name="field"> The field to set. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="comparer"> The comparer to use to check if the property has changed. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <returns> <see langword="true"/> if the property has changed, <see langword="false"/> otherwise. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="comparer"/> is <see langword="null"/>. </exception>
        protected bool SetProperty<T>(ref T field, T newValue, EqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (comparer.Equals(field, newValue))
                return false;
            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="callback"> The callback to invoke to set the property. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <returns> <see langword="true"/> if the property has changed, <see langword="false"/> otherwise. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="callback"/> is <see langword="null"/>. </exception>
        protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;
            OnPropertyChanging(propertyName);
            callback(newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="comparer"> The comparer to use to check if the property has changed. </param>
        /// <param name="callback"> The callback to invoke to set the property. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <returns> <see langword="true"/> if the property has changed, <see langword="false"/> otherwise. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="callback"/> or <paramref name="comparer"/> is <see langword="null"/>. </exception>
        protected bool SetProperty<T>(T oldValue, T newValue, EqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (comparer.Equals(oldValue, newValue))
                return false;
            OnPropertyChanging(propertyName);
            callback(newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events.
        /// </summary>
        /// <param name="oldValue"> The old value. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="model"> The model to set the property on. </param>
        /// <param name="callback"> The callback to invoke to set the property. </param>
        /// <param name="propertyName"> The name of the property that has changed. </param>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <typeparam name="TModel"> The type of the model. </typeparam>
        /// <returns> <see langword="true"/> if the property has changed, <see langword="false"/> otherwise. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="callback"/> or <paramref name="model"/> is <see langword="null"/>. </exception>
        protected bool SetProperty<T, TModel>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;
            OnPropertyChanging(propertyName);
            callback(model, newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

#if ENABLE_RUNTIME_DATA_BINDINGS
        // UnityEngine.UIElements.INotifyBindablePropertyChanged implementation

        /// <summary>
        /// Occurs when a property value has changed, using the <see cref="UnityEngine.UIElements.INotifyBindablePropertyChanged"/> event.
        /// </summary>
        /// <remarks>
        /// This event is used to notify the UI Toolkit runtime binding system that a property has changed.
        /// </remarks>
        public event EventHandler<UnityEngine.UIElements.BindablePropertyChangedEventArgs>? propertyChanged;
#endif
    }
}
