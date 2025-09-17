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
    
    // TODO: добавить проверку добавления уже существующей книги
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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        return connection.QuerySingleOrDefault<Book>("SELECT * FROM Books WHERE Isbn=@Isbn", new { Isbn = isbn });
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        return await connection.QueryAsync<Book>("SELECT * FROM Books");
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