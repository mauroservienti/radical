﻿using System;
using System.Linq.Expressions;

namespace Radical.Validation
{
    /// <summary>
    /// Ensure is a simple, fluent based, engine useful to validate
    /// methods and constructors parameters.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to validate.</typeparam>
    public sealed class Ensure<T> : IConfigurableEnsure<T>, IEnsure<T>
    {
        private readonly T inspectedObject;
        readonly Ensure.SourceInfo si;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ensure&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="obj">The value of the parameter to validate.</param>
        /// <param name="si">The source info.</param>
        internal Ensure(T obj, Ensure.SourceInfo si)
        {
            inspectedObject = obj;
            this.si = si;
        }

        /// <summary>
        /// Gets the currently inspected object value.
        /// </summary>
        /// <returns>The currently inspected object value.</returns>
        public T GetValue()
        {
            return inspectedObject;
        }

        /// <summary>
        /// Gets the currently inspected object value casted to specified type.
        /// </summary>
        /// <typeparam name="K">The type to cast the inspected object to, K must inherit from T.</typeparam>
        /// <returns>The currently inspected object value.</returns>
        public K GetValue<K>() where K : T
        {
            return (K)inspectedObject;
        }

        /// <summary>
        /// Identifies the name of the parameter that will be validated.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The Ensure instance for fluent interface usage.</returns>
        public IEnsure<T> Named(string parameterName)
        {
            _name = parameterName;
            return this;
        }

        /// <summary>
        /// Identifies the name of the parameter that will be validated.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The Ensure instance for fluent interface usage.</returns>
        public IEnsure<T> Named(Expression<Func<T>> parameterName)
        {
            var expression = parameterName.Body as MemberExpression;
            if (expression == null)
            {
                throw new NotSupportedException("Only MemberExpression(s) are supported.");
            }

            nameExpression = parameterName;
            _name = null;

            return this;
        }

        Expression<Func<T>> nameExpression;
        string _name;
        /// <summary>
        /// Gets or sets the name of the parameter to validate.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name) && nameExpression != null)
                {
                    var expression = (MemberExpression)nameExpression.Body;
                    _name = expression.Member.Name;
                }

