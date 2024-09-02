using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(DialogTrigger))]
    class DialogTriggerTests : VisualElementTests<DialogTrigger>
    {
        protected override string mainUssClassName => null;
    }
}
