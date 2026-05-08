using System.Threading.Tasks;

namespace MospolitechProject.Services
{
    public interface IFilePicker
    {
        Task<string> GetFileAsync();
    }
}