using System;
using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models.Metadata
{
    public class CustomerMetadata
    {
        [Display(Name = "Mã KH")]
        public int CustomerID { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; }

        [StringLength(30)]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Tài khoản")]
        public string Username { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? DateCreated { get; set; }
    }
}
