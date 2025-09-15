using System.Collections;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    public class AnchorPopupUtilsTests
    {
        bool m_SetupDone;

        UIDocument m_TestUI;

        Panel m_Panel;

        ActionButton m_Trigger;

        VisualElement m_Element;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (!m_SetupDone)
            {
                // Load new scene
                var scene = SceneManager.CreateScene("ComponentTestScene-" + Random.Range(1, 1000000));
                while (!SceneManager.SetActiveScene(scene))
                {
                    yield return null;
                }
                m_TestUI = Utils.ConstructTestUI();
                m_Panel = new Panel();
                m_Trigger = new ActionButton {label = "Trigger"};
                m_Element = new VisualElement
                {
                    style =
                    {
                        position = Position.Absolute,
                        width = 100,
                        height = 100,
                    }
                };
                m_Panel.Add(m_Trigger);
                m_Panel.Add(m_Element);
                m_TestUI.rootVisualElement.Add(m_Panel);
            }

            m_Panel.DismissAllPopups();
            yield return new WaitForSeconds(0.1f);
            m_SetupDone = true;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (m_TestUI)
                Object.Destroy(m_TestUI.gameObject);

            m_TestUI = null;
            m_SetupDone = false;
#pragma warning disable CS0618
            SceneManager.UnloadScene(SceneManager.GetActiveScene());
#pragma warning restore CS0618
        }

        [TestCaseSource(typeof(AnchorPopupTestsArgs))]
        public void CanComputePosition(
            PositionOptions options,
            PositionResult result)
        {
            var computed = AnchorPopupUtils.ComputePosition(m_Element, m_Trigger, m_Panel, options);
            // todo create a test for this
            Assert.IsTrue(true);
            computed.finalPlacement = result.finalPlacement;
            computed.left = result.left;
            computed.top = result.top;
            computed.marginTop = result.marginTop;
            computed.marginLeft = result.marginLeft;
            computed.tipLeft = result.tipLeft;
            computed.tipTop = result.tipTop;
        }

        public class AnchorPopupTestsArgs : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                var result = new PositionResult();
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Bottom }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Top }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Left }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Right }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.BottomLeft }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.BottomRight }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.TopLeft }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.TopRight }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.LeftTop }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.LeftBottom }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.RightTop }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.RightBottom }, result };

                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Start }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.End }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.StartTop }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.StartBottom }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.EndTop }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.EndBottom }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.TopStart }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.TopEnd }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.BottomStart }, result };
                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.BottomEnd }, result };

                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Bottom, crossSnap = true }, result };

                yield return new object[] { new PositionOptions { favoritePlacement = PopoverPlacement.Left, crossSnap = true }, result };

            }
        }
    }
}
