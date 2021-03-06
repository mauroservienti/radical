﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Radical.ComponentModel.ChangeTracking
{
    /// <summary>
    /// Provides change tracking functionalities.
    /// </summary>
    public interface IChangeTrackingService : IRevertibleChangeTracking, IComponent
    {
        /// <summary>
        /// Whether this component is disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Occurs when the internal state of the tracking service changes.
        /// </summary>
        event EventHandler TrackingServiceStateChanged;

        /// <summary>
        /// Occurs when are changes accepted.
        /// </summary>
        event EventHandler ChangesAccepted;

        /// <summary>
        /// Occurs when changes are rejected.
        /// </summary>
        event EventHandler ChangesRejected;

        event EventHandler<CancelEventArgs> AcceptingChanges;

        event EventHandler<CancelEventArgs> RejectingChanges;

        /// <summary>
        /// Creates a bookmark useful to save a position
        /// in this IChangeTrackingService.
        /// </summary>
        /// <remarks>
        /// A bookmark is always created also if there are no changes currently registered by
        /// the change tracking service, in this case reverting to the created bookmark equals
        /// to perform a full change reject.
        /// </remarks>
        /// <returns>An <c>IBookmark</c> instance.</returns>
        IBookmark CreateBookmark();

        /// <summary>
        /// Reverts the status of this IChangeTrackingService
        /// to the specified bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified 
        /// bookmark has not been created by this service.</exception>
        void Revert(IBookmark bookmark);

        /// <summary>
        /// Validates the specified bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        /// <returns><c>True</c> if the given bookmark is valid; otherwise <c>false</c>.</returns>
        bool Validate(IBookmark bookmark);

        /// <summary>
        /// Registers the supplied object as a new object.
        /// </summary>
        /// <param name="entity">The object to track as transient.</param>
        /// <exception cref="ArgumentException">If thew change tracking service has already registered the object or if has pending changes for the object an ArgumentException is raised.</exception>
        void RegisterTransient(object entity);

        /// <summary>
        /// Registers the supplied entity as a new object.
        /// </summary>
        /// <param name="entity">The object to track as transient.</param>
        /// <param name="autoRemove">if set to <c>true</c> the object is automatically removed from the list 
        /// of registered objects in case of Undo and RejectChanges.</param>
        /// <remarks>if <c>autoRemove</c> is set to true (the default value) and RejectChanges, 
        /// or an Undo that removes the last IChange of the object, is called the object then is automatically 
        /// removed from the list of the new objects.</remarks>
        /// <exception cref="ArgumentException">If the change tracking service has already registered the object or if has pending changes for the object an ArgumentException is raised.</exception>
        void RegisterTransient(object entity, bool autoRemove);

        /// <summary>
        /// Unregisters the supplied entity from the transient objects 
        /// marking it as a NonTransient entity.
        /// </summary>
        /// <param name="entity">The entity to unregister.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the supplied entity is not in <c>IsTransient</c> state an ArgumentException is raised.</exception>
        void UnregisterTransient(object entity);

        /// <summary>
        /// Gets a value indicating whether this instance has transient entities.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has transient entities; otherwise, <c>false</c>.
        /// </value>
        bool HasTransientEntities { get; }

        /// <summary>
        /// Gets the state of the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A set of values from the <see cref="EntityTrackingStates"/> enumeration.</returns>
        EntityTrackingStates GetEntityState(object entity);

        /// <summary>
        /// Gets all the entities tracked by this service instance.
        /// </summary>
        /// <returns>A enumerable list of tracked entities.</returns>
        IEnumerable<object> GetEntities();

        /// <summary>
        /// Gets a list of entities based on a filter.
        /// </summary>
        /// <param name="stateFilter">The sate filter to use to search entities.</param>
        /// <param name="exactMatch">if set to <c>true</c> the search is performed using an exact match behavior.</param>
        /// <returns>An enumerable list of entities that matches the filter.</returns>
        IEnumerable<object> GetEntities(EntityTrackingStates stateFilter, bool exactMatch);

        /// <summary>
        /// Gets a value indicating whether this instance can undo the last change.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        bool CanUndo { get; }

        /// <summary>
        /// Undoes the last IChange held by 
        /// this instance and removes it from
        /// the cache.
        /// </summary>
        void Undo();

        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        bool CanRedo { get; }

        /// <summary>
        /// Redoes the last undone change.
        /// </summary>
        void Redo();

        /// <summary>
        /// Gets all the changes currently held by
        /// this IChangeTrackingService
        /// </summary>
        /// <returns></returns>
        IChangeSet GetChangeSet();

        /// <summary>
        /// Gets all the changes currently held by
        /// this IChangeTrackingService filtered by the
        /// supplied IChangeSetBuilder.
        /// </summary>
        /// <param name="filter">The IChangeSetBuilder.</param>
        /// <returns></returns>
        IChangeSet GetChangeSet(IChangeSetFilter filter);

        /// <summary>
        /// Adds a new change definition to this IChangeTrackingService.
        /// </summary>
        /// <param name="change">The change to store.</param>
        /// <param name="behavior">The requested behavior.</param>
        void Add(IChange change, AddChangeBehavior behavior);

        /// <summary>
        /// Generates an advisory that contains all the operations that
        /// an hypothetical UnitOfWork must perform in order to persist
        /// all the changes tracked by this ChangeTrackingService.
        /// </summary>
        /// <returns>A read-only list of <see cref="IAdvisedAction"/>.</returns>
        IAdvisory GetAdvisory();

        /// <summary>
        /// Generates an advisory that contains all the operations that
        /// an hypothetical UnitOfWork must perform in order to persist
        /// all the changes tracked by this ChangeTrackingService.
        /// The generation is customized using the supplied <see cref="IAdvisoryBuilder"/>.
        /// </summary>
        /// <param name="builder">An instance of a class implementing this <see cref="IAdvisoryBuilder"/> 
        /// interface used to control the advisory generation process.</param>
        /// <returns>A read-only list of <see cref="IAdvisedAction"/>.</returns>
        IAdvisory GetAdvisory(IAdvisoryBuilder builder);

        /// <summary>
        /// Suspends all the tracking operation of this service.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Gets a value indicating whether this instance is suspended.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        bool IsSuspended { get; }

        /// <summary>
        /// Resumes all the tracking operation of this service.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Resume")]
        void Resume();

        /// <summary>
        /// Stops tracking the supplied entities removing any changes linked to the entity 
        /// and removing it, if necessary, from the transient entities.
        /// </summary>
        /// <param name="entity">The entity to stop tracking.</param>
        void Detach(IMemento entity);

        /// <summary>
        /// Attaches the specified item.
        /// </summary>
        /// <param name="item">The item to attach.</param>
        void Attach(IMemento item);

        /// <summary>
        /// Begins a new atomic operation. An atomic operation is useful to
        /// treat a set of subsequent changes as a single change.
        /// </summary>
        /// <exception cref="ArgumentException">An <c>ArgumentException</c> is raised if there
        /// is another active atomic operation.</exception>
        /// <returns>The newly created atomic operation.</returns>
        IAtomicOperation BeginAtomicOperation();

        /// <summary>
        /// Gets the state of the given entity property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="property">The property to inspect.</param>
        /// <returns>The actual property state.</returns>
        EntityPropertyStates GetEntityPropertyState<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property);

        /// <summary>
        /// Gets the state of the given entity property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The actual property state.
        /// </returns>
        EntityPropertyStates GetEntityPropertyState<TEntity, TProperty>(TEntity entity, string propertyName);
    }
}
