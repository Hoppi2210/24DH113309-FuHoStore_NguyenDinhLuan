namespace _24DH113309_MyStore.Models.ViewModels
{
    public class ShippingAddressVM
    {
        public string FullName { get; set; }
        public string Phone { get; set; }

        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }

        public string AddressDetail { get; set; }
        public string AddressType { get; set; } // Home / Company
        public bool IsDefault { get; set; }
    }
}
