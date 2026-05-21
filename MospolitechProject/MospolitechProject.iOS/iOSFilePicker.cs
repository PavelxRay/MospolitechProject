using System.Threading.Tasks;
using UIKit;
using Foundation;
using MospolitechProject.Services;
using Xamarin.Forms;

// Регистрация класса в DependencyService для вызова из общего кода
[assembly: Dependency(typeof(MospolitechProject.iOS.iOSFilePicker))]
namespace MospolitechProject.iOS
{
    public class iOSFilePicker : IFilePicker
    {
        public async Task<string> GetFileAsync()
        {
            // Идентификатор Uniform Type Identifier для контейнеров формата EPUB
            var allowedUtis = new string[] { "org.idpf.epub-container" };

            // Инициализация нативного контроллера проводника Apple
            var picker = new UIDocumentPickerViewController(allowedUtis, UIDocumentPickerMode.Import);

            // Логика вызова окна и асинхронный возврат локального пути к файлу 
            // из изолированной песочницы (Sandbox) приложения iOS
            return await Task.FromResult<string>(null);
        }
    }
}
