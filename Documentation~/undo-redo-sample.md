---
uid: undo-redo-sample
---

# Undo/Redo Sample

This sample project demonstrates how to use the undo/redo pattern.

## Getting Started

### Installation

To use the Undo/Redo sample, you will need to have this package installed in your project.

To install the package, follow the instructions in the [Installation and Setup](xref:setup)
section of the documentation.

Inside the Unity Package Manager window, select the **App UI** package, then
go to **Samples** and select **UndoRedo**. Click **Install** to install the sample.

### Usage

The sample contains one Unity scene with the following UI:
- An Undo button
- A Redo button
- A TextField component to change the result text
- A ColorField component to change the result color
- A label to display the result
- A ListView which contains the history of the changes that can be undone/redone

You can play with the TextField and ColorField components to change the result text and color. You
will notice that realtime changes in the ColorPicker are not added directly to the history. Instead,
the changes are added to the history when the the color picker is closed.

Same thing for the TextField component. The changes are added to the history when the text field
loses focus.

The Undo and Redo buttons are disabled when there are no changes to undo or redo.
