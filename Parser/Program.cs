using HtmlAgilityPack;
using TelegramBot.BusinessLogic.Implementation;
using TelegtramBot.Model;
using AutoMapper;
using System.Collections;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.DTO;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Common.Mapping;

var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IFilmService, FilmService>()
            .BuildServiceProvider();
var mapperConfiguration = new MapperConfiguration(x =>
{
    x.AddProfile<MapperProfile>();
});
mapperConfiguration.AssertConfigurationIsValid();
IMapper mapper = mapperConfiguration.CreateMapper();

ApplicationContext context = new ApplicationContext();
IFilmService filmService = new FilmService(context, mapper);

string html = "https://www.kinonews.ru/top100/";
HtmlWeb web = new HtmlWeb();
HtmlDocument document = web.Load(html);

var FilmDTOListItems = document.DocumentNode.Descendants("div")
    .Where(node => node.GetAttributeValue("style", "")
    .Equals("overflow:auto;")).ToList();

ArrayList listLink = new ArrayList();
ArrayList listPoster = new ArrayList();

foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'rating_leftposter')]//a[@href]"))
{
    listLink.Add("https://www.kinonews.ru" + node.GetAttributeValue("href", null));
}
foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[contains(@class, 'rating_leftposter')]//img[@src]"))
{
    listPoster.Add("https://www.kinonews.ru" + node.GetAttributeValue("src", null));
}

List<FilmDTO> filmList = new List<FilmDTO>();

int i = 0;
        foreach (var FilmDTOListItem in FilmDTOListItems)
        {
            FilmDTO film = new FilmDTO();

            film.Link = (string)listLink[i];
            film.LinkPoster = (string)listPoster[i];
            i++;
            //header
            var FilmDTOListItemHeader = FilmDTOListItem.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("bigtext")).FirstOrDefault();

            //name
            string FilmDTOListItemName = FilmDTOListItemHeader.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("titlefilm")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');
            film.Name = FilmDTOListItemName;

            //body
            var FilmDTOListItemBody = FilmDTOListItem.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("relative")).FirstOrDefault();

            var FilmDTOListItemRatingRightDesc = FilmDTOListItemBody.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("rating_rightdesc")).FirstOrDefault();

            var FilmDTOListItemTextGray = FilmDTOListItemRatingRightDesc.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("textgray")).ToList();

            //country
            var FilmDTOCountryString = FilmDTOListItemTextGray[0].InnerText;
            string[] parseFilmDTOCountryString = FilmDTOCountryString.Split(':');
            string[] parseFilmDTOCountry = parseFilmDTOCountryString[1].Split(", ");
            film.Country = parseFilmDTOCountry[0];

            //genre
            var FilmDTOGenreString = FilmDTOListItemTextGray[1].InnerText;
            string[] parseFilmDTOGenreString = FilmDTOGenreString.Split(':');
            string[] parseFilmDTOGenre = parseFilmDTOGenreString[1].Split(", ");
            film.Genre = parseFilmDTOGenre[0];

            //producer
            var FilmDTOProducerString = FilmDTOListItemTextGray[2].InnerText;
            string[] parseFilmDTOProducerString = FilmDTOProducerString.Split(": ");
            film.Producer = parseFilmDTOProducerString[1];

            //actor
            var FilmDTOActorString = FilmDTOListItemTextGray[3].InnerText;
            string[] parseFilmDTOActorString = FilmDTOActorString.Split(':');
            string[] parseFilmDTOActor = parseFilmDTOActorString[1].Split(", ");
            film.Actor = parseFilmDTOActor[0];

            filmList.Add(film);
        }

foreach (var film in filmList)
{
    filmService.Create(film);
    Console.WriteLine(film.Name + " " + film.Genre);
}

