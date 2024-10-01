using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.MVVM;
using UnityEngine;

namespace Unity.AppUI.Tests.MVVM
{
    [TestFixture]
    [TestOf(typeof(ObservableObject))]
    class ObservableObjectTests
    {
        [Test]
        public void CanSetProperties()
        {
            var obj = new Observable();

            obj.PropertyChanged += (sender, args) => { };
            obj.PropertyChanging += (sender, args) => { };

            obj.V1 = 1;
            Assert.AreEqual(1, obj.V1);
            obj.V1 = 1;
            Assert.AreEqual(1, obj.V1);

            obj.V2 = new Observable.ValueType { Value = 2 };
            Assert.AreEqual(2, obj.V2.Value);
            obj.V2 = new Observable.ValueType { Value = 2 };
            Assert.AreEqual(2, obj.V2.Value);

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V2WithNoComparer = new Observable.ValueType { Value = 2 };
            });

            obj.V3 = 3;
            Assert.AreEqual(3, obj.V3);
            obj.V3 = 3;
            Assert.AreEqual(3, obj.V3);

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V3WithNoCallback = 3;
            });

            obj.V4 = new Observable.ValueType { Value = 4 };
            Assert.AreEqual(4, obj.V4.Value);
            obj.V4 = new Observable.ValueType { Value = 4 };
            Assert.AreEqual(4, obj.V4.Value);

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V4WithNoCallback = new Observable.ValueType { Value = 4 };
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V4WithNoComparer = new Observable.ValueType { Value = 4 };
            });

            obj.V5 = 5;
            Assert.AreEqual(5, obj.V5);
            obj.V5 = 5;
            Assert.AreEqual(5, obj.V5);

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V5WithNoCallback = 5;
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                obj.V5WithNoModel = 5;
            });
        }

#if ENABLE_RUNTIME_DATA_BINDINGS
        [Test]
        public void CanNotifyWithUIToolkitRuntimeDataBindings()
        {
            var obj = new Observable();
            var changed = false;
            obj.propertyChanged += (sender, args) => {  changed = true; };

            Assert.AreEqual(0, obj.GetViewHashCode());
            obj.V1 = 1;
            Assert.IsTrue(changed);

            Assert.AreEqual(0, obj.GetViewHashCode(), "View hash code should not change if we did not call Publish");
            obj.Publish();
            Assert.AreEqual(1, obj.GetViewHashCode(), "View hash code should change after calling Publish");
        }
#endif

        public class Observable : ObservableObject
#if ENABLE_RUNTIME_DATA_BINDINGS
            , UnityEngine.UIElements.IDataSourceViewHashProvider
#endif
        {
            int m_Value1;

            ValueType m_Value2 = new ValueType();

            int m_Value3;

            ValueType m_Value4 = new ValueType();

            public int V1
            {
                get => m_Value1;
                set => SetProperty(ref m_Value1, value);
            }

            public ValueType V2
            {
                get => m_Value2;
                set => SetProperty(ref (m_Value2), value, value);
            }

            public ValueType V2WithNoComparer
            {
                get => m_Value2;
                set => SetProperty(ref (m_Value2), value, (EqualityComparer<ValueType>)null);
            }

            public int V3
            {
                get => m_Value3;
                set => SetProperty(m_Value3, value, OnV3Changed);
            }

            public int V3WithNoCallback
            {
                get => m_Value3;
                set => SetProperty(m_Value3, value, null);
            }

            public ValueType V4
            {
                get => m_Value4;
                set => SetProperty(m_Value4, value, value, OnV4Changed);
            }

            public ValueType V4WithNoCallback
            {
                get => m_Value4;
                set => SetProperty(m_Value4, value, value, null);
            }

            public ValueType V4WithNoComparer
            {
                get => m_Value4;
                set => SetProperty(m_Value4, value, null, OnV4Changed);
            }

            void OnV3Changed(int newValue)
            {
                m_Value3 = newValue;
            }

            void OnV4Changed(ValueType newValue)
            {
                m_Value4 = newValue;
            }

            public int V5
            {
                get => m_Value4.Value;
                set => SetProperty(m_Value4.Value, value, m_Value4, (vtype, v) => vtype.Value = v);
            }

            public int V5WithNoCallback
            {
                get => m_Value4.Value;
                set => SetProperty<int, ValueType>(m_Value4.Value, value, m_Value4, null);
            }

            public int V5WithNoModel
            {
                get => m_Value4.Value;
                set => SetProperty<int, ValueType>(m_Value4.Value, value, null, (vtype, v) => vtype.Value = v);
            }

            long m_ViewVersion = 0;

            public void Publish()
            {
                m_ViewVersion++;
            }

            public long GetViewHashCode()
            {
                return m_ViewVersion;
            }

            public class ValueType : EqualityComparer<ValueType>
            {
                public int Value { get; set; }

                public override bool Equals(ValueType x, ValueType y)
                {
                    if (ReferenceEquals(x, y))
                        return true;

                    if (ReferenceEquals(x, null))
                        return false;

                    if (ReferenceEquals(y, null))
                        return false;

                    if (x.GetType() != y.GetType())
                        return false;

                    return x.Value == y.Value;
                }

                public override int GetHashCode(ValueType obj)
                {
                    return obj.Value;
                }
            }
        }
    }
}
