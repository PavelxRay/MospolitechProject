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

            // Обновляем статистику
            BooksCountLabel.Text = $"{books.Count} книг в библиотеке";
            ReadingCountLabel.Text = books.Count(b => b.IsReading).ToString();

            FinishedCountLabel.Text = books.Count(b => b.IsFinished).ToString();
        }

        // ОБЫЧНОЕ НАЖАТИЕ - ПЕРЕХОД
        private async void OnBookTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var book = frame.BindingContext as Book;
            if (book != null)
            {
                await Navigation.PushAsync(new BookDetailsPage(book.Id));
            }
        }

        // СВАЙП И УДАЛЕНИЕ
        private async void OnDeleteInvoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItemView;
            if (swipeItem == null) return;

            var book = swipeItem.CommandParameter as Book;
            if (book == null) return;

            bool confirm = await DisplayAlert("Удаление", $"Удалить книгу '{book.Title}'?", "Да", "Нет");
            if (confirm)
            {
                if (!string.IsNullOrEmpty(book.FilePath) && File.Exists(book.FilePath))
                    File.Delete(book.FilePath);

                if (!string.IsNullOrEmpty(book.CoverUrl) && File.Exists(book.CoverUrl))
                    File.Delete(book.CoverUrl);

                await _dbService.DeleteChapters(book.Id);
                await _dbService.DeleteBook(book);
                await RefreshLibraryData();
            }
        }

        private async void OnImportClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ImportPage());
        }
    }
}
