using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AnalyticConfig.Models
{

    public class User
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string Customer { get; set; }

        [DisplayName("Skolenhet")]
        [Required(ErrorMessage = "{0} måste anges")]
        public string SchoolUnit { get; set; }

        [DisplayName("Namn")]
        [MaxLength(50)]
        [Required(ErrorMessage = "{0} måste vara ifyllt")]
        [RegularExpression(@"[^<>]*", ErrorMessage = "< och > är inte tillåtna tecken")]
        public string Name { get; set; }

        [DisplayName("Roll")]
        [Required(ErrorMessage = "{0} måste anges")]
        public string Role { get; set; }

        [DisplayName("Epost")]
        [Required(ErrorMessage = "{0} måste vara ifyllt")]
        [MaxLength(50)]
        [EmailAddress(ErrorMessage = "{0} måste vara av giltlig typ")]
        public string Email { get; set; }

        [DisplayName("Lösenord")]
        [Required(ErrorMessage = "{0} måste vara ifyllt")]
        [MaxLength(25, ErrorMessage = "Lösenordet måste vara max 25 tecken långt")]
        [MinLength(6, ErrorMessage = "Lösenordet måste vara minst 6 tecken långt")]
        [RegularExpression(@"^[^*<>]*$", ErrorMessage = "Asterisk och <> är inte tillåtna tecken i lösenordet")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} måste anges")]
        public IEnumerable<SelectListItem> RoleList
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Gy", Value = "Gy"},
                    new SelectListItem { Text = "Gr", Value = "Gr"},
                    new SelectListItem { Text = "Bo", Value = "Bo"},
                    new SelectListItem { Text = "Ekonomi", Value = "Ekonomi"},
                };
            }
        }

    }

}