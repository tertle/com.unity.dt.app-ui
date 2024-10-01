# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.0-pre.11] - 2024-10-01

### Changed

- Make Host optional when intializing an `App` implementation.
- Changed StoryBookEnumProperty class to become a generic type. You need to specify the Enum type as typedef parameter.
- Changed the styling of primary action buttons in AlertDialog UI element.
- Changed the Context API to propagate contexts over _internal_ components hierarchies and not just the high-level hierarchy (via `contentContainer`).

### Added

- Added support of UITK Runtime DataBinding system in ObervableObject class.
- Added PanAndZoom Manipulator
- Added CircularProgress story in Storybook sample.
- Added AlertDialog icon Design Tokens to customize icons directly from USS.
- Added ActionBar story in Storybook sample.
- Added AlertDialog examples in the UI Kit sample.
- Added Phosphor Icons
- Added support of Localization package in the Storybook window. You can now change the current used Locale in the window via a dropdown in the Storybook context toolbar. This dropdown will appear only if you have the Unity Localization package installed and have at least one existing Locale set up in your Localiztion settings.

### Fixed

- Fixed TouchSlider progress element overlapping parent's borders.
- Fixed force mark as dirty repaint Progress UI element when swtiching its variant.
- Fixed Localization support in the ActionBar UI element.
- Removed force blurring the BaseSlider (SliderFloat, TouchSliderFloat, etc) when the users stops interacting with it.

## [2.0.0-pre.10] - 2024-09-11

### Changed

- Renamed `IUIToolkitApp.mainPage` property into `IUIToolkitApp.rootVisualElement` for more clarity.
- Storybook stories are now sorted alphabetically.

### Added

- Added `IApp.services` property which returns the current `IServiceProvider` available for this instance of `IApp`. Thanks to this property you should be able to load a service from anywhere in your application by calling `App.current.services`.
- Added `LangContext.GetLocalizedStringAsyncFunc` to delegate the localization operation to a user-defined function.
- Added `IInitializableComponent` interface to `IApp` interface. Now when initializing a MVVM `App` object, the related `AppBuilder` will call `app.InitializeComponent` before hosting.
- Added `SelectedLocaleListener` manipulator that reacts to Localization Package's SelectedLocale changes in order to provide a new `LangContext` in the visual tree.
- Added `NavHost.makeScreen` property. By setting your own callback to this property, you will be able to customize the way to instantiate a `NavigationScreen` when navigating to a new destination. This can be useful when coupled to Dependency Injection so you can retreive instances of your screens from a `IServiceProvider`. By default, the property is set with the use of `System.Activator.CreateInstance`.
- Added some warning messages (Standalone builds only) when a LocalizedTextElement value cannot be localized.

### Fixed

- Fixed notify property changes for Picker.selectedIndex.
- Fixed path resolution of Stylesheets coming from Packages in Icon Browser tool.
- Fixed a regression where components using `Pressable` manipulator will not able to be clicked more than once if the cursor doesn't leave the component's layout rect.
- Fixed NullReferenceException thrown when fetching Localization tables.
- Fixed Screen Height calculation. UITK does not use Camera rect but blit on the whole screen instead.
- Fixed `InvalidOperationException` thrown by the damping effect animation of the `Canvas` component.
- Fixed the calculation of off-screen items in SwipeView with vertical direction.

## [2.0.0-pre.9] - 2024-08-29

### Changed

- Scrollable manipulator now stops the propagation of WheelEvent. This affects only the Drawer and SwipeView elements.
- Refactored the SwipeView element logic, without impacting the public API.
- The Canvas now uses `Experimental.Animation` system from UI-Toolkit for its damping effect when releasing the mouse with some velocity. That replaces the previous implementation that was using the `VisualElementScheduledItem`.
- Changed fade in animation in Tooltip to use USS transitions.

### Fixed

- Fixed the wrap system of the SwipeView element when swiping between elements quickly.
- Fixed Daisy chaining window procedures on Windows platform.
- Fixed an edge case when popovers are dismissed as OutOfBounds as soon as Show() is called.
- Invoke click event only if Pressable is still hovered.
- Fixed styling of emphasized checkboxes.
- Fixed a bug where tooltips stop being shown when the window is docked/undocked.

### Added

- Added `justified` property on Tabs component to jusitfy tabs layout in horizontal direction.
- Added `damping-effect-duration` property in Canvas element. The default value is 750ms.
- Added `IsContextClick()` extension method for PointerEvent.
- Added support of graceful fallback to lambda `Plafform` implementation if native plugins can not be loaded by the current plaftorm.

## [2.0.0-pre.8] - 2024-08-12

### Changed

- Refactored completely the DropZone UI element. Now the DropZone doesn't embed any logic, but uses a `DropZoneController` instead. You can access this controller via `DropZone.controller` property and attaches a callback method to accept dragged objects and listens to drop events.
- Defer checking Popup's container candidate when the Popup is about to be shown, instead of during Popup creation.
- Refactored the styling of Chip UI element.
- Upgraded the old Drag And Drop Sample to use the refactored DropZone and the new Drag And Drop system.
- Changed the way MarkDirtyRepaint is scheduled on elements containing animated textures (check properly for visibility).
- Refactored the styling of DropZone UI element.

### Added

- Added the `Unity.AppUI.UI.DropZoneController` Manipulator for a lower level approach to create your own "drop zones".
- Added `--appui-splitview-splitter-anchor-size` design token.
- `Added Unity.AppUI.Core.DragAndDrop` class to handle drag and drop (in-panel and/or with the Editor). The support of external drag and drop at runtime is planned for future releases.
- Added `Unity.AppUI.UI.Chip.deletedWithEventInfo` and `Unity.AppUI.UI.Chip.clickedWithEventInfo` events.
- Added new Story in the Drag And Drop sample.
- Added support of `TextElement.displayTooltipWhenElided` to show elided text as a tooltip using the App UI tooltip system.

### Fixed

- Fixed compilation errors when the Unity project's Input Handling is set to `Both` or `New Input System` and the package `com.unity.inputsystem` is not installed.
- Fixed IL2CPP Compilation errors on Windows Platform due to non-static MonoPInvokeCallback.
- Fixed PInvoke delegate types on Windows platform.
- Fixed styling of SplitView's Splitter Anchor size.
- Fixed styling of BaseGridView when containing a single column.
- Popovers and Modals now correctly start checking for PointerDown events in the visual tree when they become visible.
- Removed console message when trying to add an Editor MonoBehaviour in the scene during PlayMode.

## [2.0.0-pre.7] - 2024-07-30

### Changed

- Changed DialogTrigger.keyboardDismissDisabled to DialogTrigger.keyboardDismissEnabled for consistency.
- Renamed Popup.parentView to Popup.containerView for more clarity.

### Added

