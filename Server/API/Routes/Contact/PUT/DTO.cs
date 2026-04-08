using FastEndpoints;
using FluentValidation;

namespace Server.API.Routes.Contact.PUT;

public record PutContactRequest
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Address { get; set; }
}

public record PutContactResponse
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Address { get; set; }
}

public class PutContactRequestValidator : Validator<PutContactRequest>
{
    public PutContactRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-post är nödvändigt.")
            .EmailAddress().WithMessage("Ogiltigt e-postformat.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefonnummer är nödvändigt.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Ogiltigt telefonnummerformat.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adress är nödvändig.");
    }
}