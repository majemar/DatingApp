using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{

    //DTOs odpowiada za konwersję danych przysłanych w formacie JSON na wartości nam potrzebne
    // czyli username i password, konwersja jest automatyczna wykonana przez EF
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify pass between 4 and 8 char")]
        public string Password { get; set; }
    }
}