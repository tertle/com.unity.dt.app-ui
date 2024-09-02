using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(SearchBar))]
    class SearchBarTests : VisualElementTests<SearchBar>
    {
        protected override string mainUssClassName => SearchBar.ussClassName;
    }
}
