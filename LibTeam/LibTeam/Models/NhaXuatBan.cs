using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class NhaXuatBan
    {
        [Key]
        public int NhaXuatBanID { get; set; }  // Khóa chính tự tăng

        [Required, StringLength(150)]
        public string TenNXB { get; set; }

        [StringLength(200)]
        public string DiaChiNXB { get; set; }

        [StringLength(15)]
        public string SDTNXB { get; set; }

        // Quan hệ 1-n: Một nhà xuất bản có nhiều tựa sách
        public ICollection<TuaSach> TuaSaches { get; set; } = new HashSet<TuaSach>();
    }
}
