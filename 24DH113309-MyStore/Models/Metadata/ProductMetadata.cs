using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models.Metadata
{
    public class ProductMetadata
    {
        [Display(Name = "Mã sản phẩm")]
        public int ProductID { get; set; }

        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Mô tả")]
        public string ProductDecription { get; set; }

        [Required, Range(0, 1000000000)]
        [Display(Name = "Giá")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ProductImage { get; set; }
    }
}
