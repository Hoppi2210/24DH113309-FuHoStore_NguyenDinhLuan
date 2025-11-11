using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using _24DH113309_MyStore.Models.Metadata;   // ✅ import metadata namespace

namespace _24DH113309_MyStore.Models
{
    // ===============================
    // 🔹 PRODUCT PARTIAL CLASS
    // ===============================
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
        [NotMapped]
        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(ProductDecription)) return "";
                return ProductDecription.Length > 100
                    ? ProductDecription.Substring(0, 100) + "..."
                    : ProductDecription;
            }
        }
    }

    // ===============================
    //  CUSTOMER PARTIAL CLASS
    // ===============================
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
        [NotMapped]
        public string DisplayName => $"{CustomerName} ({CustomerEmail})";
    }

    // ===============================
    // 🔹 USER PARTIAL CLASS
    // ===============================
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        [NotMapped]
        public string RoleDescription
        {
            get
            {
                switch (UserRole)
                {
                    case "Admin": return "Quản trị viên";
                    case "Staff": return "Nhân viên";
                    default: return "Khách hàng";
                }
            }
        }
    }

    // ===============================
    // 🔹 ORDER PARTIAL CLASS
    // ===============================
    public partial class Order
    {
        [NotMapped]
        public string StatusText
        {
            get
            {
                if (PaymentStatus == null)
                    return "Không xác định";
                return PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase)
                    ? "Đã thanh toán"
                    : "Chưa thanh toán";
            }
        }
    }
}
