using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class DocGia
    {
        [Key]
        public int SoTheDG { get; set; }  // Mã thẻ độc giả

        [Required, StringLength(100)]
        public string TenDG { get; set; }

        [StringLength(200)]
        public string DiaChiDG { get; set; }

        [StringLength(15)]
        public string SDTDG { get; set; }
    }
}
