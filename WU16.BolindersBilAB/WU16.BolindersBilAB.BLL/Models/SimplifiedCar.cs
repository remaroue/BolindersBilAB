namespace WU16.BolindersBilAB.BLL.Models
{
    public class SimplifiedCar
    {
        public string Title { get; set; }
        public string LicenseNumber { get; set; }
        public string ImageName { get; set; }
        public decimal? Price { get; internal set; }
    }
}