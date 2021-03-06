﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ChangeTracking.Specialized;
using SharpTestsEx;

namespace Radical.Tests.Model
{
    [TestClass()]
    public class CollectionClearedDescriptorTest
    {
        [TestMethod]
        public void collectionClearedDescriptor_ctor_normal_should_set_expected_values()
        {
            var items = new[]
            {
                new GenericParameterHelper(),
                new GenericParameterHelper(),
                new GenericParameterHelper()
            };

            var target = new CollectionRangeDescriptor<GenericParameterHelper>(items);

            target.Items.Should().Have.SameSequenceAs(items);
        }
    }
}
