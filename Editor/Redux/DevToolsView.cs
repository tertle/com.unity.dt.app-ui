using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using Unity.AppUI.Redux;
using Unity.AppUI.Redux.DevTools;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using SliderInt = UnityEngine.UIElements.SliderInt;
using Toolbar = UnityEditor.UIElements.Toolbar;
#if UNITY_ENABLE_VERSION_CONTROL
using Unity.Plastic.Newtonsoft.Json;
#endif

namespace Unity.AppUI.Editor.Redux.DevTools
{
    /// <summary>
    /// The User Interface for the Redux DevTools.<br/>
    /// You can instantiate this class and add it to a visual tree to display the DevTools anywhere in your UI.
    /// </summary>
    class DevToolsView : BaseVisualElement // VisualElement
    {
        const string k_Hidden = "unity-hidden";

        DevToolsUI m_DevTools;

        Toolbar m_Toolbar;

        ToolbarToggle m_RecordButton;

        ToolbarButton m_PlayPauseButton;

        ToolbarButton m_BackButton;

        ToolbarButton m_ForwardButton;

        SliderInt m_TimeSlider;

        EditorToolbarDropdown m_StoreDropdown;

        GenericMenu m_StoreMenu;

        TwoPaneSplitView m_SplitView;

        ListView m_ActionHistoryListView;

        VisualElement m_Footer;

        VisualElement m_InspectorPane;

        VisualElement m_ActionSearchPane;

        ToolbarSearchField m_ActionSearch;

        VisualElement m_ActionHistoryPane;

#if UNITY_ENABLE_TABVIEW
        TabView m_InspectorTabView;
#else
        VisualElement m_InspectorTabView;
#endif

        readonly List<IDisposableSubscription> m_Subscriptions = new();

        VisualElement m_ActionInspectorPane;

        VisualElement m_StateInspectorPane;

        VisualElement m_DiffInspectorPane;

        TextElement m_ActionInspectorContent;

        TextElement m_StateInspectorContent;

        TextElement m_DiffInspectorContent;

        /// <summary>
        /// Create a new instance of the DevTools GUI.
        /// </summary>
        public DevToolsView()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("dc664d6bcfb84f56a9abeb5b57501e11")));

            AddToClassList("dev-tools");
            m_Toolbar = new Toolbar();
            m_Toolbar.AddToClassList("dev-tools__toolbar");

            m_StoreDropdown = new EditorToolbarDropdown();
            m_StoreDropdown.AddToClassList("dev-tools__store-dropdown");
            m_StoreDropdown.SetEnabled(false);
            m_StoreDropdown.clicked += OnStoreDropdownClicked;
            m_Toolbar.Add(m_StoreDropdown);

            m_RecordButton = new ToolbarToggle();
            m_RecordButton.RegisterValueChangedCallback(OnRecordToggled);
            m_RecordButton.AddToClassList("dev-tools__record-button");
            m_Toolbar.Add(m_RecordButton);

            m_PlayPauseButton = new ToolbarButton(OnPlayPauseToggled);
            m_PlayPauseButton.Add(new VisualElement());
            m_PlayPauseButton.AddToClassList("dev-tools__play-pause-button");
            m_Toolbar.Add(m_PlayPauseButton);

            m_BackButton = new ToolbarButton(OnBackButtonClicked);
            m_BackButton.AddToClassList("dev-tools__back-button");
            m_BackButton.Add(new VisualElement());
            m_BackButton.tooltip = "Jump back to previous state";
            m_Toolbar.Add(m_BackButton);

            m_ForwardButton = new ToolbarButton(OnForwardButtonClicked);
            m_ForwardButton.AddToClassList("dev-tools__forward-button");
            m_ForwardButton.Add(new VisualElement());
            m_ForwardButton.tooltip = "Jump forward to next state";
            m_Toolbar.Add(m_ForwardButton);

            m_TimeSlider = new SliderInt(0, 1)
            {
                showInputField = true,
#if UNITY_ENABLE_SLIDER_FILL
                fill = true
#endif
            };
            m_TimeSlider.AddToClassList("dev-tools__time-slider");
            m_TimeSlider.RegisterValueChangedCallback(OnTimeSliderValueChanged);
            m_Toolbar.Add(m_TimeSlider);

            m_SplitView = new TwoPaneSplitView
            {
                fixedPaneIndex = 0,
                fixedPaneInitialDimension = 200,
                viewDataKey = "DevToolsSplitView",
            };
            m_SplitView.AddToClassList("dev-tools__split-view");

            m_ActionHistoryPane = new VisualElement();
            m_ActionHistoryPane.AddToClassList("dev-tools-action-history__pane");
            m_SplitView.Add(m_ActionHistoryPane);

            m_ActionSearchPane = new VisualElement();
            m_ActionSearchPane.AddToClassList("dev-tools-action-history__search-pane");
            m_ActionHistoryPane.Add(m_ActionSearchPane);

            m_ActionSearch = new UnityEditor.UIElements.ToolbarSearchField();
            m_ActionSearch.AddToClassList("dev-tools-action-history__search-field");
            m_ActionSearch.RegisterValueChangedCallback(OnSearchFilterChanged);
            m_ActionSearchPane.Add(m_ActionSearch);

            m_ActionHistoryListView = new ListView(null, 24, MakeActionItem, BindActionItem);
            m_ActionHistoryListView.unbindItem = UnbindActionItem;
            m_ActionHistoryListView.selectionType = SelectionType.Multiple;
