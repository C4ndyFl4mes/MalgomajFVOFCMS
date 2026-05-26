using FastEndpoints;
using Server.API.Data;
using Server.API.Models;

namespace Server.API.Routes.Slide.GET;

public class GetSlideshowEndpoint(AppDbContext ctx) : Endpoint<GetSlideshowRequest, GetSlideshowResponse>
{
    public override void Configure()
    {
        Get("/api/slides");
        AllowAnonymous();
    }

    public override async Task<GetSlideshowResponse> ExecuteAsync(GetSlideshowRequest request, CancellationToken ct)
    {
        GetSlideshowData data = new(ctx);

        List<SlideModel> slides = await data.GetSlideshowAsync(ct);

        return GetSlideshowMapper.ToResponse(slides, request.LanguageCode);
    }
}