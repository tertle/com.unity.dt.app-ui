---
uid: whats-new
---

# What's New

This section contains information about new features, improvements, and issues fixed.

For a complete list of changes made, refer to the **Changelog** page.

The main updates in this release include:

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

