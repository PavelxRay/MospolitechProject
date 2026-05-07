using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.App;
using MospolitechProject.Droid;
using MospolitechProject.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidFilePicker))]
namespace MospolitechProject.Droid
{
    public class AndroidFilePicker : IFilePicker
    {
        // Источник для ожидания результата выбора файла
        public static TaskCompletionSource<string> Tcs;

        public async Task<string> GetFileAsync()
        {
            Tcs = new TaskCompletionSource<string>();

            // Создаем намерение (Intent) для открытия документа
            var intent = new Intent(Intent.ActionOpenDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("application/epub+zip"); // Фильтр только для EPUB

            // Получаем текущую Activity (наш MainActivity)
            var activity = (MainActivity)Forms.Context;

            // Запускаем окно выбора
            activity.StartActivityForResult(intent, 1001);

            // Ждем, пока пользователь выберет файл и сработает OnActivityResult в MainActivity
            var uriString = await Tcs.Task;

            if (string.IsNullOrEmpty(uriString)) return null;

            return CopyFileToLocal(Android.Net.Uri.Parse(uriString));
        }

        private string CopyFileToLocal(Android.Net.Uri uri)
        {
            try
            {
                var context = Android.App.Application.Context;
                using (var inputStream = context.ContentResolver.OpenInputStream(uri))
                {
                    // Создаем уникальное имя файла
                    var fileName = $"book_{Guid.NewGuid()}.epub";
                    var localPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, fileName);

                    using (var outputStream = File.Create(localPath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                    return localPath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка копирования: {ex.Message}");
                return null;
            }
        }
    }
}
