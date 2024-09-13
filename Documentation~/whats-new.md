---
uid: whats-new
---

# What's New

This section contains information about new features, improvements, and issues fixed.

For a complete list of changes made, refer to the **Changelog** page.

The main updates in this release include:

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

