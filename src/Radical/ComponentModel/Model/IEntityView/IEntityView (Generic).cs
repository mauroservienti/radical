﻿using Radical.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Radical.ComponentModel
{
    /// <summary>
    /// Extends the <see cref="IEntityView"/> interface by providing
    /// advanced features. This <c>IEntityView</c> is capable of managing custom properties added at runtime
    /// in order to build, for example, runtime evaluated columns dynamically.
    /// </summary>
    /// <typeparam name="T">The type (System.Type) of objects managed by this collection view.</typeparam>
    public interface IEntityView<T> :
        IEntityView,
        IEnumerable<IEntityItemView<T>>
    //where T : class
    {
        /// <summary>
        /// Adds a property mapping using the specified display name, the supplied property type and the supplied getter and setter.
        /// </summary>
        /// <param name="calculatedPropertyDisplayName">The display name of the new property.</param>
        /// <param name="getter">A delegate to call in order to get the value of the dynamically generated property.</param>
        /// <param name="setter">A delegate to call in order to set the value of the dynamically generated property.</param>
        /// <returns>A reference to the dynamically generated property.</returns>
        EntityItemViewPropertyDescriptor<T> AddCustomProperty<TProperty>(
            string calculatedPropertyDisplayName,
            EntityItemViewValueGetter<T, TProperty> getter,
            EntityItemViewValueSetter<T, TProperty> setter);

        /// <summary>
        /// Adds a property mapping using the specified display name, the supplied property type and the supplied getter.
        /// </summary>
        /// <param name="calculatedPropertyDisplayName">Calculated name of the property display.</param>
        /// <param name="getter">A delegate to call in order to get the value of the dynamically generated property.</param>
        /// <returns>A reference to the dynamically generated property.</returns>
        /// <remarks>Using this overload implicitly creates a read-only property because no setter has been supplied.</remarks>
        EntityItemViewPropertyDescriptor<T> AddCustomProperty<TProperty>(string calculatedPropertyDisplayName,
            EntityItemViewValueGetter<T, TProperty> getter);

        EntityItemViewPropertyDescriptor<T> AddCustomProperty<TProperty>(string calculatedPropertyDisplayName,
            EntityItemViewValueGetter<T, TProperty> getter,
            Func<TProperty> defaultValueInterceptor);

        /// <summary>
        /// Adds a property mapping that maps a property using the supplied display name.
        /// </summary>
        /// <param name="propertyName">Name of the property to map to.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns>A reference to the dynamically generated property.</returns>
        EntityItemViewPropertyDescriptor<T> AddCustomProperty(string propertyName, string displayName);

        /// <summary>
        /// Adds a property mapping using the supplied property descriptor instance.
        /// </summary>
        /// <param name="customProperty">The custom property descriptor to use as mapping.</param>
        /// <returns>A reference to the dynamically generated property.</returns>
        EntityItemViewPropertyDescriptor<T> AddCustomProperty(EntityItemViewPropertyDescriptor<T> customProperty);

        /// <summary>
        /// Adds a property mapping that maps the supplied property.
        /// </summary>
        /// <param name="propertyName">Name of the property to map to.</param>
        /// <returns>A reference to the dynamically generated property.</returns>
        EntityItemViewPropertyDescriptor<T> AddCustomProperty(string propertyName);

        bool IsCustomPropertyDefined(string propertyName);

        /// <summary>
        /// Removes the property mapping.
        /// </summary>
        /// <param name="customProperty">The custom property.</param>
        /// <returns><c>True</c> if the operation was successful, otherwise <c>false</c>.</returns>
        bool RemoveCustomProperty(EntityItemViewPropertyDescriptor<T> customProperty);

        /// <summary>
        /// Removes the property mapping.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>True</c> if the operation was successful, otherwise <c>false</c>.</returns>
        bool RemoveCustomProperty(string propertyName);

        /// <summary>
        /// Gets or sets a value indicating whether this instance must auto generate property mappings.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance must auto generate property mappings; otherwise, <c>false</c>.
        /// </value>
        bool AutoGenerateProperties { get; set; }

        /// <summary>
        /// Gets the all the dynamically added custom property mappings.
        /// </summary>
        /// <returns>A read-only list of dynamically added property mappings.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<EntityItemViewPropertyDescriptor<T>> GetCustomProperties();

        EntityItemViewPropertyDescriptor<T> GetCustomProperty(string name);

        PropertyDescriptor GetProperty(string name);

        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the collection of items returned by the data source
        /// </summary>
        /// <returns>The filter used to filter out in the item collection returned by the data source. </returns>
        new IEntityItemViewFilter<T> Filter { get; set; }

        /// <summary>
        /// Moves the specified item to the specified new index.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="newIndex">The destination index.</param>
        void Move(IEntityItemView<T> item, int newIndex);

        void ApplySort(IComparer<IEntityItemView<T>> comparer);

        /// <summary>
        /// Applies the given filter.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void ApplyFilter(Predicate<T> predicate);

        TValue GetCustomPropertyValue<TValue>(string customPropertyName, IEntityItemView<T> item);

        void SetCustomPropertyValue<TValue>(string customPropertyName, IEntityItemView<T> item, TValue value);

        event EventHandler<AddingNewEventArgs<T>> AddingNew;

        new IEntityItemView<T> AddNew();
        IEntityItemView<T> AddNew(Action<AddingNewEventArgs<T>> addNewInterceptor);
    }
}