- Added Modal.outsideClickDismissEnabled and Modal.outsideClickStrategy properties to support dismissing Modals by clicking outside of them.
- Added Popup<T>.SetContainerView method to set a custom container which will be the parent of the popup's view.
- Made `AnchorPopup.GetMovableElement` method public for easier access and increase customization possibilities.

### Fixed

- Fixed "Shape" icon.
- Fixed border color variable for AccordionItem.
- Fixed CultureInfo used during source code generation.

### Removed

- Removed intrusive Debug.Log calls from Platform class on Windows platform.
- Removed warning message when using Single selection type in an overflown ActionGroup.

## [2.0.0-pre.6] - 2024-07-07

### Added

- Added support of Attributes on fields and properties in the Dependency Injection system.
- Added support of UnityEditor ColorPicker in ColorField component.
- Added support of Color without alpha information in ColorField and ColorPicker.
- Added `rounded-progress-corners` boolean property in `CircularProgress` and `LinearProgress` to be able to disable rounded corners.
- Added `trailing-icon` property in `ActionButton` component.
- Added customization support for the size of the Color swatch inside the ColorField.
- Added arrow-square-in icon.

### Changed

- Changed the Text element inside the ColorField to become a selectable text.

### Fixed

- Fixed styling issues in `ActionButton` component.
- Fixed Menu's backdrop to block pointer events
- Fixed ColorField styling issues.
- Use correct color variables for Radio and Checkbox components

## [2.0.0-pre.5] - 2024-06-18

### Changed

- Complete rewrite of the SplitView component. The SplitView is no more a derived from TwoPaneSplitView from UI-Toolkit, but a full custom component that supports any number of panes.

### Fixed

- Fixed reset of Dropdown value when changing its source items.
- Fixed a visual bug where the checkmark symbol didn't appear on DropdownItem or MenuItem that have a `selected` state.

### Added

- Added `indicatorPosition` property in `AccordionItem` component, in order to swap the indicator position either at start or end of the heading row.
- Added `check` regular icon as a required icon in App UI themes.

## [2.0.0-pre.4] - 2024-06-05

### Fixed

- Fixed MacOS native plugin memory leak when opening the Help menu in the Editor.

## [2.0.0-pre.3] - 2024-05-30

### Added

- Added AsyncThunk support for Redux implementation.
- Added Anchor Position support for Toast UI elements.
- Added the `key` string property on `Radio` component to be used as unique identifier in their group.
- Added unit tests for MemoryUtils utility class.

### Removed

- Removed programatic construction of RadioGroup with IList object. Since App UI offers the possibility to have Radio component as deep as you want in the visual tree compared to its RadioGroup ancestor, we wanted to limit conflicts between construction kinds.

### Changed

- Moved Toast animation logic from code to USS.
- The `RadioGroup` component uses a `string` type for its `value` property. This string value is equal to the currently checked `Radio` component's `key` property.
- The `Toast.AddAction` method will now ask for a callback that takes a `Toast` object as argument (instead of no argument at all). This will give you an easier way to dismiss the toast from the action callback.
- You can now pass an `autoDismiss` argument to the `Toast.AddAction` method. This will automatically dismiss the toast when the action is triggered. This argument is optional and defaults to `true` for backward compatibility.
- Changed MemoryUtils.Concatenate implementation to not use variadic parameters and avoid implicit allocations.

### Fixed

- Fixed Action Dispatch to every Slice Reducers
- Every shared libraries of native plugins are now correctly signed with the correct Unity Technologies certificate (MacOS and Windows only)
- Fixed support of Radio component that are deeper than the direct child of a RadioGroup.

## [2.0.0-pre.2] - 2024-05-07

### Added

- Added `PinchGestureRecognizer` implementation for the new Gesture Recognizer System.
- Added an experimental method `Platform.GetSystemColor` to fetch color values defined by the Operating System for specific UI element types. This can be useful if you want to precisely follow the color palette of a high-contrast theme directed by the OS.
- Added "Icon Browser", a new Editor tool that enables users to generate UI-Toolkit stylesheets with a specific list of icons.
- Added a new experimental Gesture Recognizer System.
- Added the ability to subscribe and check if the current operating system is in Reduce-Motion Accessibility Mode (Windows/Mac/Android/iOS).
- Added the ability to subscribe and check if the current Text Scale Factor of the currently used window (Unity Player window or the Game view window in the Editor) (Windows/Mac/Android/iOS).
- Added the ability to subscribe and check if the current operating system is in High-Contrast Mode (Windows/Mac/Android/iOS).
- Added the ability to subscribe and check if the current operating system is in LeftToRight or RightToLeft layout direction (Windows/Mac/Android/iOS).
- Added the ability to subscribe and check if the current Scale Factor of the currently used window (Unity Player window or the Game view window in the Editor) (Windows/Mac/Android/iOS).
- Added the ability to subscribe and check if the current operating system is in Dark Mode (Windows/Mac/Android/iOS).

### Changed

- Refactored every native plugin provided by the package.
- Changed the Trackpad sample project to work properly with the new events coming from the new Gesture Recognizer System.

### Fixed

- Fixed meta files for native plugins on Windows platform.
- Fixed an early return in the PreProcessBuild callback of App UI when no persistent AppUISettings have been found.

## [2.0.0-pre.1] - 2024-03-25

### Added

- Added `DatePicker`, `DateRangePicker`, `DateField` and `DateRangeField` components. Theses components use the new `Date` and `DateRange` data structure also provided by App UI.
- Added `VisualElementExtensions.SetTooltipContent` method to populate a tooltip template with new content.
- Added `MasonryGridView` component.
- Added tests for Pan and Magnify gesture data structures.

### Removed

- TextFieldExtensions.BlinkingCursor extension method has become obsolete. Please use the new BlinkingCursor manipulator instead.

### Changed

- Replaced the MacOS native plugin by a `.dylib` library instead of a `.bundle` one.

### Fixed

- Fixed some namespace usage to avoid relative ones.

## [1.0.6] - 2024-03-15

### Fixed

- Fixed the tooltip tip size to not be displayed over tooltip content.
- Fixed the handling of Tab key to focus the next component from a TextArea
- Fixed size of the Radio button for pixel alignment on 96dpi screens.

## [1.0.5] - 2024-03-10

### Fixed

- Fixed Hero banner in the documentation homepage

## [1.0.4] - 2024-03-08

### Fixed

- Fixed GridFour icon PNG file
- Fixed font weight on AppBar title elements.
- Fixed accent color in Editor Dark theme
- Fixed ActionGroup Refresh strategy.

### Added

- Added gutter USS custom property for the GridView component.
- Added answer in the FAQ documentation about MacOS quarantine attribute.

## [1.0.3] - 2024-03-01

### Fixed

