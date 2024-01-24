using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Assignement3_Domain
{
    public class Car
    {
        
        public int Id { get; set; }
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string Gear { get; set; }
        public double Km { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Sales Sales { get; set; }  // لاجل العلاقة مع جدول المبيعات 
        public List<Part> Parts { get; set; } = new List<Part>(); // لاجل العلاقة مع جدول القطع

    }
}