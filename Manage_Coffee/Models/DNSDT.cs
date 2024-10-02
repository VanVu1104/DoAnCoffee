using System.ComponentModel.DataAnnotations;

namespace Manage_Coffee.Models
{
    public class DNSDT
    {
        [Required]
        public int Sdt { get; set; } // Số điện thoại

        [Required]
        [DataType(DataType.Password)]
        public string Matkhau { get; set; }
    }
}
