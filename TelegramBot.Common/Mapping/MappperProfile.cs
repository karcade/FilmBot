using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;

namespace TelegramBot.Common.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Film, FilmDTO>().ReverseMap());
            var mapper = new Mapper(config);
            CreateMap<Film, FilmDTO>().ReverseMap();
        }
    }
}
