using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnalyticConfig.Models
{
    public class SystemAdmin
    {
        public int Id { get; set; }
        [DisplayName("Kund id")]
        public int CustomerID { get; set; }
        [DisplayName("Kund")]
        [Required(ErrorMessage = "{0} måste anges")]
        public string Customer { get; set; }
        [DisplayName("Användarnamn")]
        [Required(ErrorMessage = "{0} måste anges")]
        [RegularExpression(@"[A-Za-z0-9._()\[\]-]{3,20}")]
        public string Username { get; set; }
        [DisplayName("Epost")]
        [Required(ErrorMessage = "{0} måste anges")]
        public string Email { get; set; }
        [DisplayName("Lösenord")]
        [MaxLength(25, ErrorMessage = "Lösenordet måste vara max 25 tecken långt")]
        [MinLength(6, ErrorMessage = "Lösenordet måste vara minst 6 tecken långt")]
        [RegularExpression(@"^[^*<>]*$", ErrorMessage = "Asterisk och <> är inte tillåtna tecken i lösenordet")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Roller")]
        public string Role { get; set; }
    }
}