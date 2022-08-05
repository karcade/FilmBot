namespace TelegramBot.Common.DTO
{
    public class FilmDTO
    {
        //public int RatingPlace { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string Genre { get; set; } = String.Empty;
        public string Producer { get; set; } = String.Empty;
        public string Actor { get; set; } = String.Empty;

        public string Info()
        {
            return "Название: " + Name + "\nСтрана:" + Country + ". Жанр:" + Genre;
        }
    }
}
