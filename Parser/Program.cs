using HtmlAgilityPack;
using TelegramBot.Common;
using TelegramBot.BusinessLogic.Implementation;
using TelegtramBot.Model;
using TelegramBot.Model.DatabaseModels;
using AutoMapper;
using System.Collections;
using Microsoft.EntityFrameworkCore;

string html = "https://www.kinonews.ru/top100/";
HtmlWeb web = new HtmlWeb();
HtmlDocument document = web.Load(html);

var FilmListItems = document.DocumentNode.Descendants("div")
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

List<Film> filmList = new List<Film>();

using (ApplicationContext context = new ApplicationContext())
{
    int i = 0;
        foreach (var FilmListItem in FilmListItems)
        {
            Film film = new Film();

            film.Link = (string)listLink[i];
            film.LinkPoster = (string)listPoster[i];
            i++;
            //header
            var FilmListItemHeader = FilmListItem.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("bigtext")).FirstOrDefault();

            //name
            string? FilmListItemName = FilmListItemHeader.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("titlefilm")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t');
            film.Name = FilmListItemName;

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
            film.Country = parseFilmCountry[0];

            //genre
            var FilmGenreString = FilmListItemTextGray[1].InnerText;
            string[] parseFilmGenreString = FilmGenreString.Split(':');
            string[] parseFilmGenre = parseFilmGenreString[1].Split(", ");
            film.Genre = parseFilmGenre[0];

            //producer
            var FilmProducerString = FilmListItemTextGray[2].InnerText;
            string[] parseFilmProducerString = FilmProducerString.Split(": ");
            film.Producer = parseFilmProducerString[1];

            //actor
            var FilmActorString = FilmListItemTextGray[3].InnerText;
            string[] parseFilmActorString = FilmActorString.Split(':');
            string[] parseFilmActor = parseFilmActorString[1].Split(", ");
            film.Actor = parseFilmActor[0];

            filmList.Add(film);

            context.Films.Add(film);
        }
    context.SaveChanges();
}

using (ApplicationContext context = new ApplicationContext())
{
    var films = context.Films.AsNoTracking().ToList();
    Console.WriteLine("Данные после добавления:");
    foreach(Film film in films)
    {
        Console.WriteLine($"{film.Id}.{film.Name} - {film.Country}- {film.LinkPoster}- {film.Link}");
    }
}
