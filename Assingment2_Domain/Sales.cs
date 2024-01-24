using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assignement3_Domain
{
    public class Sales
    {
        public int Id { get; set; }

        public string Total { get; set; }

        public bool IsDeleted { get; set; } = false;

         public int CustomerId { get; set; }// لاجل العلاقة مع الزبائن

        // السطرين التاليين لتصحيح الخطا التالي 
        //JsonSerializationException: Self referencing loop detected with type 'Assingment2_Domain.Sales'. Path 'customers.sales
        // وهذا الخطا ظهر عندي محاولتي لاضافة عملية بيع جديدة حيث عندما اراد 
        // اعادتها لي رأي الخاصية التالية التي في السطر 26 فذهب الى كلاس الزبون
        // ورأى هناك قائمة مبيعات فدخل في حلقة 
        [JsonIgnore]
        [IgnoreDataMember]
        public Customer Customer { get; set; }// لاجل العلاقة مع الزبائن
        
        public int CarId { get; set; }// لاجل العلاقة مع جدول السيارات
       
        [JsonIgnore]
        [IgnoreDataMember]
        public Car Car { get; set; }// لاجل العلاقة مع جدول السيارات

    }
}
