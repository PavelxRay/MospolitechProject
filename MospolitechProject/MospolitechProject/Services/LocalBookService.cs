using System.Collections.Generic;
using System.Threading.Tasks;
using MospolitechProject.Models;

namespace MospolitechProject.Services
{
    public class LocalBookService : IBookService
    {
        private readonly DatabaseService _db = new DatabaseService();

        public async Task<List<Book>> GetBooksAsync()
        {
            await _db.Init();
            return await _db.GetBooks();
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            var result = await _db.SaveBook(book);
            return result > 0;
        }
    }
}
