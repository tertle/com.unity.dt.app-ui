using System;
using System.Collections.Generic;
using NUnit.Framework;
using EnumField = Unity.AppUI.UI.EnumField;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(EnumField))]
    class EnumFieldTests : VisualElementTests<EnumField>
    {
        protected override string mainUssClassName => EnumField.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", _ => new EnumField(EnumForTest.Value1));
            }
        }

        protected override bool uxmlConstructable => true;

        protected override IEnumerable<string> uxmlTestCases
        {
            get
            {
                yield return "<appui:EnumField />";
                yield return @"<appui:EnumField enum-type=""Unity.AppUI.Tests.UI.EnumForTest, Unity.AppUI.Tests"" enum-value=""Value1"" />";
                yield return @"<appui:EnumField enum-type=""Unity.AppUI.Tests.UI.EnumForTest, Unity.AppUI.Tests"" enum-value=""Value2"" />";
            }
        }
    }

    enum EnumForTest
    {
        Value1,
        Value2,
        Value3
    }
}
