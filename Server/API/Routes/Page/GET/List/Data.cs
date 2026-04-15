using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Page.GET.List;

public class GetPageListData(AppDbContext ctx)
{
    public async Task<List<PageModel>> GetPageListAsync(CancellationToken ct)
    {
        List<PageModel> pages = await ctx.Pages.Include(page => page.Translations).ToListAsync(ct);
        return pages;
    }
}