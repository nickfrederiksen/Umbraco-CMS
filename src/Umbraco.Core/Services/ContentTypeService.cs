using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Persistence.Repositories;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Services.Filters;
using Umbraco.Cms.Core.Services.Locking;

namespace Umbraco.Cms.Core.Services;

/// <summary>
///     Represents the ContentType Service, which is an easy access to operations involving <see cref="IContentType" />
/// </summary>
public class ContentTypeService : ContentTypeServiceBase<IContentTypeRepository, IContentType>, IContentTypeService
{
    public ContentTypeService(
        ICoreScopeProvider provider,
        ILoggerFactory loggerFactory,
        IEventMessagesFactory eventMessagesFactory,
        IContentService contentService,
        IContentTypeRepository repository,
        IAuditRepository auditRepository,
        IDocumentTypeContainerRepository entityContainerRepository,
        IEntityRepository entityRepository,
        IEventAggregator eventAggregator,
        IUserIdKeyResolver userIdKeyResolver,
        ContentTypeFilterCollection contentTypeFilters)
        : base(
            provider,
            loggerFactory,
            eventMessagesFactory,
            repository,
            auditRepository,
            entityContainerRepository,
            entityRepository,
            eventAggregator,
            userIdKeyResolver,
            contentTypeFilters) =>
        ContentService = contentService;

    protected override int[] ReadLockIds => ContentTypeLocks.ReadLockIds;

    protected override int[] WriteLockIds => ContentTypeLocks.WriteLockIds;

    protected override Guid ContainedObjectType => Constants.ObjectTypes.DocumentType;

    private IContentService ContentService { get; }

    /// <summary>
    ///     Gets all property type aliases across content, media and member types.
    /// </summary>
    /// <returns>All property type aliases.</returns>
    /// <remarks>Beware! Works across content, media and member types.</remarks>
    public IEnumerable<string> GetAllPropertyTypeAliases()
    {
        using (ICoreScope scope = ScopeProvider.CreateCoreScope(autoComplete: true))
        {
            // that one is special because it works across content, media and member types
            scope.ReadLock(Constants.Locks.ContentTypes, Constants.Locks.MediaTypes, Constants.Locks.MemberTypes);
            return Repository.GetAllPropertyTypeAliases();
        }
    }

    /// <summary>
    ///     Gets all content type aliases across content, media and member types.
    /// </summary>
    /// <param name="guids">Optional object types guid to restrict to content, and/or media, and/or member types.</param>
    /// <returns>All content type aliases.</returns>
    /// <remarks>Beware! Works across content, media and member types.</remarks>
    public IEnumerable<string> GetAllContentTypeAliases(params Guid[] guids)
    {
        using (ICoreScope scope = ScopeProvider.CreateCoreScope(autoComplete: true))
        {
            // that one is special because it works across content, media and member types
            scope.ReadLock(Constants.Locks.ContentTypes, Constants.Locks.MediaTypes, Constants.Locks.MemberTypes);
            return Repository.GetAllContentTypeAliases(guids);
        }
    }

    /// <summary>
    ///     Gets all content type id for aliases across content, media and member types.
    /// </summary>
    /// <param name="aliases">Aliases to look for.</param>
    /// <returns>All content type ids.</returns>
    /// <remarks>Beware! Works across content, media and member types.</remarks>
    public IEnumerable<int> GetAllContentTypeIds(string[] aliases)
    {
        using (ICoreScope scope = ScopeProvider.CreateCoreScope(autoComplete: true))
        {
            // that one is special because it works across content, media and member types
            scope.ReadLock(Constants.Locks.ContentTypes, Constants.Locks.MediaTypes, Constants.Locks.MemberTypes);
            return Repository.GetAllContentTypeIds(aliases);
        }
    }

    public async Task<IEnumerable<IContentType>> GetByQueryAsync(IQuery<IContentType> query, CancellationToken cancellationToken)
    {
        using ICoreScope scope = ScopeProvider.CreateCoreScope();
        // that one is special because it works across content, media and member types
        scope.ReadLock(Constants.Locks.ContentTypes);
        IEnumerable<IContentType> contentTypes = Repository.Get(query);
        scope.Complete();
        return contentTypes;
    }

    protected override void DeleteItemsOfTypes(IEnumerable<int> typeIds)
    {
        using (ICoreScope scope = ScopeProvider.CreateCoreScope())
        {
            var typeIdsA = typeIds.ToArray();
            ContentService.DeleteOfTypes(typeIdsA);
            ContentService.DeleteBlueprintsOfTypes(typeIdsA);
            scope.Complete();
        }
    }

    #region Notifications

    protected override SavingNotification<IContentType> GetSavingNotification(
        IContentType item,
        EventMessages eventMessages) => new ContentTypeSavingNotification(item, eventMessages);

    protected override SavingNotification<IContentType> GetSavingNotification(
        IEnumerable<IContentType> items,
        EventMessages eventMessages) => new ContentTypeSavingNotification(items, eventMessages);

    protected override SavedNotification<IContentType> GetSavedNotification(
        IContentType item,
        EventMessages eventMessages) => new ContentTypeSavedNotification(item, eventMessages);

    protected override SavedNotification<IContentType> GetSavedNotification(
        IEnumerable<IContentType> items,
        EventMessages eventMessages) => new ContentTypeSavedNotification(items, eventMessages);

    protected override DeletingNotification<IContentType> GetDeletingNotification(
        IContentType item,
        EventMessages eventMessages) => new ContentTypeDeletingNotification(item, eventMessages);

    protected override DeletingNotification<IContentType> GetDeletingNotification(
        IEnumerable<IContentType> items,
        EventMessages eventMessages) => new ContentTypeDeletingNotification(items, eventMessages);

    protected override DeletedNotification<IContentType> GetDeletedNotification(
        IEnumerable<IContentType> items,
        EventMessages eventMessages) => new ContentTypeDeletedNotification(items, eventMessages);

    protected override MovingNotification<IContentType> GetMovingNotification(
        MoveEventInfo<IContentType> moveInfo,
        EventMessages eventMessages) => new ContentTypeMovingNotification(moveInfo, eventMessages);

    protected override MovedNotification<IContentType> GetMovedNotification(
        IEnumerable<MoveEventInfo<IContentType>> moveInfo, EventMessages eventMessages) =>
        new ContentTypeMovedNotification(moveInfo, eventMessages);

    protected override ContentTypeChangeNotification<IContentType> GetContentTypeChangedNotification(
        IEnumerable<ContentTypeChange<IContentType>> changes, EventMessages eventMessages) =>
        new ContentTypeChangedNotification(changes, eventMessages);

    protected override ContentTypeRefreshNotification<IContentType> GetContentTypeRefreshedNotification(
        IEnumerable<ContentTypeChange<IContentType>> changes, EventMessages eventMessages) =>
        new ContentTypeRefreshedNotification(changes, eventMessages);

    #endregion
}