                return _name;
            }
        }

        /// <summary>
        /// Specifies the custom user message to be used when raising exceptions.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>This ensure instance for fluent interface usage.</returns>
        public IEnsure<T> WithMessage(string errorMessage)
        {
            UserErrorMessage = errorMessage;
            return this;
        }

        /// <summary>
        /// Specifies the custom user message to be used when raising exceptions.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="formatArgs">The format arguments.</param>
        /// <returns>
        /// This ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> WithMessage(string errorMessage, params object[] formatArgs)
        {
            UserErrorMessage = string.Format(errorMessage, formatArgs);
            return this;
        }

        /// <summary>
        /// Gets the user custom error message.
        /// </summary>
        /// <value>The error message.</value>
        public string UserErrorMessage
        {
            get;
            private set;
        }

        /*
         * {0} --> NewLine
         * {1} --> class name
         * {2} --> source type
         * {3} --> method name
         * {4} --> user error message (if any)
         * {5} --> <validatorSpecificMessage>
         * {6} --> parameter name (if any)
         */
        const string fullErrorMessageFormat = "Ensure validation failure.{0}{0}" +
                                              "   {5}{0}" +
                                              "   Location: class {1} at {3} {2}.{0}" +
                                              "   Name: '{6}'{0}" +
                                              "   Caller supplied informations: {4}{0}";

        /// <summary>
        /// Gets the full error message.
        /// </summary>
        /// <param name="validatorSpecificMessage">The validator specific message.</param>
        /// <returns>The error message.</returns>
        public string GetFullErrorMessage(string validatorSpecificMessage)
        {
            var fullErrorMessage = string.Format
            (
                fullErrorMessageFormat,
                Environment.NewLine,
                si.ClassName,
                si.SourceType.ToString().ToLower(),
                si.MethodName,
                UserErrorMessage ?? "--",
                validatorSpecificMessage,
                Name ?? "<no-name-supplied>"
            );

            return fullErrorMessage;
        }

        /// <summary>
        /// Gets the full error message.
        /// </summary>
        /// <returns>The error message.</returns>
        public string GetFullErrorMessage()
        {
            return GetFullErrorMessage(string.Empty);
        }

        /// <summary>
        /// Gets the value of the validated parameter.
        /// </summary>
        /// <value>The value of the parameter.</value>
        public T Value
        {
            get { return inspectedObject; }
        }

        bool state = false;

        /// <summary>
        /// Execute the given predicate and saves the result for later usage.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate in order to establish if the operation result is <c>true</c> or <c>false</c>.</param>
        /// <returns>The Ensure instance for fluent interface usage.</returns>
        public IEnsure<T> If(Predicate<T> predicate)
        {
            state = predicate(inspectedObject);
            return this;
        }

        /// <summary>
        /// Executes the specified action only if the <c>If</c> operation has been evaluated to <c>true</c>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The Ensure instance for fluent interface usage.</returns>
        public IEnsure<T> Then(Action<T> action)
        {
            if (state)
            {
                action(inspectedObject);
            }

            return this;
        }

        /// <summary>
        /// Executes the specified action only if the <c>If</c> operation has been evaluated to <c>false</c>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> Else(Action<T> action)
        {
            if (!state)
            {
                action(inspectedObject);
            }

            return this;
        }

        /// <summary>
        /// Executes the specified action only if the <c>If</c> operation has been evaluated to <c>true</c>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> Then(Action<T, string> action)
        {
            if (state)
            {
                action(inspectedObject, Name);
            }

            return this;
        }

        /// <summary>
        /// Executes the specified action only if the <c>If</c> operation has been evaluated to <c>false</c>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> Else(Action<T, string> action)
        {
            if (!state)
            {
                action(inspectedObject, Name);
            }

            return this;
        }

        /// <summary>
        /// Ensure that the supplied predicate returns true.
        /// </summary>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        /// <exception cref="ArgumentException">An ArgumentException is raised if the predicate result is false.</exception>
        public IEnsure<T> IsTrue(Predicate<T> func)
        {
            return If(func).Else(obj =>
         {
             Throw(new ArgumentException(Name, GetFullErrorMessage("The supplied condition is not met, condition was expected to be true.")));
         });
        }

        /// <summary>
        /// Ensure that the supplied predicate returns false.
        /// </summary>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        /// <exception cref="ArgumentException">An ArgumentException is raised if the predicate result is true.</exception>
        public IEnsure<T> IsFalse(Predicate<T> func)
        {
            return If(func).ThenThrow(v =>
         {
             return new ArgumentException(v.GetFullErrorMessage("The supplied condition is not met, condition was expected to be false."), v.Name);
         });
        }

        /// <summary>
        /// Ensure that the supplied object is equal to the currently inspected object.
        /// </summary>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        /// <exception cref="ArgumentException">An ArgumentException is raised if the object equality fails.</exception>
        public IEnsure<T> Is(T value)
        {
            if (!Equals(inspectedObject, value))
            {
                Throw(new ArgumentException(GetFullErrorMessage("The currently inspected value is not equal to the supplied value."), Name));
            }

            return this;
        }

        /// <summary>
        /// Ensure that the supplied object is not equal to the currently inspected object.
        /// </summary>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        /// <exception cref="ArgumentException">An ArgumentException is raised if the object equality does not fail.</exception>
        public IEnsure<T> IsNot(T value)
        {
            if (Equals(inspectedObject, value))
            {
                Throw(new ArgumentException(GetFullErrorMessage("The currently inspected value should be different from to the supplied value."), Name));
            }

            return this;
        }

        /// <summary>
        /// Throws the specified error if the previous If check has returned true.
        /// </summary>
        /// <param name="builder">The exception builder.</param>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> ThenThrow(Func<IEnsure<T>, Exception> builder)
        {
            if (state)
            {
                var exception = builder(this);
                Throw(exception);
            }

            return this;
        }

        Action<IEnsure<T>, Exception> validationFailurePreview;

        /// <summary>
        /// Allows the user to intercept the ensure failure before the exception is raised.
        /// </summary>
        /// <param name="validationFailurePreview">The validation failure preview handler.</param>
        /// <returns>
        /// The Ensure instance for fluent interface usage.
        /// </returns>
        public IEnsure<T> WithPreview(Action<IEnsure<T>, Exception> validationFailurePreview)
        {
            this.validationFailurePreview = validationFailurePreview;

            return this;
        }

        /// <summary>
        /// Throws the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void Throw(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            validationFailurePreview?.Invoke(this, error);

            throw error;
        }
    }
}
