using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnalyticConfig.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Användarnamn")]
        [Required(ErrorMessage = "{0} måste anges")]
        public string Username { get; set; }

        [DisplayName("Lösenord")]
        [Required(ErrorMessage = "{0} måste anges")]
        [MaxLength(25, ErrorMessage = "Lösenordet måste vara max 25 tecken långt")]
        [RegularExpression(@"^[^*]*$", ErrorMessage = "Asterisk är inte tillåtet i lösenordet")]
        public string Password { get; set; }

    }
}