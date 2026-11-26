using FluentValidation;
using Words.Core.Dto.Response;

namespace Application.Validation
{
    public class DefinitionValidator : AbstractValidator<DefinitionDto>
    {
        public DefinitionValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Definition content is required.")
                .MaximumLength(500).WithMessage("Definition content must be at most 500 characters.");

            RuleFor(x => x.EnglishTranslation)
                .MaximumLength(500).When(x => x.EnglishTranslation != null)
                .WithMessage("English translation must be at most 500 characters.");

            RuleFor(x => x.PartOfSpeech)
                .IsInEnum().When(x => x.PartOfSpeech.HasValue)
                .WithMessage("Definition part of speech is invalid.");
                
                RuleFor(x => x.GrammaticalFeature)
                .IsInEnum().When(x => x.GrammaticalFeature.HasValue)
                .WithMessage("Definition grammatical feature is invalid.");
                
                RuleForEach(x => x.Examples)
                .SetValidator(new DefinitionExampleValidator());
        }
    }

    public class DefinitionExampleValidator : AbstractValidator<DefinitionExampleDto>
    {
        public DefinitionExampleValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Example content is required.")
                .MaximumLength(300).WithMessage("Example content must be at most 300 characters.");

            RuleFor(x => x.EnglishTranslation)
                .MaximumLength(300).When(x => x.EnglishTranslation != null)
                .WithMessage("Example English translation must be at most 300 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Example type is invalid.");
        }
    }
}
