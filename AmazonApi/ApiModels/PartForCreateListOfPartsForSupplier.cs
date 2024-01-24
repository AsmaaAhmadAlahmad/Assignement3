namespace Assingment3Api.ApiModels
{

    // هذا الكلاس قمت بإنشائه من اجل عند انشاء قائمة اجزاء للمورد ، وهذا الكلاس لا يحوي خاصية رقم المورد
    //  ولن  يُطلب من الزبون إدخال رقم المورد 
    // تتواجد خاصية رقم المورد وبالتالي  PartForCreate حيث في كلاس 
    // PartForCreate يتم طلب إدخال رقم المورد من الزبون لذلك لم أستخدم 
    public class PartForCreateListOfPartsForSupplier
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
    }
}