- Use equlity check while setting a new `text` value in a `LocalizedTextElement`.
- Fixed performance issues with the blinking text cursor in input fields.
- Fixed styling of the Toast Action container

## [1.0.2] - 2024-02-26

### Fixed

- Fixed the clipping flag on the GridView's ScrollView content container to prevent overscroll on touch devices.
- Fixed unit tests
- Disabled the dragging feature in the GridView when used on touch devices.

### Added

- Improved Localization Unity Package support with `LangContext` propagation.

## [1.0.1] - 2024-02-14

### Fixed

- Fixed ColorSwatch shader to support Shader Model older than 5.0

## [1.0.0] - 2024-02-12

### Changed

- Color related components, such as `ColorSlider` or `ColorSwatch`, support now a value of type `UnityEngine.Gradient` instead of `UnityEngine.Color` for more flexibility.
- Removed the `disabled` boolean property on App UI components from the public C# API. The `disabled` attribute in UXML has been replaced by `enabled` UXML attribute. This change has been made in order to be more consistent with Unity 2023.3, where `enabled` UXML attribute is provided by UI Toolkit on any VisualElement.
- Replaced `Nullable<T>` properties in components by a custom serializable implementation called `Optional<T>`
- Renamed `Panel.dir` property with `Panel.layoutDirection`.
- Replaced the `Divider.vertical` property by `Divider.direction` enum property.
- Removed the `ApplicationContext` class and `VisualElementExtensions.GetContext()` method, replaced by the ne `ProvideContext` and `RegisterContextChangedCallback` API.
- Replaced `ActionGroup.vertical` property by `ActionGroup.direction` enum property.
- Migrated code into the App UI monorepo at https://github.com/Unity-Technologies/unity-app-ui
- Renamed `format` UXML Attribute to `format-string` to bind correctly in UIBuilder
- Readjusted font sizes used on components to follow new design tokens
- Changed assembly definition files to support new UXML Serialization starting Unity 2023.3.0a1
- Removed `size` property from Checkbox and Toggle components.
- In Numerical fields and sliders, the string formatting for percentage values follows now the C# standard. The formatted string of the value `1` using `0P` or `0%` will give you `100%`. If you want the user to be able to type `100` in order to get a `100%` as a formatted string in a numerical field, we suggest to use `0\%` string format (be sure that the `highValue` of your field is set to `100` too).
- Previously the Panel Constructor set the default scale context to "large" on mobile platforms, now the default scale context for any platform is "medium".
- All user input related UI controls now inherit from the new IInputElement interface. This has no impact on the current implementation (some addition in certain components).
- The `MacroCommand.Flush` method will now call `UndoCommand.Flush` on its child commands from the most recent to the oldest one.
  This is the same order as the one used by the `MacroCommand.Undo` method.
- Most App UI component styling now use component-level design tokens for background and border colors
- Renamed CircularProgress and LinearProgress `color` Uxml Attribute to `color-override` to bind correctly in UIBuilder
- Runtime Tooltips won't be displayed if the picked element has `.is-open` USS class currently applied.
- Use the ConditionalWeakTable for new releases of Unity where a fix from IL2CPP has landed.
- Float Fields using a string format set on Percentage (P) will now use the same value as what you typed
- Use ConditionalWeakTable whenever possible (Editor and no IL2CPP builds)

### Added

- Added support of new UI Toolkit Runtime Bindings feature through bindable properties in each App UI components. More than 420 properties can now be bound (2023.2+).
- Added support to new UI Toolkit Uxml Serialization using source code generators.
- Added `BaseVisualElement` and `BaseTextElement` classes which are used as base class for mostly every App UI component
- Added numerous custom PropertyDrawers for a better experience in UIBuilder (2023.3+)
- Added Layout Direction `dir` context in the App UI `ApplicationContext`.
- Added `maxLength` property in TextArea and TextField components.
- Added `isPassword` and `maskChar` properties to TextField component.
- Added Editor-Dark and Editor-Light themes.
- Added others Inter font variants
- Added the support of `Fixed` gradient blend mode in ColorSwatch shader.
- Added `.appui-row` USS classes which support current layout direction context.
- Added new IInputElement interface in the public API
- Added missing API documentation
- Added new methods to push sub-menus in `MenuBuilder` class.
- Added monitoring of AccordionItem content size to make AccordionItem fit its content when it is already open.
- Added the possibility to setup navigation visual components from your NavigationScreen implementation directly. You are still able to create a global setup using your own implementation of the INavVisualController interface that you have set on your NavHost.
- Added `Spacer` component.
- Added the support of a specific drag direction and drag threshold in the Draggable manipulator. Specifying a direction will avoid to prevent scrolling in the opposite direction if this maniuplator is used inside a ScrollView for example.
- Added `submit-on-enter` property in `TextArea` component.
- Added more tests
- Added support of LTR and RTL layout direction in most App UI components
- Added `submit-modifiers` property in `TextArea` component.
- Added forceUseTooltipSystem property on the Panel component to force the use of App UI tooltips system regardless the state of UI-Toolkit default tooltip system. This can be useful in a context where UITK tooltips can't be displayed in the Editor.
- Added `submitted` event property in `TextArea` component.
- Added mention about full integration of UI Toolkit with the New Input System starting 2023.2 in the documentation
- Added `subMenuOpened` event property in `MenuItem` UI component.
- Added `accent` property to the `FloatingActionButton` component.
- Added `isReadonly` property to TextArea and TextField components.
- Added `isOpen` property setter to be able to set the open state of the `Drawer` component without animation.

### Removed

- Starting Unity 2023.3, App UI will not provide any `UxmlFactory` or `UxmlTraits` implementation, as they become obsolete and replaced by the new UITK Uxml serialization system.
- Removed Relocations folder from MacOS native plugin debug symbols bundle
- Removed the `margin` property from the Tray component.
- Removed the `size` property of the `Tray` component. The Tray component will fit its content in the screen automatically and doen't need anymore any specific size to be set.
- Removed the `expandable` property from the Tray component. A Tray component is should not be expandable by definition. If you are willing to have some scrollable content inside a Tray component, a new component called ScrollableTray will be avilable in future release of App UI.

### Fixed

