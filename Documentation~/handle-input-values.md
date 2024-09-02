---
uid: handle-input-values
---

# Input Values

In this section we will cover how to handle values from input UI components. This includes value observation, validation, and formatting.

## Observation

In App UI Unity Package, input-related UI components expose two types of events: ChangingEvent and ChangeEvent.
These events are used to notify listeners when the value of the input component is being changed and when the value has finished changing, respectively.

### ChangingEvent

The `ChangingEvent` is triggered when the user is interacting with the input component, such as dragging a slider or typing in a text field.
This event is fired continuously as the user interacts with the component, providing real-time feedback on the value changes.
It allows you to perform any necessary actions or updates while the user is still interacting with the component.

### ChangeEvent

The `ChangeEvent` is triggered when the user has finished interacting with the input component, or when the value has been changed via the public API directly.
This event is fired only once when the value has stabilized, providing a final notification of the value change.
It allows you to perform any necessary actions or updates after the user has made their selection or after the value has been changed programmatically.

By using the `ChangingEvent` and `ChangeEvent` in your input-related UI components, you can efficiently manage and respond to user interactions and value changes.

## Validation

In App UI Unity Package, input-related UI components provide a validation delegate method that allows you to validate the new value before it is applied to the component.
This validation mechanism enables you to enforce specific rules or constraints on the input values and ensure that only valid values are accepted by the UI component.

### Validation Delegate

The validation delegate method is a function that takes the new value as an argument and returns a boolean value indicating whether the new value is valid or not.
If the validation function returns `true`, the new value will be accepted and applied to the UI component.
If it returns `false`, the new value will be rejected, and the UI component will retain its previous value.

The validation delegate method is provided as a property called `validateValue` in the input-related UI components.

You can set this property to your custom validation method to perform value validation.

### Handling Invalid Values

If the validation delegate method returns false, indicating that the new value is invalid,
the UI component will retain its previous value, and no change will be applied.
You can also handle invalid values in your own way by displaying an error message or taking other actions as needed.

By using the validation delegate method, you can ensure that your input-related UI components only accept valid values.

## Formatting

The formatting of value occurs when the value is being displayed in the UI component.
Like the validation delegate method, formatting can be set via the `formatString` property in the input-related UI components.
Note that only certain UI components support formatting, such as
[numeric fields](xref:Unity.AppUI.UI.NumericalField`1.formatString) and
[sliders](xref:Unity.AppUI.UI.BaseSlider`2.formatString).