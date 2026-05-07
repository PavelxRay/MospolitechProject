using MospolitechProject.Models;
using MospolitechProject.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MospolitechProject
{
    public partial class MainPage : ContentPage
    {
        // Ссылаемся на ИНТЕРФЕЙС, а не на реализацию
        private readonly IBookService _bookService = new LocalBookService();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshData();
        }

        private async Task RefreshData()
        {
            // UI просто просит данные, ему не важно откуда они (БД или API)
            BooksListView.ItemsSource = await _bookService.GetBooksAsync();
        }

        async void OnCheckApiClicked(object sender, EventArgs e)
        {
            // Здесь можно оставить твой Mock API для демонстрации разделения
            StatusLabel.Text = "Данные получены через интерфейс IBookService";
        }
    }
}