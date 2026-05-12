using MospolitechProject.Helpers;
using MospolitechProject.Services;
using MospolitechProject.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MospolitechProject.Views
{
    public partial class ImportPage : ContentPage
    {
        private readonly EpubService _epubService = new EpubService();
        private readonly DatabaseService _dbService = new DatabaseService();

        public ImportPage()
        {
            InitializeComponent();
        }

        private async void OnSelectFileClicked(object sender, EventArgs e)
        {
            try
            {
                var customPicker = DependencyService.Get<IFilePicker>();
                var localPath = await customPicker.GetFileAsync();

                if (string.IsNullOrEmpty(localPath)) return;

                LoadingLayout.IsVisible = true;
                StatusLabel.Text = "Парсинг книги...";

                var book = await _epubService.ParseEpubAsync(localPath);
                book.TotalChapters = _epubService.Chapters.Count;
                book.Progress = 0;
                book.IsReading = true;

                await _dbService.Init();
                await _dbService.SaveBook(book);

                var dbChapters = _epubService.Chapters.Select((ch, index) => new Chapter
                {
                    BookId = book.Id,
                    Index = index,
                    Title = ch.Title,
                    Text = ch.Text,
                    IsCompleted = false
                }).ToList();

                await _dbService.InsertChapters(dbChapters);

                LoadingLayout.IsVisible = false;

                // ВМЕСТО DisplayAlert:
                SuccessMessageLabel.Text = $"Книга '{book.Title}' добавлена в библиотеку!";
                SuccessAlert.IsVisible = true;
            }
            catch (Exception ex)
            {
                LoadingLayout.IsVisible = false;
                // Ошибки можно оставить системными или сделать ErrorAlert по аналогии
                await DisplayAlert("Ошибка импорта", ex.Message, "OK");
            }
        }

        // Обработчик кнопки в кастомном алерте
        private async void OnSuccessConfirmClicked(object sender, EventArgs e)
        {
            SuccessAlert.IsVisible = false;
            await Navigation.PopAsync(); // Возвращаемся в библиотеку
        }

    }
}
