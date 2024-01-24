using Assignement3_Domain.ApiModels;
using Assignement3_Domain.Services;
using Assignement3_Data;
using Assignement3_Domain;
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




    public class PartsController : ControllerBase
    {
        private readonly IGenericRepository<Part, PartForCreate, PartForUpdate> genericRepository;

        public PartsController(IGenericRepository<Part, PartForCreate, PartForUpdate> genericRepository)
        {
            this.genericRepository = genericRepository;
        }



        [HttpGet("{partId}", Name = "GetPart")]
        public async Task<ActionResult> GetPart(int partId)  // ايند بوينت لجلب قطعة بحسب الآي دي 
        {
            var part = await genericRepository.GetItemByIdAsync(
                filterIdAndIsDeleted: p => p.Id == partId && p.IsDeleted == false); // الفلترة لجلب القطعة بحسب الآي دي 
                                                                                    // وأن تكون غبر محذوفة
            if (part == null)
                return NotFound();

            return Ok(part);
        }





        [HttpGet]
        public async Task<ActionResult> GetParts() // ايند بوينت جلب كل القطع
        {
            var parts = await genericRepository.GetItemsAsync(
                filter: c => c.IsDeleted == false // الفلترة لجلب القطع الغير محذوفة فقط
             // ,includeProperties: "Cars" // لن يتم تضمين قائمة السيارات لكل قطعة بسبب وجود السمات 
                                           //  [JsonIgnore] [IgnoreDataMember]
                                           // 🥺 في كلاس القطع وهاتين الخاصيتين لا يمكنني الاستغناء عنهما Cars فوق الخاصية 
                                           // وإلا سيظهر خطا اخر مشروح في كلاس القطع
              );

            if (!parts.Any())
                return NotFound();

            return Ok(parts);
        }




        

        [HttpDelete("{partId}")]
        public async Task<ActionResult> DeletePart(int partId) // ايند بوينت لحذف قطعة
        {
           // جلب القطعة للتأكد من تواجدها في قاعدة البيانات
            var part = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(partId);

            // الشرط الثاني للتأكد بأن الجزء غير محذوف
            if (part == null || part.IsDeleted == true)
                return NotFound();

            // استدعاء دالة الحذف في حال كان الجزء متواجد في القاعدة و غير محذوف
            await genericRepository.DeleteItemAsync(partId);

            return NoContent();
        }



        // في الايند بوينت التالية تم حقن الجينريك ريبوزيتوري الخاصة بالموردين لكي يتم التأكد  
        // من وجود المورد الذي قام بتوريد هذه القطعة 

        // وتم حقن الجينريك ريبوزيتوري الخاصة بالسيارات لإضافة حتى أستدعي الدالة التي ستقوم
        // بإضافة قائمة السيارات التي يتواجد فيها هذا الجزء الذي سيتم إضافته
        [HttpPost]
        public async Task<ActionResult> AddPart([FromServices] IGenericRepository<Supplier,SupplierForCreate, SupplierForUpdate> genericRepositorySupplier
            , [FromServices] IGenericRepository<Car,CarForCreate,CarForUpdate> genericRepositoryCar
            , [FromQuery] PartForCreate partForCreate // حيث body لأنه لايمكن تمرير بارامترين من [FromQuery] هذا البارمتر يتم تمريره بطريقة
            , List<CarForCreate> cars)                // هذا البارامتر سيتم تمريره من البادي
        {
            // اضافة جزء
            var ThePartThatWasAdded = await genericRepository.AddItemAsync(partForCreate);

            // جلب الموّرد  الذي ورّد هذا الجزء للتأكد من تواجده في قاعدة البيانات 
            var supplier = await genericRepositorySupplier.GetItemByIdForUpdateOrDeleteAsync(ThePartThatWasAdded.SupplierId);

            // الشرط الثاني للتأكد أن المورد ليس محذوف
            if (supplier == null || supplier.IsDeleted == true)
                return NotFound();

            // اضافة قائمة سيارات يتواجد فيها هذا الجزء الذي تمت إضافته 
            ThePartThatWasAdded.Cars =await genericRepositoryCar.AddRangeAsync(cars);

            // هذه الدالة التالية تحوي دالة حفظ التغييرات في القاعدة 
            // حيث وضعتها بهذا الشكل حتى لا أقوم بحقن كونتكست القاعدة هنا 
            // await context1.SaveChangesAsync() من اجل كتابة 
            // وقد قمت باستدعاء دالة الحفظ هنا وليس في الريبوزيتوري لأنني احتجت لفعل خطوات قبل الحفظ
            // و هذه الخطوات هي اضافة قائمة سيارات يتواجد فيها هذا الجزء والتأكد من تواجد المورد الذي ورد هذا الجزء 
            await genericRepository.save();

            return CreatedAtRoute("GetPart", new { partId = ThePartThatWasAdded.Id }, ThePartThatWasAdded);
        }




        [HttpPut("{partId}")]
        public async Task<ActionResult> UpdatePart(int partId, PartForUpdate partForUpdate) // ايند بوينت تعديل جزء
        {
            // جلب الجزء للتأكد من تواجده في قاعدة البيانات
            var partThatWillUpdate = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(partId);

            // الشرط الثاني للتأكد بأن الجزء غير محذوف
            if (partThatWillUpdate == null || partThatWillUpdate.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل في حال كان الجزء متواجد في القاعدة وليس محذوف
            await genericRepository.UpdateItemAsync(partId, partForUpdate);

            return NoContent();
        }



        [HttpPatch("{partId}")]
        public async Task<ActionResult> PartiallyUpdatePart(int partId, JsonPatchDocument<PartForUpdate> patchDocument)
        {
            // جلب الجزء للتأكد من تواجده في قاعدة البيانات
            var partThatWillUpdate = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(partId);

            // الشرط الثاني للتأكد بأن الجزء غير محذوف
            if (partThatWillUpdate == null || partThatWillUpdate.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل الجزئي في حال كان الجزء متواجد في القاعدة وليس محذوف
            await genericRepository.UpdatePartiallyItemAsync(partId, patchDocument);

            return NoContent();
        }

    }
}
