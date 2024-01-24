using Assignement3_Domain.ApiModels;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace Assignement3_Domain.Services
{
    //  من أجل الكلاس الأصلي T
    // ApiModels  من أجل الكلاس الذي يُستخدم للإضافة والمتواجد في مجلد  B
    // ApiModels  من أجل الكلاس الذي يُستخدم للتعديل والمتواجد في مجلد C
    public interface IGenericRepository<T, B, C> where T : class where B : class where C : class
    {
        // في الدالة التالية يتم الفلترة عن العنصر المطلوب بواسطة الآي دي داخل الفلتر 
        // حيث قمت بتمرير شرط خلال اقواس دالة الجلب في داخل كل متحكم ليتم جلب العنصر 
        // الذي مررنا الآي دي الذي يخصه يعني لم يتبقى حاجة لتمرير الآي دي كبارامتر في الدالة
        Task<T> GetItemByIdAsync(
          //  int id, 
            Expression<Func<T, bool>> filterIdAndIsDeleted = null // هذا البارامتر لوضع فلتر على البيانات
            , string includeProperties = ""); // هذا البارامتر لتضمين خصائص القوائم في الكلاسات

       Task<IEnumerable<T>> GetItemsAsync(
       Expression<Func<T, bool>> filter = null // هذا البارامتر لوضع فلتر على البيانات
       , string includeProperties = ""); // هذا البارامتر لتضمين خصائص القوائم في الكلاسات

        Task DeleteItemAsync(int id); // دالة حذف كائن 

        Task UpdateItemAsync(int id, C item); // دالة تعديل كائن

        Task<T> AddItemAsync(B item, int supplierId = 0, int customerId = 0, int carId = 0);

        Task UpdatePartiallyItemAsync(int id, JsonPatchDocument<C> patchDocument); // دالة تعديل جزئي

        Task save(); // دالة للحفظ في قاعدة البيانات

        Task<List<T>> AddRangeAsync(List<B> bs); // دالة اضافة قائمة لكائن ما

        
        Task<T> GetItemByIdForUpdateOrDeleteAsync(int id); // الدالة التالية فقط من اجل التأكد من تواجد العنصر قبل حذفه أو تعديله
                                                           // لأن العمليات على العناصر المحذوفة يجب أن تكون ممنوعة

    }
}