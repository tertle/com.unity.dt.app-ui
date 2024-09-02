using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Preloader))]
    class PreloaderTests : VisualElementTests<Preloader>
    {
        protected override string mainUssClassName => Preloader.ussClassName;
    }
}
