using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MospolitechProject.Helpers;
using MospolitechProject.Services;

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

                // Включаем спиннер
                LoadingLayout.IsVisible = true;
                StatusLabel.Text = "Парсинг книги...";

                var book = await _epubService.ParseEpubAsync(localPath);
                book.IsReading = true;

                await _dbService.Init();
                await _dbService.SaveBook(book);

                // 1. ОСТАНАВЛИВАЕМ спиннер сразу после записи в БД
                LoadingLayout.IsVisible = false;

                // 2. Показываем успех
                await DisplayAlert("Успех", $"Книга '{book.Title}' добавлена!", "ОК");

                // 3. ПЕРЕХОДИМ в библиотеку
                // Используем PopAsync для возврата по стеку навигации
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                LoadingLayout.IsVisible = false;
                await DisplayAlert("Ошибка импорта", ex.Message, "OK");
            }
        }

    }
}
