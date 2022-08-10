using AutoMapper;
using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;

namespace TelegramBot.BusinessLogic.Interfaces
{
    public interface IFilmService
    {
        FilmDTO Get(int id);
        Film GetById(int id);
        IEnumerable<Film> GetAll();
        void Create(FilmDTO filmDTO);
        void Delete(int id);
        IEnumerable<FilmDTO> GetListByGenre(string genre);
        FilmDTO GetRandomByGenre(string genre);
        List<FilmDTO> GetFixAmount(int take);
    }
}
