using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assignement3_Domain
{
    public class Part
    {
        public int Id { get; set; }
        public string Name {get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;


        // سبب وضع السطرين السفليين موجود في هذا الرابط 
        // https://www.notion.so/0078150c488743eb97c8f5b1559e2667
        // حيث السبب هو حل لمشكلة 
        // Self referencing loop detected with type 'Assingment2_Domain.Car'. Path 'parts[0].cars'.
        // التي ظهرت عندما اضفت سيارة جديدة مع قائمة اجزاء خاصة بها
        // حيث عندما اراد ان يعيد هذه السيارة المضافة مع قائمة اجزائها
        //في الاستجابة جاء الى هنا ورأى قائمة سيارات فأصبحت حلقة وحدث خطا
        [JsonIgnore]
        [IgnoreDataMember]
        public List<Car> Cars { get; set; } = new List<Car>();  // لاجل العلاقة مع جدول السيارات
        public int SupplierId { get; set; }    // لاجل العلاقة مع جدول الموردين 


        // هذين السطرين السفليين هما حل لمشكلة 
        // Self referencing loop detected with type 'Assingment2_Domain.Part'. Path 'parts[0].supplier.parts'.
        // التي ظهرت عندما اضفت سيارة جديدة حيث عندما أراد أن يعيد هذه السيارة
        // Supplier في الاستجابة جاء الى الخاصية السفلية التي نوعها 
        // فذهب ليجلب المورد الذي ورد هذا الجزء فرأى انه يملك قائمة اجزاء ايضا 
        // فدخل في حلقة وظهر الخطا

        // وايضا حل للمشكلة التالية 
        // Self referencing loop detected for property 'supplier' with type 'Assingment2_Domain.Supplier'. Path 'parts[0]'.
        // التي تظهر عندما أضيف مورد جديد مع قائمة اجزاء خاصة به 
        [JsonIgnore]
        [IgnoreDataMember]
        public Supplier? Supplier { get; set; } // لاجل العلاقة مع جدول الموردين
    }
}
