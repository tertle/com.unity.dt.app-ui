using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(AssetTargetField))]
    class AssetTargetFieldTests : VisualElementTests<AssetTargetField>
    {
        protected override string mainUssClassName => AssetTargetField.ussClassName;
    }
}