#if UITK_SELECTED_INDICES_CHANGED
            m_ActionHistoryListView.selectedIndicesChanged += OnActionHistorySelectionChanged;
#else
            m_ActionHistoryListView.onSelectedIndicesChange -= OnActionHistorySelectionChanged;
#endif
            m_ActionHistoryListView.AddToClassList("dev-tools-action-history__list-view");
            m_ActionHistoryPane.Add(m_ActionHistoryListView);

            m_InspectorPane = new VisualElement();
            m_InspectorPane.AddToClassList("dev-tools-inspector__pane");
            m_SplitView.Add(m_InspectorPane);

#if UNITY_ENABLE_TABVIEW
            m_InspectorTabView = new TabView();
            m_InspectorTabView.Add(new Tab("Action"));
            m_InspectorTabView.Add(new Tab("State"));
            m_InspectorTabView.Add(new Tab("Diff"));
            m_InspectorTabView.AddToClassList("dev-tools-inspector__tab-view");
            m_InspectorTabView.activeTabChanged += OnTabChanged;
#else
            m_InspectorTabView = new VisualElement();
            var actionButton = new Button() { text = "Action" };
            actionButton.clickable.clickedWithEventInfo += OnTabChanged;
            m_InspectorTabView.Add(actionButton);
            var stateButton = new Button() { text = "State" };
            stateButton.clickable.clickedWithEventInfo += OnTabChanged;
            m_InspectorTabView.Add(stateButton);
            var diffButton = new Button() { text = "Diff" };
            diffButton.clickable.clickedWithEventInfo += OnTabChanged;
            m_InspectorTabView.Add(diffButton);
            m_InspectorTabView.AddToClassList("dev-tools-inspector__tab-view");
