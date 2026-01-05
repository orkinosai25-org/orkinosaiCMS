using Microsoft.EntityFrameworkCore;
using OrkinosaiCMS.Core.Entities.Sites;
using OrkinosaiCMS.Core.Interfaces.Repositories;
using OrkinosaiCMS.Core.Interfaces.Services;
using OrkinosaiCMS.Infrastructure.Data;

namespace OrkinosaiCMS.Infrastructure.Services;

/// <summary>
/// Service implementation for page layout management operations
/// </summary>
public class PageLayoutService : IPageLayoutService
{
    private readonly IRepository<PageLayout> _layoutRepository;
    private readonly IRepository<PageSection> _sectionRepository;
    private readonly IRepository<PageBlock> _blockRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public PageLayoutService(
        IRepository<PageLayout> layoutRepository,
        IRepository<PageSection> sectionRepository,
        IRepository<PageBlock> blockRepository,
        IUnitOfWork unitOfWork,
        ApplicationDbContext context)
    {
        _layoutRepository = layoutRepository;
        _sectionRepository = sectionRepository;
        _blockRepository = blockRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    // Layout operations
    public async Task<PageLayout?> GetLayoutByPageIdAsync(int pageId)
    {
        return await _context.PageLayouts
            .Include(l => l.Sections.OrderBy(s => s.Order))
            .ThenInclude(s => s.Blocks.OrderBy(b => b.Order))
            .FirstOrDefaultAsync(l => l.PageId == pageId && l.IsActive);
    }

    public async Task<PageLayout> CreateLayoutAsync(PageLayout layout)
    {
        await _layoutRepository.AddAsync(layout);
        await _unitOfWork.SaveChangesAsync();
        return layout;
    }

    public async Task<PageLayout> UpdateLayoutAsync(PageLayout layout)
    {
        _layoutRepository.Update(layout);
        await _unitOfWork.SaveChangesAsync();
        return layout;
    }

    public async Task DeleteLayoutAsync(int layoutId)
    {
        var layout = await _layoutRepository.GetByIdAsync(layoutId);
        if (layout != null)
        {
            _layoutRepository.Remove(layout);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // Section operations
    public async Task<PageSection> CreateSectionAsync(PageSection section)
    {
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();
        return section;
    }

    public async Task<PageSection> UpdateSectionAsync(PageSection section)
    {
        _sectionRepository.Update(section);
        await _unitOfWork.SaveChangesAsync();
        return section;
    }

    public async Task DeleteSectionAsync(int sectionId)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId);
        if (section != null)
        {
            _sectionRepository.Remove(section);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PageSection>> GetSectionsByLayoutIdAsync(int layoutId)
    {
        return await _context.PageSections
            .Where(s => s.PageLayoutId == layoutId)
            .OrderBy(s => s.Order)
            .Include(s => s.Blocks)
            .ToListAsync();
    }

    public async Task ReorderSectionsAsync(int layoutId, Dictionary<int, int> sectionOrders)
    {
        foreach (var (sectionId, order) in sectionOrders)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section != null && section.PageLayoutId == layoutId)
            {
                section.Order = order;
                _sectionRepository.Update(section);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    // Block operations
    public async Task<PageBlock> CreateBlockAsync(PageBlock block)
    {
        await _blockRepository.AddAsync(block);
        await _unitOfWork.SaveChangesAsync();
        return block;
    }

    public async Task<PageBlock> UpdateBlockAsync(PageBlock block)
    {
        _blockRepository.Update(block);
        await _unitOfWork.SaveChangesAsync();
        return block;
    }

    public async Task DeleteBlockAsync(int blockId)
    {
        var block = await _blockRepository.GetByIdAsync(blockId);
        if (block != null)
        {
            _blockRepository.Remove(block);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PageBlock>> GetBlocksBySectionIdAsync(int sectionId)
    {
        return await _context.PageBlocks
            .Where(b => b.PageSectionId == sectionId)
            .OrderBy(b => b.ColumnIndex)
            .ThenBy(b => b.Order)
            .ToListAsync();
    }

    public async Task ReorderBlocksAsync(int sectionId, Dictionary<int, int> blockOrders)
    {
        foreach (var (blockId, order) in blockOrders)
        {
            var block = await _blockRepository.GetByIdAsync(blockId);
            if (block != null && block.PageSectionId == sectionId)
            {
                block.Order = order;
                _blockRepository.Update(block);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    // Template operations
    public async Task<PageLayout> ApplyTemplateAsync(int pageId, string templateName)
    {
        // Create a new layout for the page
        var layout = new PageLayout
        {
            PageId = pageId,
            IsActive = true,
            Version = 1
        };

        await _layoutRepository.AddAsync(layout);
        await _unitOfWork.SaveChangesAsync();

        // Apply template-specific structure
        switch (templateName.ToLower())
        {
            case "hero":
                await ApplyHeroTemplateAsync(layout);
                break;
            case "gallery":
                await ApplyGalleryTemplateAsync(layout);
                break;
            case "text-image":
                await ApplyTextImageTemplateAsync(layout);
                break;
            case "cards":
                await ApplyCardsTemplateAsync(layout);
                break;
            default:
                await ApplyBasicTemplateAsync(layout);
                break;
        }

        return layout;
    }

    private async Task ApplyHeroTemplateAsync(PageLayout layout)
    {
        var section = new PageSection
        {
            PageLayoutId = layout.Id,
            Order = 0,
            SectionType = "full-width",
            ColumnConfiguration = "[{\"width\": \"100%\"}]"
        };
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();

        var block = new PageBlock
        {
            PageSectionId = section.Id,
            ColumnIndex = 0,
            Order = 0,
            BlockType = "hero",
            Content = "{\"title\": \"Welcome\", \"subtitle\": \"Your hero message here\", \"imageUrl\": \"\"}"
        };
        await _blockRepository.AddAsync(block);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ApplyGalleryTemplateAsync(PageLayout layout)
    {
        var section = new PageSection
        {
            PageLayoutId = layout.Id,
            Order = 0,
            SectionType = "full-width",
            ColumnConfiguration = "[{\"width\": \"100%\"}]"
        };
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();

        var block = new PageBlock
        {
            PageSectionId = section.Id,
            ColumnIndex = 0,
            Order = 0,
            BlockType = "gallery",
            Content = "{\"images\": []}"
        };
        await _blockRepository.AddAsync(block);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ApplyTextImageTemplateAsync(PageLayout layout)
    {
        var section = new PageSection
        {
            PageLayoutId = layout.Id,
            Order = 0,
            SectionType = "two-column",
            ColumnConfiguration = "[{\"width\": \"50%\"}, {\"width\": \"50%\"}]"
        };
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();

        var textBlock = new PageBlock
        {
            PageSectionId = section.Id,
            ColumnIndex = 0,
            Order = 0,
            BlockType = "text",
            Content = "{\"html\": \"<p>Your text content here</p>\"}"
        };
        var imageBlock = new PageBlock
        {
            PageSectionId = section.Id,
            ColumnIndex = 1,
            Order = 0,
            BlockType = "image",
            Content = "{\"src\": \"\", \"alt\": \"Image\"}"
        };
        await _blockRepository.AddAsync(textBlock);
        await _blockRepository.AddAsync(imageBlock);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ApplyCardsTemplateAsync(PageLayout layout)
    {
        var section = new PageSection
        {
            PageLayoutId = layout.Id,
            Order = 0,
            SectionType = "three-column",
            ColumnConfiguration = "[{\"width\": \"33.33%\"}, {\"width\": \"33.33%\"}, {\"width\": \"33.33%\"}]"
        };
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();

        for (int i = 0; i < 3; i++)
        {
            var card = new PageBlock
            {
                PageSectionId = section.Id,
                ColumnIndex = i,
                Order = 0,
                BlockType = "cards",
                Content = $"{{\"title\": \"Card {i + 1}\", \"text\": \"Card content\", \"imageUrl\": \"\"}}"
            };
            await _blockRepository.AddAsync(card);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ApplyBasicTemplateAsync(PageLayout layout)
    {
        var section = new PageSection
        {
            PageLayoutId = layout.Id,
            Order = 0,
            SectionType = "full-width",
            ColumnConfiguration = "[{\"width\": \"100%\"}]"
        };
        await _sectionRepository.AddAsync(section);
        await _unitOfWork.SaveChangesAsync();

        var block = new PageBlock
        {
            PageSectionId = section.Id,
            ColumnIndex = 0,
            Order = 0,
            BlockType = "text",
            Content = "{\"html\": \"<p>Start editing your page</p>\"}"
        };
        await _blockRepository.AddAsync(block);
        await _unitOfWork.SaveChangesAsync();
    }
}
