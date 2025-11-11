using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models.Metadata
{
    public class UserMetadata
    {
        [Display(Name = "Mã nhân viên")]
        public int UserID { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Quyền truy cập")]
        public string Role { get; set; }
    }
}
