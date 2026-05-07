using SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MospolitechProject.Models;
using Xamarin.Essentials;

namespace MospolitechProject.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        public async Task Init()
        {
            if (_db != null) return;
            var path = Path.Combine(FileSystem.AppDataDirectory, "library.db3");
            _db = new SQLiteAsyncConnection(path);
            await _db.CreateTableAsync<Book>();
        }

        public Task<List<Book>> GetBooks() => _db.Table<Book>().ToListAsync();
        public Task<int> SaveBook(Book book) => _db.InsertAsync(book);

        public Task<int> DeleteBook(Book book)
        {
            return _db.DeleteAsync(book);
        }
    }


}
