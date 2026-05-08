using Server.API.Models;
using Server.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Server.API.Routes.Page.POST;

public class PostPageData(AppDbContext ctx)
{
    public async Task<PageModel> SavePageAsync(PostPageRequest request, CancellationToken ct)
    {
        PageModel incomingPage = !request.Id.HasValue ?
            PostPageMapper.ToModel(request, new Guid()) :
            PostPageMapper.ToModel(request, request.Id.Value);

        if (await ctx.Pages.AnyAsync(page => page.Id == incomingPage.Id, cancellationToken: ct))
        {
            PageModel currentPage = await ctx.Pages.Include(page => page.Translations).FirstOrDefaultAsync(page => page.Id == incomingPage.Id, ct)
                ?? throw new KeyNotFoundException("Sidan kunde inte hittas.");

            currentPage.Type = incomingPage.Type;
            currentPage.IsPublished = incomingPage.IsPublished;
            if (incomingPage.IsPublished && !currentPage.PublishedAt.HasValue)
            {
                currentPage.PublishedAt = DateTime.UtcNow;
            }

            // If a page is unpublished, we should clear the PublishedAt and UpdatedAt fields, and set IsPublished to false.
            if (!incomingPage.IsPublished)
            {
                currentPage.PublishedAt = null;
                currentPage.UpdatedAt = null;
                currentPage.IsPublished = false;
            }

            // Updating translations by clearing and re-adding them.
            ctx.PageTranslations.RemoveRange(currentPage.Translations);
            currentPage.Translations.Clear();

            foreach (PageTranslationModel translation in incomingPage.Translations)
            {
                translation.PageId = currentPage.Id;
                currentPage.Translations.Add(translation);
            }

            if (ctx.ChangeTracker.HasChanges())
            {
                currentPage.SavedAt = DateTime.UtcNow;
                if (incomingPage.IsPublished && currentPage.PublishedAt.HasValue)
                {
                    currentPage.UpdatedAt = currentPage.SavedAt;
                }
                await ctx.SaveChangesAsync(ct);
            }
        }
        else
        {
            incomingPage.SavedAt = DateTime.UtcNow;
            if (incomingPage.IsPublished)
            {
                incomingPage.PublishedAt = incomingPage.SavedAt;
            }
            await ctx.Pages.AddAsync(incomingPage, ct);
            await ctx.SaveChangesAsync(ct);
        }

        return incomingPage;
    }
}