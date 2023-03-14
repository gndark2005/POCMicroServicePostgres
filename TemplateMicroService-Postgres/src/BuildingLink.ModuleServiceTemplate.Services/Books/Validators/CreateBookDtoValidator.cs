using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Services.Books.Validators
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        private readonly IBookRepository _bookRepository;

        public CreateBookDtoValidator(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;

            RuleFor(dto => dto.Title)
                .NotEmpty()
                .NotNull();

            RuleFor(dto => dto.Author)
                .NotEmpty()
                .NotNull();

            RuleFor(dto => dto)
                .MustAsync(NotExistAnotherBookWithSameTitleAndAuthor)
                .WithMessage("There is a book with the same title and author already registered.");
        }

        private async Task<bool> NotExistAnotherBookWithSameTitleAndAuthor(CreateBookDto dto, CancellationToken token)
        {
            var doesExists = await _bookRepository.DoesBookExistWith(dto.Title, dto.Author);
            return !doesExists;
        }
    }
}
