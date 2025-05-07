using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class DocGia
    {
        [Key]
        public int SoTheDG { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public virtual ICollection<MuonSach> MuonSachs { get; set; }
    }
}
