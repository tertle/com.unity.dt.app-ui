using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extensions for <see cref="VisualElement"/> class.
    /// </summary>
    public static class VisualElementExtensions
    {
#if !UNITY_EDITOR && ENABLE_IL2CPP && !CONDITIONAL_WEAK_TABLE_IL2CPP
        static readonly WeakReferenceTable<VisualElement, AdditionalData> k_AdditionalDataCache =
            new WeakReferenceTable<VisualElement, AdditionalData>();
#else
        static readonly ConditionalWeakTable<VisualElement, AdditionalData> k_AdditionalDataCache =
            new ConditionalWeakTable<VisualElement, AdditionalData>();
#endif

        static bool TryGetValue(VisualElement key, out AdditionalData val)
        {
            if (key is IAdditionalDataHolder holder)
            {
                val = holder.additionalData;
                return val != null;
            }
            return k_AdditionalDataCache.TryGetValue(key, out val);
        }

        static AdditionalData GetOrCreateValue(VisualElement key)
        {
            if (key is IAdditionalDataHolder holder)
                return holder.additionalData ?? (holder.additionalData = new AdditionalData());
            return k_AdditionalDataCache.GetOrCreateValue(key);
        }

        /// <summary>
        /// Check if a <see cref="VisualElement"/> is invisible.
        /// </summary>
        /// <remarks>
        /// An element is considered invisible if it's not attached to a panel, its visibility attribute is set to <see cref="Visibility.Hidden"/>,
        /// has an opacity lower than 0.001 or has a display style set to <see cref="DisplayStyle.None"/>.
        /// </remarks>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <returns> True if the element is invisible, false otherwise.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static bool IsInvisible(this VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return element.panel == null || !element.visible || element.resolvedStyle.opacity < 0.001f || element.resolvedStyle.display == DisplayStyle.None;
        }

        /// <summary>
        /// Set the picking mode of a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <param name="enabled"> True to enable picking, false otherwise.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static void EnablePicking(this VisualElement element, bool enabled)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
        }

        /// <summary>
        /// Get child elements of a given type.
        /// </summary>
        /// <param name="element">The parent element.</param>
        /// <param name="recursive">If true, the search will be recursive.</param>
        /// <typeparam name="T">The type of the child elements to search for.</typeparam>
        /// <returns> A list of child elements of the given type.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static IEnumerable<T> GetChildren<T>(this VisualElement element, bool recursive)
            where T : VisualElement
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var res = new List<T>();

            foreach (var child in element.Children())
            {
                if (child is T c)
                {
                    res.Add(c);
                    if (recursive)
                        res.AddRange(c.GetChildren<T>(true));
                }
            }

            return res;
        }

        /// <summary>
        /// Get the preferred placement for a <see cref="VisualElement"/>'s <see cref="Tooltip"/>.
        /// </summary>
        /// <param name="element">The <see cref="VisualElement"/> which contains a tooltip.</param>
        /// <returns>The preferred placement, previously set using <see cref="SetPreferredTooltipPlacement"/>
        /// or the closest value set on an element.</returns>
        /// <exception cref="ArgumentNullException">The <see cref="VisualElement"/> object can't be null.</exception>
        public static PopoverPlacement GetPreferredTooltipPlacement(this VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element is IContextOverrideElement {preferredTooltipPlacementOverride: {IsSet:true}} provider)
                return provider.preferredTooltipPlacementOverride.Value;

            if (TryGetValue(element, out var data) && data.preferredTooltipPlacement.IsSet)
                return data.preferredTooltipPlacement.Value;

            var context = element.GetContext<TooltipPlacementContext>();
            return context!.placement;
        }

        /// <summary>
        /// Set a preferred <see cref="Tooltip"/> placement.
        /// </summary>
        /// <param name="element">The target visual element.</param>
        /// <param name="placement">The placement value.</param>
        /// <exception cref="ArgumentNullException">The <see cref="VisualElement"/> object can't be null.</exception>
        public static void SetPreferredTooltipPlacement(this VisualElement element, OptionalEnum<PopoverPlacement> placement)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element is IContextOverrideElement el)
            {
                el.preferredTooltipPlacementOverride = placement;
                return;
            }

            if (placement.IsSet)
            {
                var data = GetOrCreateValue(element);
                data.preferredTooltipPlacement = placement;
            }
            else if (TryGetValue(element, out var data))
            {
                data.preferredTooltipPlacement = placement;
            }
        }

        /// <summary>
        /// Get the tooltip template for a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <returns> The tooltip template.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static VisualElement GetTooltipTemplate(this VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return TryGetValue(element, out var data) ? data.tooltipTemplate : null;
        }

        /// <summary>
        /// Set the tooltip template for a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="template"> The tooltip template.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static void SetTooltipTemplate(this VisualElement element, VisualElement template)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var data = GetOrCreateValue(element);
            data.tooltipTemplate = template;
        }

        /// <summary>
        /// Callback to populate the tooltip content.
        /// </summary>
        /// <param name="tooltip"> The tooltip element to populate.</param>
        public delegate void TooltipContentCallback(VisualElement tooltip);

        /// <summary>
        /// Set the tooltip content for a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="callback"> The callback to invoke to populate the tooltip.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        /// <exception cref="InvalidOperationException"> You must call SetTooltipTemplate before setting the tooltip content.</exception>
        /// <remarks>
        /// You must call <see cref="SetTooltipTemplate"/> method first in order to have a valid tooltip template to populate.
        /// </remarks>
        public static void SetTooltipContent(this VisualElement element, TooltipContentCallback callback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (GetTooltipTemplate(element) == null && callback != null)
                throw new InvalidOperationException("You must call SetTooltipTemplate before setting the tooltip content.");

            var data = GetOrCreateValue(element);
            data.tooltipContentCallback = callback;
        }

        /// <summary>
        /// Get the tooltip content for a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <returns> The tooltip content callback.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static TooltipContentCallback GetTooltipContent(this VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return TryGetValue(element, out var data) ? data.tooltipContentCallback : null;
        }

        /// <summary>
        /// Register a callback to be invoked when the tooltip content changes.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="callback"> The callback to invoke when the tooltip content changes.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        internal static void RegisterTooltipContentChangedCallback(this VisualElement element, Action callback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var data = GetOrCreateValue(element);
            data.tooltipContentCallbackChanged += callback;
        }

        /// <summary>
        /// Unregister a callback to be invoked when the tooltip content changes.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="callback"> The callback to invoke when the tooltip content changes.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        internal static void UnregisterTooltipContentChangedCallback(this VisualElement element, Action callback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var data = GetOrCreateValue(element);
            data.tooltipContentCallbackChanged -= callback;
        }

        /// <summary>
        /// Register a callback to be invoked when the tooltip template changes.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="callback"> The callback to invoke when the tooltip template changes.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        internal static void RegisterTooltipTemplateChangedCallback(this VisualElement element, Action callback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var data = GetOrCreateValue(element);
            data.tooltipTemplateChanged += callback;
        }

        /// <summary>
        /// Unregister a callback to be invoked when the tooltip template changes.
        /// </summary>
        /// <param name="element"> The target visual element.</param>
        /// <param name="callback"> The callback to invoke when the tooltip template changes.</param>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        internal static void UnregisterTooltipTemplateChangedCallback(this VisualElement element, Action callback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var data = GetOrCreateValue(element);
            data.tooltipTemplateChanged -= callback;
        }

        /// <summary>
        /// Additional Data that should be stored on any <see cref="VisualElement"/> object.
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        internal class AdditionalData
        {
            /// <summary>
            /// Callbacks to invoke when the element is attached to a panel to send the context changed event.
            /// </summary>
            internal Dictionary<Type, EventCallback<AttachToPanelEvent>>
                sendContextChangedOnAttachedToPanelCallbacksPerType { get; }  = new();

            /// <summary>
            /// Callbacks to invoke when the context changes.
            /// </summary>
            internal Dictionary<Type, List<object>> contextChangedCallbacksPerType { get; } = new ();

            /// <summary>
            /// Callbacks to invoke when the element is attached to a panel to change the context.
            /// </summary>
            internal Dictionary<Type, List<EventCallback<AttachToPanelEvent>>>
                contextChangedOnAttachedToPanelCallbacksPerType { get; } = new ();

            /// <summary>
            /// The Contexts collection.
            /// </summary>
            internal Dictionary<Type, IContext> contexts { get; } = new ();

            /// <summary>
            /// The previous Contexts collection.
            /// </summary>
            internal Dictionary<Type, IContext> previousContexts { get; } = new ();

            /// <summary>
            /// The preferred placement for a tooltip.
            /// </summary>
            public OptionalEnum<PopoverPlacement> preferredTooltipPlacement { get; set; } = OptionalEnum<PopoverPlacement>.none;

            VisualElement m_TooltipTemplate;

            /// <summary>
            /// The tooltip template to use for this element.
            /// </summary>
            public VisualElement tooltipTemplate
            {
                get => m_TooltipTemplate;
                set
                {
                    if (m_TooltipTemplate != value)
                    {
                        m_TooltipTemplate = value;
                        tooltipTemplateChanged?.Invoke();
                    }
                }
            }

            TooltipContentCallback m_TooltipContentCallback;

            /// <summary>
            /// The callback to populate the tooltip content.
            /// </summary>
            public TooltipContentCallback tooltipContentCallback
            {
                get => m_TooltipContentCallback;
                set
                {
                    m_TooltipContentCallback = value;
                    tooltipContentCallbackChanged?.Invoke();
                }
            }

            /// <summary>
            /// Event to invoke when the tooltip template changes.
            /// </summary>
            public event Action tooltipContentCallbackChanged;

            /// <summary>
            /// Event to invoke when the tooltip template changes.
            /// </summary>
            public event Action tooltipTemplateChanged;
        }

        /// <summary>
        /// Find the closest context provider in the hierarchy of a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <returns> The context provider.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static VisualElement GetContextProvider<T>(this VisualElement element)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var el = element;

            while (el != null)
            {
                if (TryGetValue(el, out var data) && data.contexts.ContainsKey(typeof(T)))
                    return el;

                el = el.hierarchy.parent;
            }

            return null;
        }

        /// <summary>
        /// Get the context of a given type in a <see cref="VisualElement"/>.
        /// </summary>
        /// <remarks>
        /// This method will look for the context in the current element and its parents without checking
        /// if the element is part of the visual tree.
        /// </remarks>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <returns> The context.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static T GetContext<T>(this VisualElement element)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element.GetContextProvider<T>() is { } provider)
            {
                TryGetValue(provider, out var data);
                return (T)data.contexts[typeof(T)];
            }

            return default;
        }

        /// <summary>
        /// Get the context of a given type in a <see cref="VisualElement"/> if this element is provider of this context.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <returns> The context if the element is provider of this context, null otherwise.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static T GetSelfContext<T>(this VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (TryGetValue(element, out var data) && data.contexts.ContainsKey(typeof(T)))
                return (T)data.contexts[typeof(T)];

            return default;
        }

        /// <summary>
        /// Make the element provide a context of a given type in a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <param name="context"> The context.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static void ProvideContext<T>(this VisualElement element, T context)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var data = GetOrCreateValue(element);

            void OnAttached(AttachToPanelEvent evt)
            {
                if (evt.destinationPanel != null)
                    element.SendContextChangedEvent<T>();
            }

            if (context == null)
            {
                if (data.contexts.ContainsKey(typeof(T)))
                {
                    data.contexts.Remove(typeof(T));
                    element.SendContextChangedEvent<T>();
                }
                if (data.sendContextChangedOnAttachedToPanelCallbacksPerType.TryGetValue(typeof(T), out var callback))
                {
                    data.sendContextChangedOnAttachedToPanelCallbacksPerType.Remove(typeof(T));
                    element.UnregisterCallback(callback);
                }
            }
            else
            {
                data.contexts[typeof(T)] = context;
                if (!data.sendContextChangedOnAttachedToPanelCallbacksPerType.ContainsKey(typeof(T)))
                {
                    var callback = new EventCallback<AttachToPanelEvent>(OnAttached);
                    data.sendContextChangedOnAttachedToPanelCallbacksPerType[typeof(T)] = callback;
                    element.RegisterCallback(callback);
                }
                element.SendContextChangedEvent<T>();
            }
        }

        /// <summary>
        /// Check if a <see cref="VisualElement"/> provides a context of a given type.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <returns> True if the element provides the context, false otherwise.</returns>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object can't be null.</exception>
        public static bool IsContextProvider<T>(this VisualElement element)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return TryGetValue(element, out var data) && data.contexts.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Register a callback to be invoked when the context of a given type changes in a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <param name="callback"> The callback.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        public static void RegisterContextChangedCallback<T>(this VisualElement element, EventCallback<ContextChangedEvent<T>> callback)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            void SendContextChangedEventLocal()
            {
                var context = element.GetContext<T>();

                if (TryGetValue(element, out var data) && data.previousContexts.ContainsKey(typeof(T)))
                {
                    var previousContext = data.previousContexts[typeof(T)];
                    if (previousContext != null && previousContext.Equals(context))
                        return;
                }

                using var evt = ContextChangedEvent<T>.GetPooled(context);
                evt.target = element;
                callback(evt);
            }

            void OnAttached(AttachToPanelEvent attachToPanelEvent)
            {
                if (attachToPanelEvent.destinationPanel != null)
                    SendContextChangedEventLocal();
            }

            var data = GetOrCreateValue(element);
            if (data.contextChangedCallbacksPerType.TryGetValue(typeof(T), out var callbacks))
            {
                if (!callbacks.Contains(callback))
                {
                    callbacks.Add(callback);
                    var attachCallback = new EventCallback<AttachToPanelEvent>(OnAttached);
                    data.contextChangedOnAttachedToPanelCallbacksPerType[typeof(T)].Add(attachCallback);
                    element.RegisterCallback(attachCallback);
                    if (element.panel != null)
                        SendContextChangedEventLocal();
                }
            }
            else
            {
                callbacks = new List<object> { callback };
                data.contextChangedCallbacksPerType[typeof(T)] = callbacks;
                var attachCallback = new EventCallback<AttachToPanelEvent>(OnAttached);
                data.contextChangedOnAttachedToPanelCallbacksPerType[typeof(T)] = new List<EventCallback<AttachToPanelEvent>> { attachCallback };
                element.RegisterCallback(attachCallback);
                if (element.panel != null)
                    SendContextChangedEventLocal();
            }
        }

        /// <summary>
        /// Unregister a callback to be invoked when the context of a given type changes in a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="element"> The <see cref="VisualElement"/> object.</param>
        /// <param name="callback"> The callback.</param>
        /// <typeparam name="T"> The type of the context.</typeparam>
        /// <exception cref="ArgumentNullException"> The <see cref="VisualElement"/> object and the callback can't be null.</exception>
        public static void UnregisterContextChangedCallback<T>(this VisualElement element, EventCallback<ContextChangedEvent<T>> callback)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (TryGetValue(element, out var data) &&
                data.contextChangedCallbacksPerType.TryGetValue(typeof(T), out var callbacks))
            {
                var index = callbacks.IndexOf(callback);

                if (index >= 0)
                {
                    callbacks.RemoveAt(index);
                    var attachCallback = data.contextChangedOnAttachedToPanelCallbacksPerType[typeof(T)][index];
                    data.contextChangedOnAttachedToPanelCallbacksPerType[typeof(T)].RemoveAt(index);
                    element.UnregisterCallback(attachCallback);
                }
            }
        }

        internal static void SendContextChangedEvent<T>(this VisualElement element)
            where T : IContext
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var context = element.GetContext<T>();

            using var evt = ContextChangedEvent<T>.GetPooled(context);
            evt.target = element;

            void CallCallbacks(VisualElement el, ContextChangedEvent<T> evt)
            {
                if (TryGetValue(el, out var data))
                {
                    if (data.previousContexts.ContainsKey(typeof(T)))
                    {
                        var previousContext = data.previousContexts[typeof(T)];
                        if (previousContext != null && previousContext.Equals(evt.context))
                            return;
                    }

                    data.previousContexts[typeof(T)] = evt.context;

                    if (data.contextChangedCallbacksPerType.TryGetValue(typeof(T), out var callbacks))
                    {
                        foreach (var cb in callbacks)
                        {
                            ((EventCallback<ContextChangedEvent<T>>)cb).Invoke(evt);
                        }
                    }
                }
            }

            void SendContextChangedEventToChildren(VisualElement parent, ContextChangedEvent<T> evt)
            {
                if (parent.IsContextProvider<T>())
                    return;

                CallCallbacks(parent, evt);

                foreach (var c in parent.hierarchy.Children())
                {
                    SendContextChangedEventToChildren(c, evt);
                }
            }

            CallCallbacks(element, evt);
            foreach (var child in element.hierarchy.Children())
            {
                SendContextChangedEventToChildren(child, evt);
            }
        }
    }
}
