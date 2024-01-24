//using AmazonApi.Services;
using Assignement3_Domain;

namespace Assignement3_Domain.ApiModels
{
    public class PartForCreate
    {
       
        public string Name { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }

        public int SupplierId { get; set; }

    }
}
