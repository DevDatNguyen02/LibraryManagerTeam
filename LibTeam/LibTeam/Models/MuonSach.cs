using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibTeam.Models
{
    public class MuonSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MuonSachID { get; set; }

        // FK đến CuonSach
        [ForeignKey("CuonSach")]
        public int CuonSachID { get; set; }
        public CuonSach CuonSach { get; set; }

        // FK đến DocGia
        [ForeignKey("DocGia")]
        public int SoTheDG { get; set; }
        public DocGia DocGia { get; set; }

        [Required]
        public DateTime NgayMuon { get; set; }

        [Required]
        public DateTime NgayHenTra { get; set; }

        public DateTime? NgayTra { get; set; }


    }
}