#endif
            m_InspectorPane.Add(m_InspectorTabView);

            m_ActionInspectorPane = new ScrollView();
            m_ActionInspectorPane.AddToClassList("dev-tools-inspector__action-pane");
            m_InspectorPane.Add(m_ActionInspectorPane);

            m_ActionInspectorContent = new TextElement { enableRichText = true };
            m_ActionInspectorContent.AddToClassList("dev-tools-inspector__action-content");
            m_ActionInspectorPane.Add(m_ActionInspectorContent);

            m_StateInspectorPane = new ScrollView();
            m_StateInspectorPane.AddToClassList("dev-tools-inspector__state-pane");
            m_InspectorPane.Add(m_StateInspectorPane);

            m_StateInspectorContent = new TextElement { enableRichText = true };
            m_StateInspectorContent.AddToClassList("dev-tools-inspector__state-content");
            m_StateInspectorPane.Add(m_StateInspectorContent);

            m_DiffInspectorPane = new ScrollView();
            m_DiffInspectorPane.AddToClassList("dev-tools-inspector__diff-pane");

            m_InspectorPane.Add(m_DiffInspectorPane);

            m_DiffInspectorContent = new TextElement { enableRichText = true };
            m_DiffInspectorContent.AddToClassList("dev-tools-inspector__diff-content");
            m_DiffInspectorPane.Add(m_DiffInspectorContent);

            m_Footer = new VisualElement();
            m_Footer.AddToClassList("dev-tools__footer");

            // todo: add footer content with import/export, etc.

            Add(m_Toolbar);
            Add(m_SplitView);
            Add(m_Footer);

            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
        }

        // UI event handlers

        void OnTimeSliderValueChanged(ChangeEvent<int> evt)
        {
            var liftedState = m_DevTools.store.GetState()?.liftedState;
            if (liftedState != null && evt.newValue != liftedState.currentStateIndex)
                m_DevTools.Dispatch(new JumpToStateAction(evt.newValue));
        }

        void OnActionHistorySelectionChanged(IEnumerable<int> indices)
        {
            var indicesArray = indices.ToArray();
            switch (indicesArray.Length)
            {
                case > 1:
                {
                    // choose the one under the cursor
                    var id = (int)panel.Pick(Event.current.mousePosition).FindAncestorUserData();
                    m_DevTools.Dispatch(new SelectActionForInspectionAction(id));
                    break;
                }
                case 1:
                {
                    var id = (int)m_ActionHistoryListView.itemsSource[indicesArray[0]];
                    m_DevTools.Dispatch(new SelectActionForInspectionAction(id));
                    break;
                }
            }
        }

        void OnSearchFilterChanged(ChangeEvent<string> evt)
        {
            m_DevTools.Dispatch(new SetActionSearchFilterAction(evt.newValue));
        }

        void OnStoreDropdownClicked()
        {
            m_StoreMenu?.DropDown(m_StoreDropdown.worldBound);
        }

#if UNITY_ENABLE_TABVIEW
        void OnTabChanged(Tab previousTab, Tab newTab)
        {
            EditorPrefs.SetString(k_SelectedTabKey, newTab.label);
            m_DevTools.Dispatch(new SelectInspectorTabAction(newTab.label));
        }
#else
        void OnTabChanged(EventBase evt)
        {
            if (evt.target is not Button {text: {} tabName})
                return;
            EditorPrefs.SetString(k_SelectedTabKey, tabName);
            m_DevTools.Dispatch(new SelectInspectorTabAction(tabName));
        }
