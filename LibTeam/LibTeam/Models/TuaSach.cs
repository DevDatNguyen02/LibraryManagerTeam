using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibTeam.Models
{
    public class TuaSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TuaSachID { get; set; }

        [Required]
        public string TenTuaSach { get; set; }

        [Required]
        public string MoTa { get; set; }

        // Quan hệ 1-n với CuonSach
        public ICollection<CuonSach> CuonSaches { get; set; }
    }
}
