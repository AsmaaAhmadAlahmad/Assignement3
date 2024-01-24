namespace Assingment3Api.ApiModels
{
    public class SalesForCreateListOfSalesForCustomer
    {
        // هذا الكلاس قمت بإنشائه من اجل عند انشاء قائمة مبيعات للزبون وهذا الكلاس  لا يحوي خاصية رقم الزبون
        //  ولن  يُطلب من المستخدم إدخال رقم الزبون 
        // تتواجد خاصية رقم الزبون وبالتالي  SalesForCreate حيث في كلاس 
        // SalesForCreate يتم طلب إدخال رقم الزبون من المستخدم لذلك لم أستخدم 
        public string Total { get; set; }
        public int CarId { get; set; }
    }
}