#endif

        void OnRecordToggled(ChangeEvent<bool> evt)
        {
            var toggleOn = evt.newValue;
            m_DevTools.Dispatch(new PauseRecordingAction(!toggleOn));
        }

        void OnPlayPauseToggled()
        {
            var isPlaying = m_DevTools.store.GetState().isPlaying;
            if (!isPlaying)
            {
                var liftedState = m_DevTools.store.GetState().liftedState;
                if (liftedState == null || liftedState.computedStates.Count == 0)
                    return;

                if (liftedState.currentStateIndex == liftedState.computedStates.Count - 1)
                    m_DevTools.Dispatch(new JumpToStateAction(0));
                m_DevTools.Dispatch(DevToolsUIAction.startPlayingBack.Invoke(m_DevTools));
            }
            else
            {
                m_DevTools.Dispatch(new PausePlayingAction());
            }
        }

        void OnBackButtonClicked()
        {
            var currentIndex = m_DevTools.store.GetState().liftedState?.currentStateIndex;
            if (currentIndex.HasValue)
            {
                var clampedIndex = Math.Max(0, currentIndex.Value - 1);
                if (clampedIndex != currentIndex)
                    m_DevTools.Dispatch(new JumpToStateAction(clampedIndex));
            }
        }

        void OnForwardButtonClicked()
        {
            var liftedState = m_DevTools.store.GetState().liftedState;
            if (liftedState != null)
            {
                var clampedIndex = Math.Min(liftedState.computedStates.Count - 1, liftedState.currentStateIndex + 1);
                if (clampedIndex != liftedState.currentStateIndex)
                    m_DevTools.Dispatch(new JumpToStateAction(clampedIndex));
            }
        }

        void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            CreateStore();
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            ReleaseStore();
        }

        static VisualElement MakeActionItem()
        {
            var root = new VisualElement();

            var item = new VisualElement();
            item.AddToClassList("dev-tools-action-history-item");

            var label = new Label("Action") { name = "action-label" };
            label.AddToClassList("dev-tools-action-history-item__label");
            item.Add(label);

            var time = new Label("Time") { name = "action-time" };
            time.AddToClassList("dev-tools-action-history-item__time");
            item.Add(time);

            var buttonGroup = new VisualElement();
            buttonGroup.AddToClassList("dev-tools-action-history-item__button-group");

            var jumpToButton = new Button { name = "jumpToButton", text = "Jump" };
            jumpToButton.AddToClassList("dev-tools-action-history-item__jump-to-button");
            jumpToButton.tooltip = "Jump to the state after this action has been dispatched";
            buttonGroup.Add(jumpToButton);

            var skipButton = new Button { name="skippedButton", text = "Skip" };
            skipButton.AddToClassList("dev-tools-action-history-item__skip-button");
            skipButton.tooltip = "Toggle the inclusion of this action in the state computation";
            buttonGroup.Add(skipButton);

            item.Add(buttonGroup);

            root.Add(item);
            return root;
        }

        void BindActionItem(VisualElement item, int idx)
        {
            const string playedUssClassName = "dev-tools-action-history-item--played";
            const string skippedUssClassName = "dev-tools-action-history-item--skipped";

            var liftedState = m_DevTools.store.GetState().liftedState;
            var actionId = (int) m_ActionHistoryListView.itemsSource[idx];
            if (item == null || liftedState == null)
                return;

            item.userData = actionId;
            var action = liftedState.actionsById[actionId];
            var stagedActionIndex = liftedState.stagedActionIds.IndexOf(actionId);
            item[0].EnableInClassList(playedUssClassName, stagedActionIndex <= liftedState.currentStateIndex);
            item[0].EnableInClassList(skippedUssClassName, liftedState.skippedActionIds.Contains(actionId));
            var firstActionId = (int)m_ActionHistoryListView.itemsSource[0];
            var firstAction = liftedState.actionsById[firstActionId];
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(action.timestamp).LocalDateTime;
            var firstTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(firstAction.timestamp).LocalDateTime;
            var timeDelta = timestamp - firstTimestamp;

            const string timeFormat = "HH:mm:ss";

            var label = item.Q<Label>("action-label");
            label.text = action.action.type;

            var time = item.Q<Label>("action-time");
            time.text = idx == 0 ? timestamp.ToString(timeFormat) : "+" + timeDelta;

            var jumpToButton = item.Q<Button>("jumpToButton");
            jumpToButton.userData = actionId;
            jumpToButton.clickable.clickedWithEventInfo += JumpToState;

            var skipButton = item.Q<Button>("skippedButton");
            skipButton.userData = actionId;
            skipButton.clickable.clickedWithEventInfo += ToggleAction;
        }

        void ToggleAction(EventBase evt)
        {
            if (evt.target is not VisualElement {userData: int actionId})
                return;

            var start = int.MaxValue;
            var end = int.MinValue;
            var active = m_DevTools.store.GetState().liftedState.skippedActionIds.Contains(actionId);
            var selection = m_ActionHistoryListView.selectedIndices;
            using var enumerator = selection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var next = (int)m_ActionHistoryListView.itemsSource[enumerator.Current];
                if (next < start)
                    start = next;
                if (next > end)
                    end = next;
            }
            if (actionId < start || actionId > end)
            {
                start = actionId;
                end = actionId;
            }
            m_DevTools.Dispatch(new ToggleAction(start, end, active));
        }

        void JumpToState(EventBase evt)
        {
            if (evt.target is not VisualElement {userData: int actionId})
                return;

            m_DevTools.Dispatch(new JumpToActionAction(actionId));
        }

        void UnbindActionItem(VisualElement item, int index)
        {
            var jumpToButton = item.Q<Button>("jumpToButton");
            jumpToButton.userData = null;
            jumpToButton.clickable.clickedWithEventInfo -= JumpToState;

            var skipButton = item.Q<Button>("skippedButton");
            skipButton.userData = null;
            skipButton.clickable.clickedWithEventInfo -= ToggleAction;

            item.userData = null;
        }

        // store lifecycle

        void CreateStore()
        {
            m_DevTools = new DevToolsUI(GetInitialState());
            var sub = m_DevTools.store.Subscribe(OnStateChanged);
            m_Subscriptions.Add(sub);

            m_DevTools.Sync();
        }

        void ReleaseStore()
        {
            foreach (var subscription in m_Subscriptions)
            {
                subscription.Dispose();
            }
            m_Subscriptions.Clear();
            m_DevTools.Dispose();
            m_DevTools = null;
        }

        // persistence

        const string k_DefaultTab = "Action";
        const string k_SelectedTabKey = "Unity.AppUI.Editor.Redux.DevToolsView.SelectedTab";

        // state initializer

        DevToolsUIState GetInitialState()
        {
            var state = new DevToolsUIState
            {
                actionInspector =
                {
                    selectedTab = EditorPrefs.GetString(k_SelectedTabKey, k_DefaultTab)
                }
            };
            return state;
        }

        // state change listeners

        void OnStateChanged(DevToolsUIState state)
        {
            // Store selection
            UpdateStoreDropdown(state);

            // Toolbar
            UpdateToolbar(state);

            // Action list
            UpdateActionList(state);

            // Inspector
            UpdateInspector(state);
        }

        void UpdateStoreDropdown(DevToolsUIState state)
        {
            // Update dropdown menu
            string selectedStoreName = null;
            m_StoreMenu = new GenericMenu();
            foreach (var store in state.stores)
            {
                var storeId = store.id;
                var isSelected = storeId == state.selectedStoreId;
                var storeName = store.name;
                if (isSelected)
                    selectedStoreName = storeName;
                m_StoreMenu.AddItem(new GUIContent(storeName), isSelected, () =>
                {
                    m_DevTools.Dispatch(new SelectStoreAction(storeId));
                });
            }

            // Update dropdown button state
            m_StoreDropdown.SetEnabled(state.stores is { Length: > 0 });
            m_StoreDropdown.text = string.IsNullOrEmpty(selectedStoreName) ? "Select Store..." : selectedStoreName;
        }

        void UpdateToolbar(DevToolsUIState state)
        {
            var liftedState = state.liftedState;
            if (liftedState == null)
            {
                // disable everything
                m_RecordButton.SetEnabled(false);
                m_PlayPauseButton.SetEnabled(false);
                m_BackButton.SetEnabled(false);
                m_ForwardButton.SetEnabled(false);
                m_TimeSlider.SetEnabled(false);
                return;
            }

            // Update recording controls
            m_RecordButton.SetValueWithoutNotify(!liftedState.isPaused);
            m_RecordButton.SetEnabled(true);
            m_RecordButton.tooltip = liftedState.isPaused ? "Resume recording" : "Pause recording";

            // Update time travel controls
            m_TimeSlider.lowValue = 0;
            m_TimeSlider.highValue = Math.Max(0, liftedState.computedStates.Count - 1);
            m_TimeSlider.SetValueWithoutNotify(Math.Max(0, liftedState.currentStateIndex));
            m_TimeSlider.SetEnabled(true);

            var isValidRange = m_TimeSlider.highValue > 0;
            m_BackButton.SetEnabled(isValidRange && liftedState.currentStateIndex > 0);
            m_ForwardButton.SetEnabled(isValidRange && liftedState.currentStateIndex < liftedState.computedStates.Count - 1);

            // Update play/pause controls
            m_PlayPauseButton.EnableInClassList("dev-tools__play-pause-button--playing", state.isPlaying);
            m_PlayPauseButton.SetEnabled(isValidRange);
            m_PlayPauseButton.tooltip = state.isPlaying ? "Pause playback" : "Start playback";
        }

        void UpdateActionList(DevToolsUIState state)
        {
            var liftedState = state.liftedState;
            if (liftedState == null)
            {
                m_ActionHistoryListView.itemsSource = Array.Empty<int>();
                return;
            }

            var actions = GetFilteredActions(state);

            var previousCount = m_ActionHistoryListView.itemsSource?.Count ?? 0;
            m_ActionHistoryListView.itemsSource = actions;
            var newCount = actions.Length;

            if (state.settings.autoScrollToBottom && previousCount < newCount)
                m_ActionHistoryListView.ScrollToItem(-1);

            // Update search field
            m_ActionSearch.SetValueWithoutNotify(state.actionInspector.searchFilter ?? string.Empty);
        }

        void UpdateInspector(DevToolsUIState state)
        {
            for (var i = 0; i < m_InspectorTabView.childCount; i++)
            {
#if UNITY_ENABLE_TABVIEW
                if (m_InspectorTabView[i] is Tab tab && tab.label == state.actionInspector.selectedTab)
                    m_InspectorTabView.activeTab = tab;

#else
                if (m_InspectorTabView[i] is Button btn)
                    btn.EnableInClassList("dev-tools-inspector__tab-view-item--selected",
                        btn.text == state.actionInspector.selectedTab);
#endif
            }

            var liftedState = state.liftedState;
            if (liftedState == null || !state.actionInspector.selectedActionId.IsSet)
            {
                ClearInspector();
                return;
            }

            var actionId = state.actionInspector.selectedActionId.Value;
            if (!liftedState.actionsById.TryGetValue(actionId, out var action))
            {
                ClearInspector();
                return;
            }

            var stagedActionIds = liftedState.stagedActionIds;
            var stateIndex = stagedActionIds.IndexOf(actionId);
            var computedState = stateIndex >= 0 && stateIndex < liftedState.computedStates.Count
                ? liftedState.computedStates[stateIndex].state : null;
            var previousStateIndex = stateIndex - 1;
            while (previousStateIndex > 0 && liftedState.skippedActionIds.Contains(stagedActionIds[previousStateIndex]))
            {
                previousStateIndex--;
            }
            previousStateIndex = Mathf.Max(0, previousStateIndex);
            var previousComputedState = liftedState.computedStates[previousStateIndex].state;

            // Update inspectors based on selected tab
            switch (state.actionInspector.selectedTab)
            {
                case "Action":
                    m_ActionInspectorContent.text = GetSerializedObject(action.action);
                    m_ActionInspectorPane.RemoveFromClassList(k_Hidden);
                    m_StateInspectorPane.AddToClassList(k_Hidden);
                    m_DiffInspectorPane.AddToClassList(k_Hidden);
                    break;

                case "State":
                    m_StateInspectorContent.text = GetSerializedObject(computedState);
                    m_ActionInspectorPane.AddToClassList(k_Hidden);
                    m_StateInspectorPane.RemoveFromClassList(k_Hidden);
                    m_DiffInspectorPane.AddToClassList(k_Hidden);
                    break;

                case "Diff":
                    var previousSerialized = GetSerializedObject(previousComputedState);
                    var currentSerialized = GetSerializedObject(computedState);
                    var diff = DiffGenerator.CreateDiffRichText(previousSerialized, currentSerialized);
                    m_DiffInspectorContent.text = diff;
                    m_ActionInspectorPane.AddToClassList(k_Hidden);
                    m_StateInspectorPane.AddToClassList(k_Hidden);
                    m_DiffInspectorPane.RemoveFromClassList(k_Hidden);
                    break;
            }
        }

        // helpers

        static int[] GetFilteredActions(DevToolsUIState state)
        {
            if (state.liftedState == null)
                return Array.Empty<int>();

            var needSearch = !string.IsNullOrEmpty(state.actionInspector.searchFilter);
            return state.liftedState.actionsById.Keys
                .Where(id =>
                {
                    if (!state.settings.showSkippedActions && state.liftedState.skippedActionIds.Contains(id))
                        return false;

                    if (needSearch)
                        return MatchAction(state.liftedState.actionsById[id].action, state.actionInspector.searchFilter);

                    return true;
                })
                .OrderBy(id => id)
                .Take(state.settings.maxActionsToShow) // should be done by the instrumentation with maxAge
                .ToArray();
        }

        static bool MatchAction(IAction action, string searchFilter)
        {
            return action.type.Contains(searchFilter, StringComparison.OrdinalIgnoreCase);
        }

        void ClearInspector()
        {
            m_ActionInspectorContent.text = string.Empty;
            m_StateInspectorContent.text = string.Empty;
            m_DiffInspectorContent.text = string.Empty;
        }

        static string GetSerializedObject(object obj)
        {
#if UNITY_ENABLE_VERSION_CONTROL
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
            {
                // ignore null values to reduce the size of the json
                NullValueHandling = NullValueHandling.Ignore
            });
#else
            return EditorJsonUtility.ToJson(obj, true);
#endif
        }
    }
}
