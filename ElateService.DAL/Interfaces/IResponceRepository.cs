using ElateService.DAL.Entities;
using System.Threading.Tasks;

namespace ElateService.DAL.Interfaces
{
    public interface IResponceRepository
    {
        Task Create(Responce responce);
    }
}
