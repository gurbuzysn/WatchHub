using Web.Models;

namespace Web.Interfaces
{
    public interface IHomeViewModelService
    {
        Task<HomeViewModel> GetHomeViewModelAsync();
    }
}
    