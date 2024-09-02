---
uid: whats-new
---

# What's New

This section contains information about new features, improvements, and issues fixed.

For a complete list of changes made, refer to the **Changelog** page.

The main updates in this release include:

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

