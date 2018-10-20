using server.Repositories;
using System.Threading.Tasks;

namespace server.Services
{
    public interface ISeedDataService
    {
        Task Initialize(FoodDbContext context);
    }
}
