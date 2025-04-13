using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class TaiKhoanModel
    {
        [Key]
        public string Id { get; set; }  // Sử dụng kiểu string để tương thích với Identity

        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email của bạn!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu của bạn!")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn vai trò!")]
        public string Role { get; set; }

    }
}
