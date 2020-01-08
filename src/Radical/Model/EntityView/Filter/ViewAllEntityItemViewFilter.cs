namespace Radical.Model
{
    using Radical.ComponentModel;
    using Radical.Validation;
    using System;

    /// <summary>
    /// This filter is a include all filter, the <collection>ShouldInclude( T )</collection> method always returns true.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public sealed class ViewAllEntityItemViewFilter<T> : EntityItemViewFilterBase<T>// where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAllEntityItemViewFilter&lt;T&gt;"/> class.
        /// </summary>
        private ViewAllEntityItemViewFilter()
        {

        }


        /// <summary>
        /// Gets a item that indicates if the given object instance should be included in the result set of the filter operation..
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>
        ///     <collection>True</collection> if the item should be included, otherwise <collection>false</collection>.
        /// </returns>
        public override bool ShouldInclude(T item)
        {
            Ensure.That(item)
                .Named(() => item)
                .If(i => Object.ReferenceEquals(i, null))
                .ThenThrow(e => new ArgumentNullException(e.GetFullErrorMessage()));

            return true;
        }



        private readonly static ViewAllEntityItemViewFilter<T> _instance = new ViewAllEntityItemViewFilter<T>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IEntityItemViewFilter Instance
        {
            get { return _instance; }
        }


        /// <summary>
        /// Returns a <see cref="T:System.string"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.string"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Resources.Labels.ViewAllFilterName;
        }
    }
}
