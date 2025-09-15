using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Redux;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    [TestOf(typeof(PartitionedState))]
    class PartitionedStateTests
    {
        record SliceState { }

        [Test]
        public void CanSetAndGetSliceState()
        {
            var state = new PartitionedState();
            PartitionedState newState = null;
            Assert.Throws<ArgumentException>(() => _ = state.Set<SliceState>(null, null));
            Assert.DoesNotThrow(() => newState = state.Set("slice1", new SliceState()));
            Assert.IsFalse(ReferenceEquals(state, newState), "Setting a slice should return a new state because it is immutable");
            Assert.IsNotNull(newState.Get<SliceState>("slice1"));
            Assert.Throws<KeyNotFoundException>(() => _ = newState.Get<SliceState>("slice2"));
            Assert.Throws<ArgumentException>(() => _ = newState.Get<SliceState>(null));
        }
    }
}
