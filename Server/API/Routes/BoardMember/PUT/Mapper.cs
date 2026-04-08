using Server.API.Models;

namespace Server.API.Routes.BoardMember.PUT;

public static class PutBoardMemberMapper
{
    public static BoardMemberModel MapToBoardMemberModel(PutBoardMemberRequest request)
    {
        List<BoardMemberTranslationModel> translations = [];
        foreach (string language in request.Translations.Keys)
        {
            translations.Add(new BoardMemberTranslationModel
            {
                LanguageCode = language,
                Text = request.Translations[language],
                BoardMember = null! // This will be set by EF Core when we add the BoardMemberModel to the context.
            });
        }

        return new BoardMemberModel
        {
            Id = request.Id,
            SortOrder = request.SortOrder,
            Translations = translations
        };
    }

    public static PutBoardMemberResponse MapToPutBoardMemberResponse(BoardMemberModel model)
    {
        Dictionary<string, string> translations = [];
        foreach (BoardMemberTranslationModel translation in model.Translations)
        {
            translations[translation.LanguageCode] = translation.Text;
        }

        return new PutBoardMemberResponse
        {
            Id = model.Id,
            SortOrder = model.SortOrder,
            Translations = translations
        };
    }
}