- Fixed Unity crashes when polling TabItem elements inside Tabs component.
- Fixed Scroller styling.
- Fixed TextField and NumericalField text offset during FocusOut event
- Fixed auto scrolling to follow text edition's cursor in TextArea component.
- Fixed mouse capture during Pointer down event in Pressable manipulator for Unity versions older than 2023.1.
- Fixed tab key handling to switch focus in `TextArea` component.
- Fixed styling on BottomNavBar items
- Fixed Sliders handles to not exceed the range of track element.
- Fixed Selection handling in ActionGroup.
- Fixed styling for Disabled Quiet Button component.
- Fixed color blending in the ColorSwatch custom shader.
- Fixed ColorSwatch refresh when the Gradient reference value didn't change but Gradient keys have changed
- Fixed compilation errors while not having the new Input System installed but the Input Manager setting is set to 'New' in the Project Settings.
- Added support of Windows 11 for ARM
- Fixed layout for Bounds, Rect and Vector fields
- Fixed the height of Toast components to be `auto`.
- Fixed `AppBar` story in Storybook samples.
- Fixed UI Kit Sample with Progress components' color overrides.
- Fixed `border-radius` usage in `ExVisualElement` component.
- Fixed some styling issues on different components.
- Fixed USS icons to match with App UI Symbols names
- Cleaned up some warning messages to not get anything written in the console during package installation.
- Fixed small errors in UI Kit sample.
- Fixed a refresh bug in the App UI Storybook window.
- Fixed some examples in the UI Kit Sample
- Fixed refresh of the `ActionGroup` component
- Fixed MacOS Native plugin to support Intel 64bits architecture with IL2CPP
- Fixed typo in App UI Elevation's USS selector.
- Fixed box-shadows border-radius calculation
- Fixed border gap in TouchSlider component.
- Fixed margins on Checkbox component without label.
- Fixed ColorPicker Alpha channel slider refresh when the picked color has changed
- Fixed `isPrimaryActionDisabled` and `isSecondaryActionDisabled` property setters
- Fixed calls for ContextChanged callbacks registered by the context provider itself
- Fixed some unit tests
- Fixed Tooltip maximum size
- Fixed `MacroCommand.Undo` and `MacroCommand.Redo` methods.

## [1.0.0-pre.15] - 2024-02-09

### Removed

- Removed the `margin` property from the Tray component.
- Removed the `size` property of the `Tray` component. The Tray component will fit its content in the screen automatically and doen't need anymore any specific size to be set.
- Removed the `expandable` property from the Tray component. A Tray component is should not be expandable by definition. If you are willing to have some scrollable content inside a Tray component, a new component called ScrollableTray will be avilable in future release of App UI.

### Fixed

- Fixed styling on BottomNavBar items
- Fixed refresh of the `ActionGroup` component
- Fixed Tooltip maximum size

### Added

- Added monitoring of AccordionItem content size to make AccordionItem fit its content when it is already open.

### Changed

- Use the ConditionalWeakTable for new releases of Unity where a fix from IL2CPP has landed.

## [1.0.0-pre.14] - 2024-01-30

### Fixed

- Fixed ColorSwatch refresh when the Gradient reference value didn't change but Gradient keys have changed
- Added support of Windows 11 for ARM
- Fixed ColorPicker Alpha channel slider refresh when the picked color has changed
- Fixed calls for ContextChanged callbacks registered by the context provider itself

### Added

- Added new IInputElement interface in the public API
- Added the possibility to setup navigation visual components from your NavigationScreen implementation directly. You are still able to create a global setup using your own implementation of the INavVisualController interface that you have set on your NavHost.
- Added the support of a specific drag direction and drag threshold in the Draggable manipulator. Specifying a direction will avoid to prevent scrolling in the opposite direction if this maniuplator is used inside a ScrollView for example.
- Added forceUseTooltipSystem property on the Panel component to force the use of App UI tooltips system regardless the state of UI-Toolkit default tooltip system. This can be useful in a context where UITK tooltips can't be displayed in the Editor.
- Added mention about full integration of UI Toolkit with the New Input System starting 2023.2 in the documentation

### Changed

- All user input related UI controls now inherit from the new IInputElement interface. This has no impact on the current implementation (some addition in certain components).
- Use ConditionalWeakTable whenever possible (Editor and no IL2CPP builds)

## [1.0.0-pre.13] - 2024-01-14

### Removed

- Removed Relocations folder from MacOS native plugin debug symbols bundle

### Fixed

- Fixed TextField and NumericalField text offset during FocusOut event

### Added

- Added missing API documentation
- Added more tests

### Changed

- Changed assembly definition files to support new UXML Serialization starting Unity 2023.3.0a1

## [1.0.0-pre.12] - 2024-01-03

### Changed

- Color related components, such as `ColorSlider` or `ColorSwatch`, support now a value of type `UnityEngine.Gradient` instead of `UnityEngine.Color` for more flexibility.
- Removed the `disabled` boolean property on App UI components from the public C# API. The `disabled` attribute in UXML has been replaced by `enabled` UXML attribute. This change has been made in order to be more consistent with Unity 2023.3, where `enabled` UXML attribute is provided by UI Toolkit on any VisualElement.
- Replaced `Nullable<T>` properties in components by a custom serializable implementation called `Optional<T>`
- Renamed `Panel.dir` property with `Panel.layoutDirection`.
- Replaced the `Divider.vertical` property by `Divider.direction` enum property.
- Removed the `ApplicationContext` class and `VisualElementExtensions.GetContext()` method, replaced by the ne `ProvideContext` and `RegisterContextChangedCallback` API.
- Replaced `ActionGroup.vertical` property by `ActionGroup.direction` enum property.

### Added

- Added support of new UI Toolkit Runtime Bindings feature through bindable properties in each App UI components. More than 420 properties can now be bound (2023.2+).
- Added support to new UI Toolkit Uxml Serialization using source code generators.
- Added `BaseVisualElement` and `BaseTextElement` classes which are used as base class for mostly every App UI component
- Added numerous custom PropertyDrawers for a better experience in UIBuilder (2023.3+)
- Added the support of `Fixed` gradient blend mode in ColorSwatch shader.
- Added `.appui-row` USS classes which support current layout direction context.

### Removed

- Starting Unity 2023.3, App UI will not provide any `UxmlFactory` or `UxmlTraits` implementation, as they become obsolete and replaced by the new UITK Uxml serialization system.

### Fixed

- Fixed Sliders handles to not exceed the range of track element.
- Fixed color blending in the ColorSwatch custom shader.
- Fixed `AppBar` story in Storybook samples.
- Fixed some styling issues on different components.
- Cleaned up some warning messages to not get anything written in the console during package installation.
- Fixed small errors in UI Kit sample.
- Fixed a refresh bug in the App UI Storybook window.

## [1.0.0-pre.11] - 2023-12-21

### Fixed

- Fixed Scroller styling.
- Fixed styling for Disabled Quiet Button component.
- Fixed compilation errors while not having the new Input System installed but the Input Manager setting is set to 'New' in the Project Settings.
- Fixed border gap in TouchSlider component.
- Fixed margins on Checkbox component without label.

### Changed

- Removed `size` property from Checkbox and Toggle components.

## [1.0.0-pre.10] - 2023-12-12

### Fixed

- Fixed auto scrolling to follow text edition's cursor in TextArea component.
- Fixed layout for Bounds, Rect and Vector fields
- Fixed some examples in the UI Kit Sample

