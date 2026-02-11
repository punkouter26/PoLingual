using FluentValidation;
using PoLingual.Shared.Models;

namespace PoLingual.Web.Validators;

public class StartDebateRequestValidator : AbstractValidator<StartDebateRequest>
{
    public StartDebateRequestValidator()
    {
        RuleFor(x => x.Rapper1).NotNull().WithMessage("Rapper 1 is required.");
        RuleFor(x => x.Rapper1.Name).NotEmpty().MaximumLength(100).When(x => x.Rapper1 != null);
        RuleFor(x => x.Rapper2).NotNull().WithMessage("Rapper 2 is required.");
        RuleFor(x => x.Rapper2.Name).NotEmpty().MaximumLength(100).When(x => x.Rapper2 != null);
        RuleFor(x => x.Topic).NotNull().WithMessage("Topic is required.");
        RuleFor(x => x.Topic.Title).NotEmpty().MaximumLength(200).When(x => x.Topic != null);
        RuleFor(x => x).Must(x => x.Rapper1?.Name != x.Rapper2?.Name).WithMessage("Rappers must be different.");
    }
}

public class TranslationRequestValidator : AbstractValidator<TranslationRequest>
{
    public TranslationRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(5000).WithMessage("Text is required and must be under 5000 chars.");
    }
}
