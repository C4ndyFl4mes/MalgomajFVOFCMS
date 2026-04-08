using System.Runtime.Serialization;

namespace Server.API.Enums;

public enum PageType
{
    [EnumMember(Value = "article")]
    Article,
    [EnumMember(Value = "page")]
    Page
}