### Added

- Added `maxLength` property in TextArea and TextField components.
- Added `isPassword` and `maskChar` properties to TextField component.
- Added Editor-Dark and Editor-Light themes.
- Added `isReadonly` property to TextArea and TextField components.

### Changed

- Readjusted font sizes used on components to follow new design tokens
- Most App UI component styling now use component-level design tokens for background and border colors

## [1.0.0-pre.9] - 2023-12-05

### Fixed

- Fixed Selection handling in ActionGroup.
- Fixed MacOS Native plugin to support Intel 64bits architecture with IL2CPP

### Changed

- In Numerical fields and sliders, the string formatting for percentage values follows now the C# standard. The formatted string of the value `1` using `0P` or `0%` will give you `100%`. If you want the user to be able to type `100` in order to get a `100%` as a formatted string in a numerical field, we suggest to use `0\%` string format (be sure that the `highValue` of your field is set to `100` too).

### Added

- Added `isOpen` property setter to be able to set the open state of the `Drawer` component without animation.

## [1.0.0-pre.8] - 2023-11-30

### Fixed

- Fixed Unity crashes when polling TabItem elements inside Tabs component.
- Fixed mouse capture during Pointer down event in Pressable manipulator for Unity versions older than 2023.1.
- Fixed tab key handling to switch focus in `TextArea` component.
- Fixed UI Kit Sample with Progress components' color overrides.
- Fixed `border-radius` usage in `ExVisualElement` component.
- Fixed typo in App UI Elevation's USS selector.
- Fixed box-shadows border-radius calculation
- Fixed `isPrimaryActionDisabled` and `isSecondaryActionDisabled` property setters

### Added

- Added new methods to push sub-menus in `MenuBuilder` class.
- Added `submit-on-enter` property in `TextArea` component.
- Added `submit-modifiers` property in `TextArea` component.
- Added `submitted` event property in `TextArea` component.
- Added `subMenuOpened` event property in `MenuItem` UI component.
- Added `accent` property to the `FloatingActionButton` component.

### Changed

- Runtime Tooltips won't be displayed if the picked element has `.is-open` USS class currently applied.

## [1.0.0-pre.7] - 2023-11-24

### Added

- Added others Inter font variants

### Fixed

- Fixed some unit tests

## [1.0.0-pre.6] - 2023-11-19

### Added

- Added Layout Direction `dir` context in the App UI `ApplicationContext`.
- Added support of LTR and RTL layout direction in most App UI components

### Changed

- Previously the Panel Constructor set the default scale context to "large" on mobile platforms, now the default scale context for any platform is "medium".

## [1.0.0-pre.5] - 2023-11-15

### Changed

- Migrated code into the App UI monorepo at https://github.com/Unity-Technologies/unity-app-ui
- Renamed `format` UXML Attribute to `format-string` to bind correctly in UIBuilder
- The `MacroCommand.Flush` method will now call `UndoCommand.Flush` on its child commands from the most recent to the oldest one.
  This is the same order as the one used by the `MacroCommand.Undo` method.
- Renamed CircularProgress and LinearProgress `color` Uxml Attribute to `color-override` to bind correctly in UIBuilder
- Float Fields using a string format set on Percentage (P) will now use the same value as what you typed

### Added

- Added `disabled` property on every components that needed it in order to bind correctly with UIBuilder
- Added `Spacer` component.

### Fixed

- Fixed the height of Toast components to be `auto`.
- Fixed `MacroCommand.Undo` and `MacroCommand.Redo` methods.

## [1.0.0-pre.4] - 2023-11-08

### Changed

- The `Canvas` component now listens to `PointerDownEvent` in `TrickleDown` phase to avoid to miss the event when the pointer is over a child element.

## [1.0.0-pre.3] - 2023-11-06

### Added

- Added public access to `Pressable.InvokePressed` and `Pressable.InvokeLongPressed` methods.
- Added `trailingContainer` property to the `AccordionItem` component.

### Fixed

- Fixed warning message about Z-axis scale in the `AnchorPopup` component.
- Fixed styling of the `AccordionItem` header component.
- Fixed the `not-allowed` (disabled) cursor texture for Windows support at Runtime.
- Fixed images in the package documentation.

### Changed

- The `Pressable` manipulator now uses the `keepPropagation` property also in its `PointerUpEvent` callback to avoid to propagate the event to the parent element.

## [1.0.0-pre.2] - 2023-11-04

### Added

- Added missing XML documentation parts for the public API of the package.

## [1.0.0-pre.1] - 2023-10-31

### Added

- Added debug symbols for native plugins.

### Changed

- Moved internal Engine API related code into a new assembly `Unity.AppUI.InternalAPIBridge`.

## [0.6.5] - 2023-11-04

### Added

- Added missing XML documentation parts for the public API of the package.
- Added the `primaryManipulator` property to the `Canvas` component to get the primary pointer manipulator used by the canvas when no modifier is pressed.
- Added the `autoResize` property to the `TextArea` component to grow the text area when the text overflows.
- Added the handling of double-click in the `TextArea` resizing handle to re-enable the auto-resize feature.
- Added the `outsideScrollEnabled` property to the `AnchorPopup` components to enable or disable the outside scroll handling. The default value is `false`.
- Added a smoother animation for the `AnchorPopup` components when the popup is about to be shown.

### Fixed

- Fixed the Canvas background to support light theme.
- Fixed the styling of DropdownItem checkmark.
- Fixed the styling of MenuItem checkmark.
- Fixed the styling of Slider components.
- Fixed the styling of TextArea component.
- Fixed the styling of TouchSlider components.
- Fixed the previous value sent in `ChangeEvent` of `NumericalField`, `VectorField` and `Picker` (Dropdown) components.
- Improved the synchronization of the `AnchorPopup` components to refresh their position in the layout faster.

### Changed

- Changed the USS selector for component-level aliases to use directly `:root` selector instead of `<component_main_uss_class>` selector.

## [0.6.4] - 2023-10-31

### Fixed

- Fixed `NullReferenceException` when testing accept drag in `GridView` component.

## [0.6.3] - 2023-10-31

### Added

- Added `getItemId` property to the `GridView` component to get the id of an item.

### Fixed

- Fixed items selection persistence between refreshes in `GridView` component.

## [0.6.2] - 2023-10-27

### Added

- Added `closeOnSelection` property to the `MenuTrigger` component.
- Added `closeOnSelection` property to the `ActionGroup` component to close the popover menu (if any) when an item is selected.

### Fixed

- Fixed `closeOnSelection` property in `MenuBuilder` component.
- Fixed `TextInput` styling for Unity 2023.1+
- Fixed `InvalidCastException` at startup of santandlone builds (in both Mono and IL2CPP)
- Fixed `NullReferenceException` when dismissing a Popup from a destroyed UI-Toolkit panel.
- Fixed sub-menu indicator icon in `MenuItem` component.
- Fixed context click handling in `GridView` component.

