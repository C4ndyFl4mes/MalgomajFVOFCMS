using Microsoft.EntityFrameworkCore;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Page.GET.Editor;

public class GetPageEditorData(AppDbContext ctx)
{
    public async Task<PageModel> GetPageEditorAsync(GetPageEditorRequest request, CancellationToken ct)
    {
        PageModel model = await ctx.Pages.Include(page => page.Translations).FirstOrDefaultAsync(page => page.Id == request.PageId, ct) ??
            throw new KeyNotFoundException("Hittade inte sidan.");
        
        return model;
    }
}