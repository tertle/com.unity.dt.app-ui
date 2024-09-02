using System;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2Field = Unity.AppUI.UI.Vector2Field;
using Vector3Field = Unity.AppUI.UI.Vector3Field;
using Vector4Field = Unity.AppUI.UI.Vector4Field;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    class Vector2FieldTests : VectorFieldTests<Vector2Field, Vector2>
    {
        protected override string mainUssClassName => Vector2Field.ussClassName;
    }

    [TestFixture]
    class Vector3FieldTests : VectorFieldTests<Vector3Field, Vector3>
    {
        protected override string mainUssClassName => Vector3Field.ussClassName;
    }

    [TestFixture]
    class Vector4FieldTests : VectorFieldTests<Vector4Field, Vector4>
    {
        protected override string mainUssClassName => Vector4Field.ussClassName;
    }

    class VectorFieldTests<T, U> : VisualElementTests<T>
        where T : VisualElement, new()
        where U : struct, IEquatable<U>, IFormattable
    {

    }
}
