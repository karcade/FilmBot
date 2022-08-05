using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;
using TelegtramBot.Model;

namespace TelegramBot.BusinessLogic.Implementation
{
    public class FilmService:IFilmService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public FilmService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        const int TOP_AMOUNT= 15;
        public void Create(FilmDTO fitmDTO)
        {
            var film = _mapper.Map<FilmDTO, Film>(fitmDTO);

            film.Name = fitmDTO.Name;
            film.Country = fitmDTO.Country;
            film.Genre = fitmDTO.Genre;
            film.Producer = fitmDTO.Producer;
            film.Actor = fitmDTO.Actor;

            _context.Films.Add(film);
            _context.SaveChanges();
        }

        public List<Film> GetList()
        {
            return _context.Films.ToList();
        }

        public List<Film> GetListByGenre(string genre)
        {
            if (!_context.Films.Any(x => x.Genre == genre)) throw new Exception("Films not found.");
            return _context.Films.Where(x => x.Genre == genre).ToList();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public FilmDTO Get(int id)
        {
            if (!_context.Films.Any(x => x.Id == id)) throw new Exception("Film not found.");

            return _mapper.Map<Film, FilmDTO>(GetById(id).FirstOrDefault());
        }
        public IQueryable<Film> GetById(int id)
        {
            if (!_context.Films.Any(x => x.Id == id)) throw new Exception("Film not found.");
            return _context.Films.Where(x => x.Id == id);
        }
       
        public IQueryable<Film> GetByGenre(string genre)
        {
            if (!_context.Films.Any(x => x.Genre == genre)) throw new Exception("Films not found.");
            return _context.Films.Where(x => x.Genre == genre);
        }

        public IEnumerable<Film> GetAll()
        {
            if (_context.Films == null) throw new Exception("Films not found.");
            return _context.Films.ToList();
        }

        public List<Film> GetTop15()
        {
            List<Film> filmList = new List<Film>();
            var film = new Film();

            for (int i = 0; i < TOP_AMOUNT; i++)
            {
                film = (Film)GetById(i);
                filmList.Add(film);
            }
            return filmList;
        }

        public string GetTop15ToString()
        {
            List<Film> filmList = GetTop15();
            string info="";
            foreach(var film in filmList)
            {
               info+="Название: " + film.Name + "\nСтрана:" + film.Country + ". Жанр:" + film.Genre;
            }
            return info;
        }
    }
}
