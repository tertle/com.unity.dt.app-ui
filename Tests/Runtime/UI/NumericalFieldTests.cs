using System;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    class NumericalFieldTests<T, U> : VisualElementTests<T>
        where T : NumericalField<U>, new()
        where U : struct, IComparable, IComparable<U>, IFormattable
    {

    }
}
