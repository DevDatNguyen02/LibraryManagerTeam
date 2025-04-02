using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class SachTacGia
    {
        [Key]
        public int SachTacGiaID { get; set; }  // Khóa chính tự tăng

        [Required]
        public int TuaSachID { get; set; }     // Khóa ngoại đến TuaSach

        [Required, StringLength(100)]
        public string TenTacGia { get; set; }

        // Quan hệ điều hướng
        public TuaSach TuaSach { get; set; }
    }
}
