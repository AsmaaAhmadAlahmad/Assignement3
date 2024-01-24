using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Assignement3_Domain
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<Part> Parts { get; set; } = new List<Part>(); // لاجل العلاقة مع جدول القطع
    }
}
