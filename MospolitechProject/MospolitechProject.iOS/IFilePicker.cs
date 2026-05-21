using System.Threading.Tasks;

namespace MospolitechProject.Services
{
    // Общий контракт, абстрагирующий UI от логики конкретной ОС
    public interface IFilePicker
    {
        Task<string> GetFileAsync();
    }
}
