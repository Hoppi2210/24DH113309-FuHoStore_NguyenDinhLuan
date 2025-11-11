using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models
{
    public class CategoryMetadata
    {
        [Display(Name = "Mã danh mục")]
        public int CategoryID { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [Display(Name = "Mô tả danh mục")]
        public string Description { get; set; }
    }
}
