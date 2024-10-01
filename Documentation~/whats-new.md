---
uid: whats-new
---

# What's New

This section contains information about new features, improvements, and issues fixed.

For a complete list of changes made, refer to the **Changelog** page.

The main updates in this release include:

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

