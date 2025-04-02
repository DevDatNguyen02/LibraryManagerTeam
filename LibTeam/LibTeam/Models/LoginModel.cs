using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email của bạn!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu của bạn!")]
        public string Password { get; set; }
    }
}
