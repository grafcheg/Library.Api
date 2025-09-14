using Dapper;
using Library.Api.Data;
using Library.Api.Models;

namespace Library.Api.Services;

public class BookService : IBookService
{
    private readonly IDbConnectionFactory _connectionFactory;
    
    public BookService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;    
    }
    
    public async Task<bool> CreateAsync(Book book)
    {
        // var existingBook = await GetByIsbnAsync(book.Isnb);
        //
        // if (existingBook != null)
        // {
        //     return false;
        // }
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Books (Isbn, Title, Author, Description, PageCount, PublishDate)
            VALUES (@Isbn, @Title, @Author, @Description, @PageCount, @PublishDate)", book);
        
        return result > 0;
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> GetByTitleAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string isbn)
    {
        throw new NotImplementedException();
    }
}