## [0.6.1] - 2023-10-20

### Added

- Added `allowNoSelection` property to the `GridView` component to enable or disable the selection of no items.

### Fixed

- Fixed the support of nested components inside the `Button` component.
- Fixed the reset of the previous selection when `GridView.SetSelectionWithoutNotify` method is called.

## [0.6.0] - 2023-09-20

### Added

- Added the `Quote` component.
- Added `FieldLabel` component.
- Added `HelpText` component.
- Added `AvatarGroup` component.
- Added `required` property to the `InputLabel` component.
- Added `indicatorType` property to the `InputLabel` component.
- Added `helpMessage` property to the `InputLabel` component.
- Added `helpVariant` property to the `InputLabel` component.
- Added `variant` property to the `Avatar` component to support `square`, `rounded` and `circular` variants.

### Changed

- Added the `Button` `variant` property and removed the `primary` property.
- Updated App UI icons with a default size of 256x256 and moved into a folder named `Regular`.
- The `InputLabel` component uses the `FieldLabel` and `HelpText` components to display the label and the help text.
- The `Avatar` component now listens to `AvatarVarianContext` and `SizeContext` changes to update the variant and size of the avatar.

### Removed

- Removed the `size` property from the `InputLabel` component.

## [0.5.5] - 2023-10-27

### Added

- [Backport] Added `closeOnSelection` property to the `MenuTrigger` component.
- [Backport] Added `closeOnSelection` property to the `ActionGroup` component to close the popover menu (if any) when an item is selected.

### Fixed

- [Backport] Fixed `closeOnSelection` property in `MenuBuilder` component.
- [Backport] Fixed `TextInput` styling for Unity 2023.1+
- [Backport] Fixed `InvalidCastException` at startup of santandlone builds (in both Mono and IL2CPP)
- [Backport] Fixed `NullReferenceException` when dismissing a Popup from a destroyed UI-Toolkit panel.
- [Backport] Fixed context click handling in `GridView` component.

## [0.5.4] - 2023-10-19

### Added

- Added `allowNoSelection` property to the `GridView` component to enable or disable the selection of no items.

### Fixed

- Fixed the support of nested components inside the `Button` component.
- Fixed the reset of the previous selection when `GridView.SetSelectionWithoutNotify` method is called.

## [0.5.3] - 2023-10-16

### Added

- Added `useSpaceBar` property in `Canvas` component to enable or disable the handling of the space bar key.

### Fixed

- Removed the handling of the space bar key in the `Canvas` component when the used control scheme is `Editor`.

## [0.5.2] - 2023-10-12

### Added

- Added `acceptDrag` property to the `GridView` component to enable or disable the drag and drop feature.
- Added `menu` icon.
- Added `AddDivider` and `AddSection` methods to the `MenuBuilder` component.

### Fixed

- Fixed handling `acceptDrag` property in `Dragger` manipulator.
- Fixed MenuItem opening sub menus when the item is disabled.
- Fixed mipmap limit for Icons when a global limit is set in the Quality settings of the project.
- Fixed the capture of pointer during PointerDown event in `Dragger` manipulator. Now the pointer is captured only if the `Dragger` manipulator is active (i.e. the threshold has been reached).

## [0.5.1] - 2023-09-20

### Added

- Added `selectedIndex` property to the `Picker` component to get or set the selected index for a single selection mode.
- Added a new `Enumerable` extension method called `GetFirst` to get the first item of an enumerable collection or a default value if the collection is empty.
- Added `isPrimaryActionDisabled` and `isSecondaryActionDisabled` properties to the `AlertDialog` component.
- Added styling for `AlertDialog` component semantic variants.

### Fixed

- Fixed the `Picker` component to avoid to select multiple items if the selection mode is set to `Single`.
- Fixed `Menu.CloseSubMenus` method to close sub-menus opened by `MenuItem` components contained inside `MenuSection` components.
- Fixed double click handling in `GridView` component.

### Changed

- Changed the picking mode of the `DropdownItem` component to `Position` instead of `Ignore`.

## [0.5.0] - 2023-09-15

### Added

- Added `outsideClickStrategy` property in `AnchorPopup` component.
- Added `DropZone` component.
- Added `DropTarget` manipulator.
- Added a sample for `DropZone` and `DropTarget` components usage.
- Added `--border-style` custom USS property to support `dotted` and `dashed` border styles.
- Added `--border-speed` custom USS property to animate the border style.
- Added `Picker` component.
- Added `closeOnSelection` property for `Picker` and `Dropdown` components to close the picker when an item is selected.

### Fixed

- Fixed the `Pressable` manipulator to only handle the Left Mouse Button by default.
- Fixed current tooltip element check in `TooltipManipulator`.
- Fixed incremental value when interacting with keyboard in `ColorSlider` component.
- Fixed clearing selection in `GridView` when a user clicks outside of the grid.
- Fixed `itemsChosen` event in `GridView` to be fired only when the selection is not empty.
- Fixed Navigation keys handling in the `GridView` to clamp the selection to the grid items.

### Changed

- The `Pressable` manipulator nows inherits from `PointerManipulator` instead of `Manipulator`.
- Changed the `GridView.GetIndexByPosition` method to use a world-space position instead of a local-space position and renamed it to `GetIndexByWorldPosition`.
- TouchSlider component will now loose focus when a slide interaction has ended.
- When calling `GridView.Reset()` method, the selection won't be restored if no custom `GridView.getItemId` function has been provided.
- When using `--box-shadow-type: 1` (inset box-shadow), the `--box-shadow-spread` value was interpreted with the same direction as outset box-shadow. This has been fixed so you can use a positive spread value to go inside the element and a negative spread value to go outside the element.
- The `Dropdown` component inherits from `Picker` component. Users will be able to create custom dropdown-like components by inheriting from `Picker` component.
- The `Dropdown` component now has a selection mode property to choose between single and multiple selection modes.
- The `Dropdown` component now uses `DropdownItem` component instead of `MenuItem` component.
- Removed `default-value` UXML property from `Dropdown` component. The preferred way to set the default value is by code.

## [0.4.2] - 2023-09-01

### Added

- Added `FrameWorldRect` method to the `Canvas` component.
- Added `frameContainer` property to the `Canvas` component.

### Fixed

- Fixed the soft selection handling on `PointerUpEvent` in the `GridView` component.

### Added

- Added the `Picker` component.

### Changed

- Changed `Dropdown` implementation to use the new `Picker` component.

## [0.4.1] - 2023-08-24

### Added

- Added mouse and touchpad mapping presets to the `Canvas` component.
- Added a setting to enable or disable gestures recognition on MacOS.
- Added selection mode in `ActionGroup` component.

