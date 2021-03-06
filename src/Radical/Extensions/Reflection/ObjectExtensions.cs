﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Radical.Reflection
{
    /// <summary>
    /// Defines a delegate.
    /// </summary>
    /// <returns>A value.</returns>
    public delegate object Function();

    /// <summary>
    /// Object extensions for Fast property getters based on Lambda Expressions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Creates a fast property getter.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="target">The target instance that expose the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A delegate to get the property value.</returns>
        public static Func<T> CreateFastPropertyGetter<T>(this object target, string propertyName)
        {
            return target.CreateFastPropertyGetter<T>(target.GetType().GetProperty(propertyName));
        }

        /// <summary>
        /// Creates a fast property getter.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="target">The target instance that expose the property.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// A delegate to get the property value.
        /// </returns>
        public static Func<T> CreateFastPropertyGetter<T>(this object target, PropertyInfo property)
        {
            var targetExp = Expression.Constant(target);
            var propExp = Expression.Property(targetExp, property);

            var funcExp = Expression.Lambda<Func<T>>(propExp);
            var func = funcExp.Compile();

            return func;
        }

        /// <summary>
        /// Creates a fast property getter.
        /// </summary>
        /// <param name="target">The target instance that expose the property.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// A delegate to get the property value.
        /// </returns>
        public static Function CreateFastPropertyGetter(this object target, PropertyInfo property)
        {
            var targetExp = Expression.Constant(target);
            var propExp = Expression.Property(targetExp, property);

            var funcExp = Expression.Lambda<Function>(propExp);
            var func = funcExp.Compile();

            return func;
        }
    }
}
