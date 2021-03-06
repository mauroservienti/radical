﻿using Radical.Validation;
using System;

namespace Radical.Conversions
{
    /// <summary>
    /// Adds behaviors, in the cast/as space, to the base object class.
    /// </summary>
    public static class CastExtensions
    {
        /// <summary>
        /// Casts the source object to the given type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The source object.</param>
        /// <returns>The casted object.</returns>
        [Obsolete("CastTo<> has been obsoleted and will be removed in v3.0.0")]
        public static TResult CastTo<TResult>(this object obj)
        {
            return (TResult)obj;
        }

        /// <summary>
        /// Performs a safe cast, using the as operator, of the source object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The source object.</param>
        /// <returns>The casted object or null.</returns>
        [Obsolete("As<> has been obsoleted and will be removed in v3.0.0")]
        public static TResult As<TResult>(this object obj)
            where TResult : class
        {
            return As<TResult>(obj, r => { }, () => { });
        }

        /// <summary>
        /// Performs a safe cast, using the as operator, of the source object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The source object.</param>
        /// <param name="invalidCastAction">The invalid cast action.</param>
        /// <returns>The casted object or null.</returns>
        [Obsolete("As<> has been obsoleted and will be removed in v3.0.0")]
        public static TResult As<TResult>(this object obj, Action invalidCastAction)
            where TResult : class
        {
            return As<TResult>(obj, r => { }, invalidCastAction);
        }

        /// <summary>
        /// Performs a safe cast, using the as operator, of the source object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The source object.</param>
        /// <param name="validCastAction">The valid cast action.</param>
        /// <returns>The casted object or null.</returns>
        [Obsolete("As<> has been obsoleted and will be removed in v3.0.0")]
        public static TResult As<TResult>(this object obj, Action<TResult> validCastAction)
            where TResult : class
        {
            return As<TResult>(obj, validCastAction, () => { });
        }

        /// <summary>
        /// Performs a safe cast, using the as operator, of the source object.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The source object.</param>
        /// <param name="validCastAction">The valid cast action.</param>
        /// <param name="invalidCastAction">The invalid cast action.</param>
        /// <returns>The casted object or null.</returns>
        [Obsolete("As<> has been obsoleted and will be removed in v3.0.0")]
        public static TResult As<TResult>(this object obj, Action<TResult> validCastAction, Action invalidCastAction)
            where TResult : class
        {
            Ensure.That(validCastAction).Named("validCastAction").IsNotNull();
            Ensure.That(invalidCastAction).Named("invalidCastAction").IsNotNull();

            if (obj == null)
            {
                return null;
            }

            return (obj as TResult).If
            (
                o => o != null,
                o => validCastAction(o),
                o => invalidCastAction()
            );
        }
    }
}
