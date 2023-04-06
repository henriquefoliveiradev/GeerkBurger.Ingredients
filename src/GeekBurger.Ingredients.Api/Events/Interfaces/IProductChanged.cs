using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Events.Interfaces
{
    public interface IProductChanged
    {
        Task ProcessMessages();
    }
}
