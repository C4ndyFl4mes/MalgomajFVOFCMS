using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.User.PUT;

public record UpdatePasswordRequest
{
    public required Guid UserId { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public record Token
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public record UpdatePasswordResponse
{
    public required string Message { get; set; }
}

public class UpdatePasswordValidator : Validator<UpdatePasswordRequest>
{
    public UpdatePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Nuvarande lösenord är obligatoriskt.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nytt lösenord är obligatoriskt.")
            .MinimumLength(8).WithMessage("Nytt lösenord måste vara minst åtta tecken långt.")
            .Must(PasswordComplexity).WithMessage("Nytt lösenord måste innehålla minst en versal, en gemen, en siffra och ett specialtecken.");
    }

    private bool PasswordComplexity(string password)
    {
        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
    }
}