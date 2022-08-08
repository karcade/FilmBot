using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;
using TelegtramBot.Model;
using TelegramBot.Common.Mapping;

namespace TelegramBot.BusinessLogic.Implementation
{
    public class FilmService:IFilmService
    {
        private ApplicationContext _context;
        private readonly IMapper _mapper;

        public FilmService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<FilmDTO> GetListByGenre(string genre)
        {
            List<Film> films = new List<Film>();
            
            films = _context.Films.Where(m => m.Genre.Contains(genre)).AsNoTracking().ToList();
            
            List<FilmDTO> filmsDTO = _mapper.Map<List<FilmDTO>>(films);
            return filmsDTO;
        }

        public FilmDTO Get(int id)
        {
            var film = new Film();

            film = _context.Films.AsNoTracking().FirstOrDefault(m => m.Id == id);
           
            FilmDTO filmDTO = _mapper.Map<FilmDTO>(film);
            return filmDTO;
        }
        
        public void Create(FilmDTO filmDTO)
        {
            var film = _mapper.Map<FilmDTO, Film>(filmDTO);

            _context.Films.Add(film);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.Films.Remove(GetById(id));
            _context.SaveChanges();
        }

        public Film GetById(int id)
        {
            if (!_context.Films.Any(x => x.Id == id)) throw new Exception("Film not found");
            return _context.Films.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Film> GetAll()
        {
            if (_context.Films == null) throw new Exception("Films not found");
            return _context.Films.AsNoTracking().ToList();
        }

        public FilmDTO GetRandomByGenre(string genre)
        {
            var films = GetListByGenre(" "+genre).ToList();
            FilmDTO film =films[new Random().Next(films.Count)];
            return film;
        }

        public List<FilmDTO> GetFixAmount(int take)
        {
            var films= _context.Films.Take(take).ToList();
            return  _mapper.Map<List<FilmDTO>>(films);
        }
    }
}
