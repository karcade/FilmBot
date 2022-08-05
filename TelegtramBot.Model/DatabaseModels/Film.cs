using System.ComponentModel.DataAnnotations;

namespace TelegramBot.Model.DatabaseModels
{
    public class Film
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Country { get; set; } = String.Empty;
        public string Genre { get; set; } = String.Empty;
        public string Producer { get; set; } = String.Empty;
        public string Actor { get; set; } = String.Empty;
    }
}
