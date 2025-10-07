using FluentValidation;
using FluentValidation.Results;
using Library.Api.Auth;
using Library.Api.Data;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);

builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ =>
    {
        
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ => 
    new SqliteConnectionFactory(
        builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

// добавляем книгу, нужна авторизация
app.MapPost("books",
    [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)]
    async (Book book, IBookService bookService,
    IValidator<Book> validator) =>
{
    var validationResult = await validator.ValidateAsync(book);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    
    var created = await bookService.CreateAsync(book);

    if (!created)
    {
        return Results.BadRequest(new List<ValidationFailure>
        {
            new ("Isbn", "A book with this ISBN-13 already exists")
        });
    }

    return Results.Created($"/books/{book.Isbn}", book);
});

// возвращаем все книги или книг соответсвующих поиску по названию
app.MapGet("books",
    [AllowAnonymous]
    async (IBookService bookService,
    string? searchTerm) =>
{
    if (searchTerm is not null && !string.IsNullOrWhiteSpace(searchTerm))
    {
        var matchedTitleBooks = await bookService.GetByTitleAsync(searchTerm);
        return Results.Ok(matchedTitleBooks);
    }
    
    var books = await bookService.GetAllAsync();

    return Results.Ok(books);
});

// возвращаем книгу
app.MapGet("books/{isbn}",
    [AllowAnonymous]
    async (string isbn, IBookService bookService) =>
{
    var book = await bookService.GetByIsbnAsync(isbn);
    
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

// редактируем книгу, нужна авторизация
app.MapPut("books/{isbn}",
    [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)]
    async (string isbn, Book book, IBookService bookService, IValidator<Book> validator) =>
{
    book.Isbn = isbn;
    
    var validationResult = await validator.ValidateAsync(book);
    
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    
    var updated = await bookService.UpdateAsync(book);
    
    return updated ? Results.Ok(book) : Results.NotFound();
});

// удаляем книгу, нужна авторизация
app.MapDelete("books/{isbn}",
    [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)]
    async (string isbn, IBookService bookService) =>
{
    var deleted = await bookService.DeleteAsync(isbn);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// DB init
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();