using FluentValidation;
using Library.Api.Models;

namespace Library.Api.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(book => book.Isbn)
            .Matches(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$")
            .WithMessage("Value was not a valid ISBN number");
        
        RuleFor(book => book.Title).NotEmpty();
        RuleFor(book => book.Description).NotEmpty();
        RuleFor(book => book.PageCount).NotEmpty();
        RuleFor(book => book.Author).NotEmpty();
    }
}