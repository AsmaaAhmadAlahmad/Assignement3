using Assignement3_Domain;

namespace Assignement3_Domain.ApiModels
{
    public class CarForCreate
    {
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string Gear { get; set; }
        public double Km { get; set; }

    }
}