## [0.4.0] - 2023-08-18

### Added

- Added the **Small** scale context to App UI. This context is now the preferred one for desktop platforms.
- Added `RangeSlider` component.

### Fixed

- Fixed `ValueChanged` and `ValueChanging` events propagation in `NumericalField` and Vector components.

## [0.3.9] - 2023-08-17

### Added

- Added Context API which is accessible through any `VisualElement` instance.
- Added `preventScrollWithModifiers` property to the `GridView` component.

### Fixed

- Fixed `autoPlayDuration` property on PageView component.
- Fixed value validation in `NumericalField` component.

## [0.3.8] - 2023-08-08

### Added

- Added `tooltip-delay-ms` property to the `ContextProvider` component to customize the tooltip delay.
- Added more shortcuts to the `Canvas` component.

### Fixed

- Fixed Editor crash when updating packages from UPM window.

## [0.3.7] - 2023-08-07

### Added

- Added `tooltipTemplate` property (by code only) on `VisualElement` to customize the tooltip content.

### Fixed

- Fixed Canvas manipulators for mouse devices and generic touchpads.
- Fixed Tooltip manipulator coordinates to pick elements under the cursor.
- Fixed Tooltip display for Editor context.
- Fixed random crashes when switching focused window.

## [0.3.6] - 2023-07-28

### Added

- Added `autoPlayDuration` property to the `PageView` component.
- Added `variant` property to the `IconButton` component.
- Added `Canvas` component.
- Added `GridView` component.

### Changed

- Changed the calibration values for Magic Trackpad gestures recognition.

### Fixed

- Fixed horizontal ScrollView styling.

## [0.3.5] - 2023-07-24

### Fixed

- Regenerated the meta files inside MacOS bundle folder to avoid error messages in the console.

## [0.3.4] - 2023-07-17

### Changed

- Draggable manipulator now is publicly accessible.

### Fixed

- Ensure shaders exist before creating materials.
- Fixed random crashes during domain unload in Unity 2022.3+.
- Fixed cursors variables for Editor context.

## [0.3.3] - 2023-07-06

### Added

- Added Magic Trackpad gesture support for MacOS.
- Added `PanGesture` and `MagnificationGesture` events for UITK dispatcher.

### Fixed

- Fixed `Pressable` post-processing for disabled targets.
- Fixed visual content rendering synchronization in `Progress` and `ExVisualElement` components.
- Fixed Animation Recycling in `NavHost` component.

## [0.3.2] - 2023-06-01

### Fixed

- Fixed NavAction being added twice in NavGraph when deleting a linked NavDestination.

## [0.3.1] - 2023-05-17

### Fixed

- Fixed warning messages about styling.
- Fixed warning messages about GUID conflicts in UI Kit samples.
- Fixed warning messages about unused events in NavController.

## [0.3.0] - 2023-05-17

### Added

- Added defaultMessage property on Dropdown component.
- Added ChangingEvent on TextField, TextArea and NumericalField components.
- Added the AppBar navigation component.
- Added the variant property on the Icon component.
- Added the support of Phosphor Icons through the `com.unity.replica.phosphor-icons` package.
- Added the BottomNavBar component.
- Added the FAB component.
- Added the DrawerHeader component.
- Added the NavHost component.
- Added a ListViewItem component.
- Added the Navigation System under the `Unity.AppUI.Navigation` namespace (in its own **non auto-referenced assembly**).
- Added the support of nested Navigation graphs.
- Added the INavVisualController interface to control the content of the navigation components.
- Added the NavController, core component of the navigation system.
- Added a sample called Navigation to demonstrate the navigation system.
- Added the support of Enum properties in the Storybook editor window.
- Added ObservableObject class to implement the INotifyPropertyChanged interface.
- Host and App interfaces to create MVVM apps based on specific hosts.
- Added UITK implementations of the Host and App interfaces.
- Added Dependency Injection support for MVVM Toolkit via constructor injection.
- Added an implementation of a ServiceProvider for MVVM Toolkit.
- Added an App Builder utility MonoBehaviour to create MVVM apps as Unity component in a scene.
- Added RelayCommand and RelayCommand<T> classes to implement the ICommand interface.
- Added AsyncRelayCommand and AsyncRelayCommand<T> classes to implement the ICommand interface.
- Added a sample project to show how to use MVVM Toolkit with App UI.
- Added the MVVM implementation under the `Unity.AppUI.MVVM` namespace (in its own **non auto-referenced assembly**).
- Added a Redux implementation under the `Unity.AppUI.Redux` namespace (in its own **non auto-referenced assembly**).
- Added the UndoRedo system under the `Unity.AppUI.Undo` namespace (in its own **non auto-referenced assembly**).

### Changed

- The namespaces used by the package **has changed** from `UnityEngine.Dt.AppUI` to `Unity.AppUI`. See the migration guide in the documentation for more information.
- `Header` component is now named `Heading`. The old name is still supported but will be removed in a future release.
- CircularProgress innerRadius property is now publicly accessible.
- App UI main Looper instance is now publicly accessible.
- Improved StackView logic to support the navigation system.
- Refactored Avatar component, there are no more containers for notifications and icons.
- Refactored Badge component.

## [0.2.13] - 2023-06-28

### Fixed

- Fixed RenderTextures destruction to avoid memory leaks.

## [0.2.12] - 2023-06-01

### Added

- Added `isExclusive` property to the `Accordion` component.
- Added `shortcut` property to the `MenuItem` component.

### Fixed

- Fixed TextArea input size.
- Fixed ValueChanged events on text based inputs.
- Fixed depth tests in custom shaders for WebGL platform.

## [0.2.11] - 2023-05-12

### Added

- Added `maxDistance` property to the World-Space UI Document component.

## [0.2.10] - 2023-05-08

### Added

- Added resistance property to the `SwipeView` component.

### Fixed

- Fixed `Pressable` PointerDown event propagation.

## [0.2.9] - 2023-05-04

### Changed

- Removed `Replica` word from the documentation.

## [0.2.8] - 2023-04-30

### Added

- Added transition animations to sliders components.
- Added a `swipeable` property to the `SwipeView` component to be able to disable swipe interaction.
- Added `Preloader` component to the UI Kit sample.
- Added `Link` component.
- Added `Breadcrumbs` component to the UI Kit sample.
- Added `Toolbar` component to the UI Kit sample.

### Fixed

- Fixed `Pressable` event propagation.

### Changed

- Updated USS vars to use the new version of App UI Design Tokens.

## [0.2.7] - 2023-04-24

### Added

- Added `Refresh` method to the `Dropdown` component.
- Added AutoPlay to the `SwipeView` component.

### Fixed

- Fixed the support of the New Input System.
- Fixed the box-shadow shader with portrait aspect ratio.
- Fixed async operations on LocalizedTextElement.

