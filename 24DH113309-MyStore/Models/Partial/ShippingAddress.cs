namespace _24DH113309_MyStore.Models
{
    public partial class ShippingAddress
    {
        public string FullAddress =>
            $"{DetailAddress}, {Ward}, {District}, {Province}";

        // Vì IsDefault là bool?
        public bool IsDefaultBool =>
            IsDefault ?? false;
    }
}

