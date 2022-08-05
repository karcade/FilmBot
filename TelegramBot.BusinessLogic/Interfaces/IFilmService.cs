using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;

namespace TelegramBot.BusinessLogic.Interfaces
{
    public interface IFilmService
    {
        FilmDTO Get(int id);
        IQueryable<Film> GetById(int id);
        IQueryable<Film> GetByGenre(string genre);
        IEnumerable<Film> GetAll();
        void Create(FilmDTO fitmDTO);
        List<Film> GetList();
        List<Film> GetListByGenre(string genre);
        void Delete(int id);
        List<Film> GetTop15();
        string GetTop15ToString();
    }
}
