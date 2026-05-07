using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MospolitechProject.Models;
using MospolitechProject.Services;

namespace MospolitechProject.Views
{
    public partial class LibraryPage : ContentPage
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public LibraryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshLibraryData();
        }

        private async Task RefreshLibraryData()
        {
            await _dbService.Init();
            var books = await _dbService.GetBooks();
            BooksCollection.ItemsSource = books;

            // Статистика (динамическая)
            ReadingCountLabel.Text = books.Count(b => b.IsReading).ToString();
            BooksCountLabel.Text = $"{books.Count} книг в библиотеке";
        }

        // ОБЫЧНОЕ НАЖАТИЕ - ПЕРЕХОД
        private async void OnBookTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var book = frame.BindingContext as Book;
            if (book != null)
            {
                await Navigation.PushAsync(new ReaderPage(book));
            }
        }

        // СВАЙП И УДАЛЕНИЕ
        private async void OnDeleteInvoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var book = swipeItem.CommandParameter as Book;

            bool confirm = await DisplayAlert("Удаление", $"Удалить книгу '{book.Title}'?", "Да", "Нет");
            if (confirm)
            {
                // 1. Удаляем физические файлы (книгу и обложку)
                if (!string.IsNullOrEmpty(book.FilePath) && File.Exists(book.FilePath))
                    File.Delete(book.FilePath);

                if (!string.IsNullOrEmpty(book.CoverUrl) && File.Exists(book.CoverUrl))
                    File.Delete(book.CoverUrl);

                // 2. Удаляем из БД
                await _dbService.DeleteBook(book);

                // 3. Обновляем UI
                await RefreshLibraryData();
            }
        }

        private async void OnImportClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImportPage());
        }
    }
}
