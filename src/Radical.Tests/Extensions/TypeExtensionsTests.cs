﻿namespace Radical.Tests.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Radical.Reflection;
    using SharpTestsEx;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_is_non_generic_using_same_types_should_return_true()
        {
            var actual = typeof(object).Is(typeof(object));
            actual.Should().Be.True();
        }

        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_is_non_generic_using_valid_types_should_return_true()
        {
            var actual = typeof(int).Is(typeof(object));
            actual.Should().Be.True();
        }

        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_is_non_generic_using_non_inherited_types_should_return_false()
        {
            var actual = typeof(int).Is(typeof(string));
            actual.Should().Be.False();
        }

        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_getInheritanceChain_using_valid_type_should_return_expected_inheritance_data()
        {
            IEnumerable<Type> chain = Radical.Reflection.TypeExtensions.GetInheritanceChain(typeof(string));
            chain.Should().Have.SameSequenceAs(new Type[] { typeof(string), typeof(object) });
        }

        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_getInheritanceChain_using_valid_type_and_valid_stopper_should_return_expected_inheritance_data()
        {
            IEnumerable<Type> chain = Radical.Reflection.TypeExtensions.GetInheritanceChain(typeof(string), t => t != typeof(string));
            chain.Should().Have.SameSequenceAs(new Type[] { typeof(string) });
        }

        class Root { }

        class DescendantA : Root { }

        class DescendantB : Root { }

        [TestMethod]
        [TestCategory("TypeExtensions")]
        public void TypeExtensions_getDescendants_using_valid_type_should_return_expected_descendants()
        {
            IEnumerable<Type> descendants = Radical.Reflection.TypeExtensions.GetDescendants(typeof(Root));
            descendants.Should().Have.SameSequenceAs(new Type[] { typeof(Root), typeof(DescendantA), typeof(DescendantB) });
        }
    }
}
