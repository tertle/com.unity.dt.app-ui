---
uid: redux-sample
---

# Redux Sample

App UI provides a Redux sample project that
demonstrates how to use this pattern with this Unity package and will give you ideas about how to manage states
inside your own project with Redux.

## Getting Started

### Installation

To use the Redux sample, you will need to have this package installed in your project.

To install the package, follow the instructions in the [Installation and Setup](xref:setup)
section of the documentation.

Inside the Unity Package Manager window, select the **App UI** package, then
go to **Samples** and select **Redux**. Click **Install** to install the sample.

### Usage

To open the sample, in your Project panel go to
**Assets > Samples > App UI > Redux > Scenes** and open the **Redux** scene.

The scene contains non-graphical elements, but just a **ReduxSample** component that
is attached to a **Redux** game object. This component contains logic to manage the state of a **counter**.

If you go in Play mode, you will see in the Console some log entries about Redux actions and state changes.

We suggest you to familiarize yourself with the sample code **ReduxSample.cs** and the [State Management documentation](xref:state-management).