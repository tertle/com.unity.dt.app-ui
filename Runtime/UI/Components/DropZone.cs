using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.AppUI.Core;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A drop zone is a container that can be used to drop content into.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DropZone : BaseVisualElement
    {
        /// <summary>
        /// The controller used to manage the drag and drop operations.
        /// </summary>
        public DropZoneController controller { get; }

        /// <summary>
        /// The DropZone main styling class.
        /// </summary>
        public const string ussClassName = "appui-dropzone";

        /// <summary>
        /// The DropZone frame styling class.
        /// </summary>
        public const string frameUssClassName = ussClassName + "__frame";

        /// <summary>
        /// The DropZone background styling class.
        /// </summary>
        public const string backgroundUssClassName = ussClassName + "__background";

        /// <summary>
        /// The DropZone state styling class.
        /// </summary>
        [EnumName("GetDropZoneStateUssClassName", typeof(DragAndDropState))]
        public const string stateUssClassName = ussClassName + "--";

        /// <summary>
        /// The DropZone visible indicator styling class.
        /// </summary>
        public const string visibleIndicatorUssClassName = ussClassName + "--visible-indicator";

        readonly ExVisualElement m_DropZoneFrame;

        readonly VisualElement m_Background;

        Pressable m_Clickable;

        bool m_VisibleIndicator;

        DragAndDropState m_DropZoneState;

        IVisualElementScheduledItem m_FrameAnimation;

        /// <summary>
        /// The container used to display the content.
        /// </summary>
        public override VisualElement contentContainer => m_DropZoneFrame;

        /// <summary>
        /// The state of the DropZone.
        /// </summary>
        public DragAndDropState state
        {
            get => m_DropZoneState;
            set
            {
                if (m_DropZoneState == value)
                    return;
                RemoveFromClassList(GetDropZoneStateUssClassName(m_DropZoneState));
                m_DropZoneState = value;
                AddToClassList(GetDropZoneStateUssClassName(m_DropZoneState));
                RefreshVisibleIndicatorStyle();
            }
        }

        /// <summary>
        /// The visible indicator state of the DropZone.
        /// </summary>
        public bool visibleIndicator
        {
            get => m_VisibleIndicator;
            set
            {
                if (m_VisibleIndicator == value)
                    return;
                m_VisibleIndicator = value;
                RefreshVisibleIndicatorStyle();
            }
        }

        bool isIndicatorVisible => (state == DragAndDropState.Default && m_VisibleIndicator) || (state != DragAndDropState.Default);

        /// <summary>
        /// Create a new DropZone.
        /// </summary>
        public DropZone()
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList(ussClassName);

            m_Background = new VisualElement { name = backgroundUssClassName, pickingMode = PickingMode.Ignore };
            m_Background.AddToClassList(backgroundUssClassName);
            hierarchy.Add(m_Background);

            m_DropZoneFrame = new ExVisualElement
            {
                name = frameUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Borders,
            };
            m_DropZoneFrame.AddToClassList(frameUssClassName);
            hierarchy.Add(m_DropZoneFrame);

            controller = new DropZoneController(OnControllerStateChanged);
            this.AddManipulator(controller);

            state = DragAndDropState.Default;
            visibleIndicator = false;
            generateVisualContent = OnGenerateVisualContent;
        }

        void OnControllerStateChanged(DragAndDropState controllerState) => state = controllerState;

        void OnGenerateVisualContent(MeshGenerationContext _) => RefreshAnimation();

        void RefreshVisibleIndicatorStyle()
        {
            EnableInClassList(visibleIndicatorUssClassName, isIndicatorVisible);
            RefreshAnimation();
        }

        void RefreshAnimation()
        {
            if (this.IsInvisible() || !isIndicatorVisible)
            {
                pickingMode = PickingMode.Ignore;
                m_FrameAnimation?.Pause();
                m_FrameAnimation = null;
                return;
            }

            pickingMode = PickingMode.Position;
            m_FrameAnimation ??= m_DropZoneFrame.schedule
                .Execute(m_DropZoneFrame.MarkDirtyRepaint)
                .Every(Styles.animationRefreshDelayMs);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// The UXML factory for the DropZone.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DropZone, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DropZone"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits { }

#endif
    }
}
