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
            using (ApplicationContext context = new ApplicationContext())
            {
                films = context.Films.Where(m => EF.Functions.Like(m.Genre, $"%{genre}%")).AsNoTracking().ToList();
            }
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

            film.Name = filmDTO.Name;
            film.Country = filmDTO.Country;
            film.Genre = filmDTO.Genre;
            film.Producer = filmDTO.Producer;
            film.Actor = filmDTO.Actor;
            film.LinkPoster = filmDTO.LinkPoster;
            film.Link = filmDTO.Link;

            _context.Films.Add(film);
            _context.SaveChanges();
        }

        public List<Film> GetList()
        {
            return _context.Films.AsNoTracking().ToList();
        }

        public void Delete(int id)
        {
            _context.Films.Remove(GetById(id).FirstOrDefault());
            _context.SaveChanges();
        }

        public IQueryable<Film> GetById(int id)
        {
            if (!_context.Films.Any(x => x.Id == id)) throw new Exception("Film not found");
            return _context.Films.Where(x => x.Id == id);
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
    }
}
