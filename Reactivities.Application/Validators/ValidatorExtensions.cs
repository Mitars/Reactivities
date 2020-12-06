using FluentValidation;

namespace Reactivities.Application.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder) =>
            ruleBuilder
                .NotEmpty()
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .Matches("[A-Z]").WithMessage("Password must contain a uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase character")
                .Matches("[0-9]").WithMessage("Password must contain a number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a non alphanumeric character");
    }
}