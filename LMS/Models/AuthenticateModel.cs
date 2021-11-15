using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class AuthenticateModel
    {
        [Required(ErrorMessage ="User Name is required.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
