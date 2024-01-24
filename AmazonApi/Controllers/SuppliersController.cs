using Assignement3_Domain.ApiModels;
using Assignement3_Domain.Services;
using Assignement3_Domain;
using Assignement3_Domain.Helper;
using Assingment3Api.ApiModels;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Assingment3Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User", Policy = "ShouldBeUser")]



    public class SuppliersController : ControllerBase
    {
        private readonly IGenericRepository<Supplier, SupplierForCreate, SupplierForUpdate> genericRepository;
        public SuppliersController(IGenericRepository<Supplier, SupplierForCreate, SupplierForUpdate> genericRepository)
        {
            this.genericRepository = genericRepository;
        }



        [HttpGet("{supplierId}", Name = "GetSupplier")]
        public async Task<ActionResult> GetSupplier(int supplierId) // ايند بوينت جلب منتج واحد بواسطة الآي دي
        {
            var supplier = await genericRepository.GetItemByIdAsync(
                filterIdAndIsDeleted: s => s.Id == supplierId && s.IsDeleted == false, // الفلترة لجلب الموّرد حسب الآي دي وأن يكون غبر محذوف
                includeProperties: "Parts"); // تضمين قائمة الأجزاء التي وردها المورد

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }





        [HttpGet]
        public async Task<ActionResult> GetSuppliers() // ايند بوينت لجلب كل الموّردين
        {
            var suppliers = await genericRepository.GetItemsAsync(
                filter:s=>s.IsDeleted==false, // الفلترة لجلب الموّردين الغير محذوفين فقط
                includeProperties:"Parts"); // تضمين قوائم القطع التي ورّدوها الموردين 

            if (!suppliers.Any())
                return NotFound();

            return Ok(suppliers);
        }




       

        [HttpDelete("{supplierId}")]
        public async Task<ActionResult> DeleteSupplier(int supplierId)
        {
            // جلب المورد المُراد حذفه للتأكد من تواجده في قاعدة البيانات
            var supplier = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(supplierId);

            // الشرط الثاني من أجل التأكد من أن المورد غير محذوف
            if (supplier == null || supplier.IsDeleted == true)
                return NotFound();

            // استدعاء دالة الحذف في حال كان الموّرد متواجد في القاعدة وليس محذوف
            await genericRepository.DeleteItemAsync(supplierId);
            return NoContent();
        }



        // في الايند بوينت التالية حقنت الجينريك ريبوزيتوري الخاصة بالأجزاء حتى استدعي الدالة   
        // التي ستقوم بإضافة قائمة اجزاء قام بتوريدها هذا المورد
        [HttpPost]
        public async Task<ActionResult> AddSupplier(
          [FromServices] IGenericRepository<Part, PartForCreateListOfPartsForSupplier, PartForUpdate> genericRepositorySupplier,
          [FromQuery] SupplierForCreate supplierForCreate,  // حيث body لأنه لايمكن تمرير بارامترين من [FromQuery] هذا البارمتر يتم تمريره بطريقة
          List<PartForCreateListOfPartsForSupplier> parts)  // هذا البارامتر سيتم تمريره من البادي


        {
            // اضافة المورد
            var supplier = await genericRepository.AddItemAsync(supplierForCreate);

            // اضافة قائمة اجزاء خاصة بهذا المورد  
            supplier.Parts = await genericRepositorySupplier.AddRangeAsync(parts);

            // استدعيت دالة الإضافة هنا وليس في الريبوزيتوري القيام بخطوة قبل الحفظ وهذه الخطوة هي
            // اضافة قائمة اجزاء للموّرد
            await genericRepository.save();

            return CreatedAtRoute("GetSupplier", new { supplierId = supplier.Id }, supplier);
        }




        [HttpPut("{supplierId}")]
        public async Task<ActionResult> UpdateSupplier(int supplierId
                                                       , SupplierForUpdate supplierForUpdate) // ايند بوينت تعديل موّرد
        {
            // جلب الموّرد المُراد تعديله للتأكد من وجوده في القاعدة
            var supplierThatWillUpdate = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(supplierId);

            // الشرط الثاني من أجل التأكد من أن المورد غير محذوف
            if (supplierThatWillUpdate == null || supplierThatWillUpdate.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل في حال كان المورد موجود في القاعدةو ليس محذوف
            await genericRepository.UpdateItemAsync(supplierId, supplierForUpdate);

            return NoContent();
        }



        [HttpPatch("{supplierId}")]
        public async Task<ActionResult> PartiallyUpdateSupplier(int supplierId, JsonPatchDocument<SupplierForUpdate> patchDocument)
        {
            // جلب الموّرد المُراد تعديله للتأكد من وجوده في القاعدة
            var supplierThatWillUpdate = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(supplierId);

            // الشرط الثاني من أجل التأكد من أن العنصر غير محذوف
            if (supplierThatWillUpdate == null || supplierThatWillUpdate.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل الجزئب في حال كان المورد موجود في القاعدة وليس محذوف
            await genericRepository.UpdatePartiallyItemAsync(supplierId, patchDocument);

            return NoContent();
        }

    }
}