## [0.2.6] - 2023-04-17

### Added

- Added `innerRadius` property to the `CircularProgress` component.

### Fixed

- Fixed the RenderTexture format of the World-Space UI Kit sample.

## [0.2.5] - 2023-03-30

### Added

- Added `SnapTo` and `GoTo` methods in the SwipeView component.
- Added `startSwipeThreshold` property in the SwipeView component.
- Added Scrollable Manipulator which uses a threshold before beginning to scroll.
- Added Pressable Manipulator which can be used to capture a pointer during press but can continue propagation of Move and Up events.

### Fixed

- Removed the use of System.Linq in the App UI package.

### Changed

- SwipeView now uses the Scrollable Manipulator instead of Draggable.

## [0.2.4] - 2023-03-23

### Fixed

- Fixed Localization initialization for WebGL platform.
- Fixed ScrollView tracker styling.
- Fixed Focusable property in constructor for newer versions of Unity.
- Fixed UnityEngine namespace import for newer versions of Unity.

## [0.2.3] - 2023-03-22

### Fixed

- Fixed styling on Scroller.
- Fixed max-height on Dropdown menu.
- Fixed dismissible dialogs in UI Kit sample.
- Fixed flex-shrink value on multiple components.
- Fixed TextField UXML construction.

## [0.2.2] - 2023-03-20

### Fixed

- Fixed NullReferenceException in invalid AnchorPopup updates.

## [0.2.1] - 2023-03-07

### Fixed

- Fixed the localization of the Dropdown component.
- Fixed the sizing of Progress UI components.
- Fixed OutOfBounds dismiss on cascading Popovers.
- Fixed the blocking of placeholder events on TextField and TextArea.
- Fixed compilation error on 2022.2.0a8.

## [0.2.0] - 2023-03-05

### Added

- Added support of Native features such as system themes and scale.
  The support has been done for Android, iOS, MacOS, and Windows.
- Added support of the [Unity Localization package](https://docs.unity3d.com/Packages/com.unity.localization@1.4/manual/index.html).
  You can localize strings from the Application Context or use a
  [LocalizedTextElement](xref:Unity.AppUI.UI.LocalizedTextElement) component to localize strings from the UI.
- Added a global UI component property `preferred-tooltip-position` to set the default tooltip position.
  The property is available by code and UXML.
- Added a App UI manager with a singleton pattern to manage the App UI configuration and lifecycle.
- Added an option in the App UI Settings to auto-override the Android manifest file (Android builds only).
- Added an option in the App UI Settings to enable or disable the usage of a custom loop frequency (Editor only).
- Added a World Space UI Document component to display UI elements in the world space.
- Added Avatar UI component to display an avatar with a name and a status.
- Added a Badge UI component to display a badge with a number.
- Added a BoundsField UI component to define a three-dimensional bounding box.
- Added a BoundsIntField UI component to define a three-dimensional bounding box with integer values.
- Added a Chip UI component to display a chip with a label and an icon.
- Added a CircularProgress UI component to display a circular progress bar.
- Added a LinearProgress UI component to display a linear progress bar.
- Added a ColorField UI component to define a color.
- Added a ColorSlider UI component to choose a value from a range of colors.
- Added a ColorSwatch UI component to display a color.
- Added a ColorWheel UI component to choose a hue from a color wheel.
- Added a ColorPicker UI component to choose a color from a color wheel and a color slider.
- Added a DoubleField UI component to define a double value.
- Added a Drawer UI component to display additional content from the sides of the screen.
- Added the support of `box-shadow` and `outline` using custom USS properties (see [ExVisualElement](xref:Unity.AppUI.UI.ExVisualElement)).
- Added a IconButton UI component to display an icon button.
- Added a LocalizedTextElement UI component to display a localized text.
  Most of the App UI components have been updated to use this component.
- Added a LongField UI component to define a long value.
- Added a Mask UI component to fill an area with a solid color.
- Added a PageIndicator UI component to display dots in a pagination system.
- Added a RectField UI component to define a two-dimensional rectangular area.
- Added a RectIntField UI component to define a two-dimensional rectangular area with integer values.
- Added a TextArea UI component to display a scrollable text area.
- Added the support of expression evaluation in numeric fields (see [ExpressionEvaluator](xref:UnityEditor.ExpressionEvaluator)).
- Added an [ActionTriggered UITK event](xref:Unity.AppUI.UI.ActionTriggeredEvent) that can be triggered by Menu items.
- Added a StackView UI component to display and animate a stack of items.
- Added a SwipeView UI component to display a list of items that can be swiped in a direction.
- Added a PageView UI component which is the combination of a SwipeView and a PageIndicator.
- Added the `Submittable` UI-Toolkit manipulator to handle the submission of Action UI elements via keyboard/mouse/pointer.
- Added the [KeyboardFocusController](xref:Unity.AppUI.UI.KeyboardFocusController)
  UI-Toolkit manipulator to differentiate the focus of a UI element from the keyboard or the pointer.
- Added a MenuBuilder class to create a Menu from code.
- Added the ability for Popover element to use a modal backdrop (Pointer events are blocked).
- Added a simple implementation of a [Storybook](xref:storybook)-like tool to display and test UI components.

### Changed

- The **Application** UI element is now called [Panel](xref:Unity.AppUI.UI.Panel).
- Improved the Slider UI component to display the current value.
- Improved the Tray UI element to be resizable.
- Ability to use the Modal component with any content derived from [VisualElement](xref:UnityEngine.UIElements.VisualElement).
- The App UI main Looper is now part of the App UI manager and is
  not present in the Application UI element anymore.

### Fixed

- Fixed the Notification system when the Notificiation UI element has been destroyed without being dismissed.
- Fixed the Menu system to handle sub-menus.
- Fixed the position calculator of Popups.
- Fixed the Popup system to be able to dismiss a popup when clicking outside of it in an area that is not handled by UI-Toolkit.
- Fixed the formatting of numerical fields.
- Fixed the handle of some edge-cases in TooltipManipulator.

## [0.1.0] - 2022-08-19

### Added

- Package Structure
- First draft of User Manual documentation
- Accordion UI Component
- ActionBar UI Component
- ActionButton UI Component
- ActionGroup UI Component
- Button UI Component
- Checkbox UI Component
- Divider UI Component
- Dropdown UI Component
- NumericalField UI Components
- Header UI Component
- Icon UI Component
- Radio UI Component
- Slider UI Component
- Stepper UI Component
- Tabs UI Component
- Text UI Component
- TextField UI Component
- Toggle UI Component
- TouchSlider UI Component
- VectorField UI Components
- Dialogs & Alerts UI System
- Menu UI System
- Popup UI System
- Notification Manager & Toasts
- Message Queue System (Looper & Handler)
- ContextProvider System
- Tooltip System
