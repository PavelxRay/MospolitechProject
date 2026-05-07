using System.Collections.Generic;
using System.Threading.Tasks;
using MospolitechProject.Models;

namespace MospolitechProject.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        Task<bool> AddBookAsync(Book book);
    }
}