using HtmlAgilityPack;
using TelegramBot.Common.DTO;
using TelegramBot.BusinessLogic.Implementation;
using TelegtramBot.Model;
using TelegramBot.Model.DatabaseModels;
using AutoMapper;

string html = "https://www.kinonews.ru/top100/";
HtmlDocument document = new HtmlDocument();
HtmlWeb web = new HtmlWeb();
document = web.Load(html);

List<FilmDTO> filmDTOsList = new List<FilmDTO>();

var FilmListItems = document.DocumentNode.Descendants("div")
    .Where(node => node.GetAttributeValue("style", "")
    .Equals("overflow:auto;")).ToList();

using (ApplicationContext context = new ApplicationContext())
{
    foreach (var FilmListItem in FilmListItems)
    {
        FilmDTO filmDTO = new FilmDTO();

        //header
        var FilmListItemHeader = FilmListItem.Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
            .Contains("bigtext")).FirstOrDefault();

        //rating place
        /*string FilmId = FilmListItemHeader.Descendants("b")
            .FirstOrDefault().InnerText.Trim('\r', '\n', '\t');
        FilmId = FilmId.Remove(FilmId.Length - 1);
        filmDTO.Id = Convert.ToInt32(FilmId);*/

        //name
        string? FilmListItemName = FilmListItemHeader.Descendants("a")
            .Where(node => node.GetAttributeValue("class", "")
            .Contains("titlefilm")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');
        filmDTO.Name = FilmListItemName;

        //body
        var FilmListItemBody = FilmListItem.Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
            .Contains("relative")).FirstOrDefault();

        var FilmListItemRatingRightDesc = FilmListItemBody.Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
            .Contains("rating_rightdesc")).FirstOrDefault();

        var FilmListItemTextGray = FilmListItemRatingRightDesc.Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
            .Contains("textgray")).ToList();

        //country
        var FilmCountryString = FilmListItemTextGray[0].InnerText;
        string[] parseFilmCountryString = FilmCountryString.Split(':');
        string[] parseFilmCountry = parseFilmCountryString[1].Split(", ");
        filmDTO.Country = parseFilmCountry[0];

        //genre
        var FilmGenreString = FilmListItemTextGray[1].InnerText;
        string[] parseFilmGenreString = FilmGenreString.Split(':');
        string[] parseFilmGenre = parseFilmGenreString[1].Split(", ");
        filmDTO.Genre = parseFilmGenre[0];

        //producer
        var FilmProducerString = FilmListItemTextGray[2].InnerText;
        string[] parseFilmProducerString = FilmProducerString.Split(": ");
        filmDTO.Producer = parseFilmProducerString[1];

        //actor
        var FilmActorString = FilmListItemTextGray[3].InnerText;
        string[] parseFilmActorString = FilmActorString.Split(':');
        string[] parseFilmActor = parseFilmActorString[1].Split(", ");
        filmDTO.Actor = parseFilmActor[0];

        var config = new MapperConfiguration(cfg => cfg.CreateMap<Film, FilmDTO>().ReverseMap());
        var mapper = new Mapper(config);
        Film film = mapper.Map<FilmDTO, Film>(filmDTO);
        context.Films.Add(film);
        //filmDTOservice.Create(new FilmDTO() { Id = Convert.ToInt32(FilmId), Name = FilmListItemName, Country = parseFilmCountry[0], Genre = parseFilmGenre[0], Producer = parseFilmProducerString[1], Actor = parseFilmActor[0] });

        filmDTOsList.Add(filmDTO);
    }
    context.SaveChanges();
}

foreach(FilmDTO filmDTO in filmDTOsList)
{
    Console.WriteLine(filmDTO.Info());
}