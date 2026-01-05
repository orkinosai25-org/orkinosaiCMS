using OrkinosaiCMS.Core.Entities.Sites;

namespace OrkinosaiCMS.Core.Interfaces.Services;

/// <summary>
/// Service for managing page layouts, sections, and blocks
/// </summary>
public interface IPageLayoutService
{
    // Layout operations
    Task<PageLayout?> GetLayoutByPageIdAsync(int pageId);
    Task<PageLayout> CreateLayoutAsync(PageLayout layout);
    Task<PageLayout> UpdateLayoutAsync(PageLayout layout);
    Task DeleteLayoutAsync(int layoutId);

    // Section operations
    Task<PageSection> CreateSectionAsync(PageSection section);
    Task<PageSection> UpdateSectionAsync(PageSection section);
    Task DeleteSectionAsync(int sectionId);
    Task<IEnumerable<PageSection>> GetSectionsByLayoutIdAsync(int layoutId);
    Task ReorderSectionsAsync(int layoutId, Dictionary<int, int> sectionOrders);

    // Block operations
    Task<PageBlock> CreateBlockAsync(PageBlock block);
    Task<PageBlock> UpdateBlockAsync(PageBlock block);
    Task DeleteBlockAsync(int blockId);
    Task<IEnumerable<PageBlock>> GetBlocksBySectionIdAsync(int sectionId);
    Task ReorderBlocksAsync(int sectionId, Dictionary<int, int> blockOrders);

    // Template operations
    Task<PageLayout> ApplyTemplateAsync(int pageId, string templateName);
}
