using NUnit.Framework;
using Unity.AppUI.Navigation;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(NavHost))]
    class NavHostTests : VisualElementTests<NavHost>
    {
        protected override string mainUssClassName => NavHost.ussClassName;

        protected override string uxmlNamespaceName => "nav";
    }
}
