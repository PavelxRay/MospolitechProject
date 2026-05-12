using MospolitechProject.Models;
using MospolitechProject.ViewModels;
using System;
using Xamarin.Forms;

namespace MospolitechProject.Views
{
    public partial class BookDetailsPage : ContentPage
    {
        BookDetailsViewModel _viewModel;

        public BookDetailsPage(int bookId)
        {
            InitializeComponent();
            _viewModel = new BookDetailsViewModel();
            BindingContext = _viewModel;

            // Загружаем данные
            Device.BeginInvokeOnMainThread(async () => await _viewModel.LoadData(bookId));
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            // Переходим в читалку на сохраненный прогресс
            await Navigation.PushAsync(new ReaderPage(_viewModel.CurrentBook));
        }

        private async void OnChapterTapped(object sender, EventArgs e)
        {
            var chapter = (e as TappedEventArgs).Parameter as Chapter;
            // Устанавливаем прогресс книги на выбранную главу и открываем читалку
            _viewModel.CurrentBook.Progress = chapter.Index;
            await Navigation.PushAsync(new ReaderPage(_viewModel.CurrentBook));
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.ApplyFilters(search: e.NewTextValue);
        }

        private void OnFilterClicked(object sender, EventArgs e)
        {
            var filter = (sender as Button).CommandParameter.ToString();
            _viewModel.ApplyFilters(filter: filter);
        }

        private async void OnMarkAsReadInvoked(object sender, EventArgs e)
        {
            var chapter = (sender as SwipeItem).CommandParameter as Chapter;
            await _viewModel.ToggleReadStatus(chapter);
        }

        // Чтобы UI обновлялся при возврате из читалки
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.CurrentBook != null)
                await _viewModel.LoadData(_viewModel.CurrentBook.Id);
        }
    }
}
