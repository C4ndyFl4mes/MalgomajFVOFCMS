using System.Runtime.Serialization;

namespace Server.API.Enums;

public enum MediaType
{
    [EnumMember(Value = "image")]
    Image,
    [EnumMember(Value = "video")]
    Video
}