using Server.API.Exceptions;
using Server.API.Routes.User.SignIn;

namespace Server.UI.Services;

public record APIResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ExceptionResponse? Error { get; set; }
}