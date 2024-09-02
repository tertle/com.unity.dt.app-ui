using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Navigation.Editor
{
    /// <summary>
    /// The Navigation Graph View.
    /// </summary>
    class NavigationGraphView : GraphView
    {
        /// <summary>
        /// Event fired when a new graph has been loaded.
        /// </summary>
        public event Action<NavGraph> graphChanged;

        /// <summary>
        /// The graph asset currently used by the graph view.
        /// </summary>
        public NavGraphViewAsset graphAsset { get; private set; }

        /// <summary>
        /// The current graph displayed in the graph view.
        /// </summary>
        public NavGraph currentGraph { get; private set; }

        /// <summary>
        /// Whether or not the graph view should follow the navigation in Play mode.
        /// </summary>
        public bool followNavigation { get; set; } = true;

        NavDestination m_CurrentDestination;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NavigationGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            NavController.destinationChanged += OnDestinationChanged;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                m_CurrentDestination = null;
                RefreshCurrentDestination(false);
            }
        }

        void OnDestinationChanged(NavController controller, NavDestination destination)
        {
            m_CurrentDestination = controller.graphAsset == graphAsset ? destination : null;
            if (m_CurrentDestination)
                RefreshCurrentDestination(followNavigation);
        }

        /// <summary>
        /// Sets the graph asset to use in the graph view.
        /// </summary>
        /// <param name="asset"> The graph asset to use. </param>
        public void SetGraphAsset(NavGraphViewAsset asset)
        {
            if (asset == null)
                return;

            graphAsset = asset;
            viewDataKey = asset.name;
            SetGraph(graphAsset.rootGraph);
        }

        /// <summary>
        /// Sets the graph that should be displayed in the graph view.
        /// </summary>
        /// <param name="graph"> The graph to display. </param>
        public void SetGraph(NavGraph graph)
        {
            SetGraphInternal(graph, false);
        }

        void SetGraphInternal(NavGraph graph, bool keepFollowing)
        {
            graphViewChanged -= OnGraphViewChanged;
            ClearGraph();

            currentGraph = graph;

            if (graph == null)
                return;

            var destinations = graphAsset
                .nodes
                .Where(c => c is NavDestination d && d.parent == graph)
                .Cast<NavDestination>();

            var subGraphs = graphAsset
                .nodes
                .Where(c => c is NavGraph g && g.parent == graph)
                .Cast<NavGraph>();

            foreach (var destination in destinations)
            {
                AddElement(GenerateNavigationDestinationNode(destination));

                foreach (var destinationAction in destination.actions)
                {
                    AddElement(GenerateActionNode(destinationAction));
                }
            }

            foreach (var subGraph in subGraphs)
            {
                AddElement(GenerateNavigationSubGraphNode(subGraph));
            }

            foreach (var globalAction in graph.actions)
            {
                AddElement(GenerateActionNode(globalAction));
            }

            foreach (var node in nodes)
            {
                if (node is Node {userData: NavAction action})
                {
                    if (action.destination != null)
                    {
                        var destinationNode = GetNodeByGuid(action.destination.guid);
                        var edge = node.outputContainer.Q<Port>().ConnectTo<Edge>(destinationNode.inputContainer.Q<Port>());
                        AddElement(edge);
                    }
                }

                if (node is Node {userData: NavDestination dest})
                {
                    foreach (var destAction in dest.actions)
                    {
                        var destActionNode = GetNodeByGuid(destAction.guid);
                        var edge = node.outputContainer.Q<Port>().ConnectTo<Edge>(destActionNode.inputContainer.Q<Port>());
                        AddElement(edge);
                    }
                }
            }

            RefreshStartDestination();
            RefreshGlobalActions();
            RefreshCurrentDestination(keepFollowing);

            if (!keepFollowing)
                schedule.Execute(() => FrameAll()).ExecuteLater(2L);

            graphViewChanged += OnGraphViewChanged;
            graphChanged?.Invoke(graph);
        }

        void ClearGraph()
        {
            DeleteElements(graphElements.ToList());
        }

        Node GenerateActionNode(NavAction action)
        {
            var serializedObject = new SerializedObject(action);
            var node = new Node
            {
                title = action.name,
                viewDataKey = action.guid,
                userData = action
            };
            node.titleContainer.Q<Label>("title-label").bindingPath = "m_Name";
            node.extensionContainer.Add(GenerateActionInspector(serializedObject));
            node.topContainer.parent.Insert(0, GenerateDescription("Action"));

            var refPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(string));
            refPort.portName = "";
            node.inputContainer.Add(refPort);

            var destPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int));
            destPort.portName = "Destination";
            node.outputContainer.Add(destPort);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(action.position, Vector2.zero));

            node.Bind(serializedObject);

            return node;
        }

        VisualElement GenerateActionInspector(SerializedObject obj)
        {
            var container = new VisualElement();
            container.AddToClassList("inspector-container");

            var nameField = new PropertyField(obj.FindProperty("m_Name"));
            container.Add(nameField);

            var options = new PropertyField(obj.FindProperty("m_Options"));
            container.Add(options);

            var args = new PropertyField(obj.FindProperty("m_DefaultArguments"));
            container.Add(args);

            return container;
        }

        Node GenerateNavigationDestinationNode(NavDestination destination)
        {
            var serializedObject = new SerializedObject(destination);
            var node = new Node
            {
                title = destination.name,
                viewDataKey = destination.guid,
                userData = destination
            };
            node.AddToClassList("destination");
            node.titleContainer.Q<Label>("title-label").bindingPath = "m_Name";
            node.extensionContainer.Add(GenerateDestinationInspector(serializedObject));
            node.topContainer.parent.Insert(0, GenerateDescription("Destination"));

            var previousPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
            previousPort.portName = "";
            node.inputContainer.Add(previousPort);

            var nextPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(string));
            nextPort.portName = "Actions";
            node.outputContainer.Add(nextPort);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(destination.position, Vector2.zero));

            node.Bind(serializedObject);

            return node;
        }

        static readonly Dictionary<string, string> k_Templates = TypeCache
            .GetTypesDerivedFrom<NavigationScreen>()
            .ToDictionary(t => t.AssemblyQualifiedName, t => t.Name + " (" + t.Namespace + ")");

        internal static VisualElement GenerateDestinationInspector(SerializedObject obj)
        {
            var container = new VisualElement();
            container.AddToClassList("inspector-container");

            var nameField = new TextField("Name Identifier");
            container.Add(nameField);
            nameField.bindingPath = "m_Name";

            var labelField = new TextField("Label");
            container.Add(labelField);
            labelField.bindingPath = "m_Label";

            var templateField = new PopupField<string>(
                "Template",
                k_Templates.Keys.ToList(),
                0,
                s => string.IsNullOrEmpty(s) ? "None" : k_Templates[s],
                s => k_Templates[s]);
            container.Add(templateField);
            templateField.bindingPath = "m_Template";

            var showBottomNavBar = new Toggle("Show Bottom Nav Bar");
            container.Add(showBottomNavBar);
            showBottomNavBar.bindingPath = "m_ShowBottomNavBar";

            var showAppBar = new Toggle("Show App Bar");
            container.Add(showAppBar);
            showAppBar.bindingPath = "m_ShowAppBar";

            var showBackButton = new Toggle("Show Back Button");
            container.Add(showBackButton);
            showBackButton.bindingPath = "m_ShowBackButton";

            var showDrawer = new Toggle("Show Drawer");
            container.Add(showDrawer);
            showDrawer.bindingPath = "m_ShowDrawer";

            var showNavigationRail = new Toggle("Show Navigation Rail");
            container.Add(showNavigationRail);
            showNavigationRail.bindingPath = "m_ShowNavigationRail";

            return container;
        }

        static VisualElement GenerateDescription(string description)
        {
            var container = new VisualElement { pickingMode = PickingMode.Ignore };
            container.AddToClassList("description-container");
            var label = new Label(description) { pickingMode = PickingMode.Ignore, name= "description-label" };
            label.AddToClassList("description-label");
            container.Add(label);
            return container;
        }

        static VisualElement GenerateGraphInspector(SerializedObject obj)
        {
            var container = new VisualElement();
            container.AddToClassList("inspector-container");

            var nameField = new TextField("Name Identifier");
            container.Add(nameField);
            nameField.bindingPath = "m_Name";

            return container;
        }

        Node GenerateNavigationSubGraphNode(NavGraph subGraph)
        {
            var serializedObject = new SerializedObject(subGraph);
            var node = new Node
            {
                title = subGraph.name,
                viewDataKey = subGraph.guid,
                userData = subGraph
            };
            node.AddToClassList("subgraph");
            node.titleContainer.Q<Label>("title-label").bindingPath = "m_Name";
            node.extensionContainer.Add(GenerateGraphInspector(serializedObject));
            node.topContainer.parent.Insert(0, GenerateDescription("Nested Graph"));
            node.RegisterCallback<MouseDownEvent>(OnSubGraphClicked);

            var previousPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
            previousPort.portName = "";
            node.inputContainer.Add(previousPort);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(subGraph.position, Vector2.zero));

            node.Bind(serializedObject);

            return node;
        }

        void OnSubGraphClicked(MouseDownEvent evt)
        {
            if (evt.target is Node node && evt.clickCount == 2)
            {
                var subGraph = (NavGraph)node.userData;
                SetGraph(subGraph);
            }
        }

        void OnRemoveEdge(Edge edge)
        {
            if (edge.output.node.userData is NavDestination dest)
            {
                // set the action as global (current graph)
                var action = (NavAction)edge.input.node.userData;
                dest.actions.Remove(action);
                if (!currentGraph.actions.Contains(action))
                {
                    currentGraph.actions.Add(action);
                    EditorUtility.SetDirty(currentGraph);
                    EditorUtility.SetDirty(dest);
                }
            }
            else if (edge.output.node.userData is NavAction act)
            {
                // set the destination to null
                act.destination = null;
                var inputPort = edge.output.node.inputContainer.Q<Port>();
                foreach (var c in inputPort.connections)
                {
                    var d = (NavDestination)c.output.node.userData;
                    EditorUtility.SetDirty(d);
                }
                EditorUtility.SetDirty(currentGraph);
            }
        }

        void OnRemoveNode(Node node)
        {
            // Remove all edges connected to this node
            foreach (var port in node.inputContainer.Children().Cast<Port>())
            {
                foreach (var edgeToRemove in port.connections)
                {
                    OnRemoveEdge(edgeToRemove);
                    RemoveElement(edgeToRemove);
                }
            }

            foreach (var port in node.outputContainer.Children().Cast<Port>())
            {
                foreach (var edgeToRemove in port.connections)
                {
                    OnRemoveEdge(edgeToRemove);
                    RemoveElement(edgeToRemove);
                }
            }

            var component = node.userData;
            if (component is NavDestination destination && currentGraph.startDestination == destination)
            {
                currentGraph.startDestination = null;
                EditorUtility.SetDirty(currentGraph);
            }

            if (component is NavGraphViewHierarchicalNode c)
            {
                graphAsset.RemoveNode(c);
                EditorUtility.SetDirty(graphAsset);
            }
            else
            {
                var action = (NavAction) component;
                currentGraph.actions.Remove(action);
                graphAsset.RemoveNode(action);
                EditorUtility.SetDirty(graphAsset);
                EditorUtility.SetDirty(currentGraph);
            }
        }

        void OnRemoveElement(GraphElement element)
        {
            if (element is Edge edge)
            {
                OnRemoveEdge(edge);
            }
            else if (element is Node node)
            {
                OnRemoveNode(node);
            }
        }

        void OnCreateEdge(Edge edge)
        {
            if (edge.output.node is Node {userData: NavAction action})
            {
                // set the destination
                var destination = (NavGraphViewHierarchicalNode)edge.input.node.userData;
                action.destination = destination;
                var inputPort = edge.output.node.inputContainer.Q<Port>();
                foreach (var c in inputPort.connections)
                {
                    var d = (NavDestination)c.output.node.userData;
                    EditorUtility.SetDirty(d);
                }
                EditorUtility.SetDirty(currentGraph);
            }
            else if (edge.output.node is Node {userData: NavDestination dest})
            {
                // set the action as local (destination)
                var a = (NavAction)edge.input.node.userData;
                currentGraph.actions.Remove(a);
                dest.actions.Add(a);
                EditorUtility.SetDirty(dest);
                EditorUtility.SetDirty(currentGraph);
            }
        }

        static void OnMoveElement(GraphElement element)
        {
            if (element is Node { userData: NavGraphViewHierarchicalNode component } node)
            {
                component.position = node.GetPosition().position;
                EditorUtility.SetDirty(component);
            }
            if (element is Node { userData: NavAction action } actionNode)
            {
                action.position = actionNode.GetPosition().position;
            }
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                   OnRemoveElement(element);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    OnCreateEdge(edge);
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (var element in graphViewChange.movedElements)
                {
                    OnMoveElement(element);
                }
            }

            if (currentGraph.startDestination == null)
            {
                var destination = graphAsset.nodes.FirstOrDefault(c => c is NavDestination dest && dest.parent == currentGraph);
                if (destination != null)
                {
                    currentGraph.startDestination = (NavDestination)destination;
                    EditorUtility.SetDirty(currentGraph);
                }
            }

            RefreshStartDestination();
            RefreshGlobalActions();

            AssetDatabase.SaveAssets();

            return graphViewChange;
        }

        /// <summary>
        /// Get all ports compatible with given port.
        /// </summary>
        /// <param name="startPort"> The port to get compatible ports for.</param>
        /// <param name="nodeAdapter"> The node adapter.</param>
        /// <returns> A list of compatible ports.</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            if (startPort.node is Node {userData: NavDestination})
            {
                if (startPort.direction == Direction.Input)
                    return ports.Where(port =>
                        port.direction == Direction.Output && port.node is Node {userData: NavAction}).ToList();
                if (startPort.direction == Direction.Output)
                    return ports.Where(port =>
                        port.direction == Direction.Input && port.node is Node {userData: NavAction}).ToList();
            }
            else if (startPort.node is Node {userData: NavAction})
            {
                if (startPort.direction == Direction.Input)
                    return ports.Where(port =>
                        port.direction == Direction.Output && port.node is Node {userData: NavDestination}).ToList();
                if (startPort.direction == Direction.Output)
                    return ports.Where(port =>
                        port.direction == Direction.Input && port.node is Node {userData: NavDestination or NavGraph}).ToList();
            }
            else if (startPort.node is Node {userData: NavGraph})
            {
                if (startPort.direction == Direction.Input)
                    return ports.Where(port =>
                        port.direction == Direction.Output && port.node is Node {userData: NavAction}).ToList();
            }

            return new List<Port>();
        }

        /// <summary>
        /// Build the contextual menu for the graph view.
        /// </summary>
        /// <param name="evt"> The event.</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (graphAsset == null)
                return;

            evt.menu.AppendAction("Create Destination",
                CreateDestination, DropdownMenuAction.AlwaysEnabled);

            evt.menu.AppendAction("Create Sub Graph",
                CreateSubGraph, DropdownMenuAction.AlwaysEnabled);

            evt.menu.AppendAction("Create Action",
                CreateAction, DropdownMenuAction.AlwaysEnabled);

            evt.menu.AppendSeparator();

            var isDestinationSelected = selection.Count == 1 && selection[0] is Node {userData: NavDestination};

            if (isDestinationSelected)
            {
                var dest = (NavDestination) ((Node)selection[0]).userData;
                evt.menu.AppendAction("Set as Start Destination",
                    SetAsStartDestination, dest.parent.startDestination == dest ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                evt.menu.AppendSeparator();
            }

            var isSubGraphSelected  = selection.Count == 1 && selection[0] is Node {userData: NavGraph};

            if (isSubGraphSelected)
            {
                evt.menu.AppendAction("Open Nested Graph",
                    OpenSubGraph, DropdownMenuAction.AlwaysEnabled);
                evt.menu.AppendSeparator();
            }

            var isRootGraph = currentGraph.parent == null;

            evt.menu.AppendAction("Go Back",
                (a) => GoBack(),
                isRootGraph ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
        }

        void CreateAction(DropdownMenuAction obj)
        {
            var position = MouseToContent(obj.eventInfo.localMousePosition) - new Vector2(16, 16);
            var action = ScriptableObject.CreateInstance<NavAction>();
            action.name = "New Action";
            action.position = position;
            currentGraph.actions.Add(action);
            EditorUtility.SetDirty(currentGraph);
            graphAsset.AddNode(action);
            AddElement(GenerateActionNode(action));
            RefreshGlobalActions();
        }

        void SetAsStartDestination(DropdownMenuAction action)
        {
            var node = (Node)selection[0];
            var destination = (NavDestination)node.userData;
            currentGraph.startDestination = destination;
            EditorUtility.SetDirty(currentGraph);

            RefreshStartDestination();
        }

        void RefreshStartDestination()
        {
            foreach (var node in nodes.ToList().Where(n => n.userData is NavDestination))
            {
                var destination = (NavDestination)node.userData;
                node.title = destination.name;
                var isStartDestination = currentGraph.startDestination == destination;
                node.contentContainer.Q<Label>("description-label").text = isStartDestination ? "Start Destination" : "Destination";
                node.EnableInClassList("start-destination", isStartDestination);
            }
        }

        void RefreshCurrentDestination(bool keepFollowing)
        {
            if (!graphAsset)
                return;

            Node node = null;
            if (m_CurrentDestination)
            {
                node = GetNodeByGuid(m_CurrentDestination.guid);
                if (node == null)
                {
                    var p = m_CurrentDestination.parent;
                    while (p && GetNodeByGuid(p.guid) == null)
                    {
                        p = p.parent;
                    }

                    node = p ? GetNodeByGuid(p.guid) : null;

                    if (node is {userData: NavGraph graph} && keepFollowing)
                    {
                        SetGraphInternal(graph, true);
                        return;
                    }
                }
            }

            if (node == null && keepFollowing)
            {
                SetGraph(graphAsset.rootGraph);
                return;
            }

            foreach (var n in nodes.Where(n => n is {userData: NavGraphViewHierarchicalNode}))
            {
                if (node == null)
                    continue;

                var isCurrent = node == n;
                n.EnableInClassList("current-destination", isCurrent);
                if (isCurrent && followNavigation)
                {
                    ClearSelection();
                    AddToSelection(n);
                    schedule.Execute(() => FrameSelection()).ExecuteLater(2L);
                }
            }
        }

        void RefreshGlobalActions()
        {
            foreach (var node in nodes.Where(n => n is {userData: NavAction}))
            {
                var isGlobal = currentGraph.actions.Contains((NavAction)node.userData);
                node.contentContainer.Q<Label>("description-label").text = isGlobal ? "Global Action" : "Action";
                node.EnableInClassList("global-action", isGlobal);
                node.EnableInClassList("local-action", !isGlobal);
            }
        }

        void GoBack()
        {
            SetGraph(currentGraph.parent);
        }

        void OpenSubGraph(DropdownMenuAction action)
        {
            var node = (Node)selection[0];
            var subGraph = node.userData as NavGraph;

            SetGraph(subGraph);
        }

        Vector2 MouseToContent(Vector2 position)
        {
            position.x = (position.x - contentViewContainer.worldBound.x) / scale;
            position.y = (position.y - contentViewContainer.worldBound.y) / scale;
            return position;
        }

        void CreateSubGraph(DropdownMenuAction action)
        {
            var position = MouseToContent(action.eventInfo.localMousePosition) - new Vector2(16, 16);
            var graph = ScriptableObject.CreateInstance<NavGraph>();
            graph.name = "nav_new_graph";
            graph.parent = currentGraph;
            graph.position = position;
            graph.guid = GUID.Generate().ToString();
            var startDestination = ScriptableObject.CreateInstance<NavDestination>();
            startDestination.name = "Start";
            startDestination.parent = graph;
            startDestination.position = position;
            startDestination.guid = GUID.Generate().ToString();
            graph.startDestination = startDestination;

            graphAsset.AddNode(graph);
            graphAsset.AddNode(startDestination);

            var node = GenerateNavigationSubGraphNode(graph);
            AddElement(node);
            node.SetPosition(new Rect(position, Vector2.zero));
        }

        void CreateDestination(DropdownMenuAction action)
        {
            var position = MouseToContent(action.eventInfo.localMousePosition) - new Vector2(16, 16);
            var destination = ScriptableObject.CreateInstance<NavDestination>();
            destination.name = "new_destination";
            destination.parent = currentGraph;
            destination.position = position;
            destination.guid = GUID.Generate().ToString();

            graphAsset.AddNode(destination);

            var node = GenerateNavigationDestinationNode(destination);
            AddElement(node);
            node.SetPosition(new Rect(position, Vector2.zero));
        }
    }
}
