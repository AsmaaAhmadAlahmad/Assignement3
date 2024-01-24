using Assignement3_Domain.ApiModels;
using Assignement3_Domain.Services;
using Assignement3_Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Assingment3Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User" , Policy = "ShouldBeUser")]


    public class CarsController : ControllerBase
    {
        private readonly IGenericRepository<Car, CarForCreate, CarForUpdate> repositoryTest;

        public CarsController(IGenericRepository<Car, CarForCreate, CarForUpdate> repositoryTest)
        {
            this.repositoryTest = repositoryTest;
        }


       



        [HttpGet("{carId}", Name = "GetCar")]
        public async Task<ActionResult> GetCar(int carId) // ايند بوينت جلب سيارة واحدة بواسطة الاي دي
        {
            var car = await repositoryTest.GetItemByIdAsync(
                filterIdAndIsDeleted:c=>c.IsDeleted!=true && c.Id== carId // الفلترة لجلب السيارة حسب الآي دي وأن تكون غبر محذوفة
                , includeProperties:"Parts"); //  تضمين قائمة الأجزاء للسيارة 

            if (car == null)
                return NotFound();

            return Ok(car);
        }




        [HttpGet]
        public async Task<ActionResult> GetCars() // ايند بوينت جلب كل السيارات
        {
            var cars = await repositoryTest.GetItemsAsync(filter: w => w.IsDeleted == false // فلترة لجلب السيارات الغير محذوفة فقط
                                                           , includeProperties: "Parts"); // تضمين قوائم الأجزاء

            if (!cars.Any())
                return NotFound();

            return Ok(cars);
        }






        [HttpDelete("{carId}")]
        public async Task<ActionResult> DeleteCar(int carId) // ايند بوينت حذف سيارة
        {
            // جلب السيارة المُراد حذفها للتأكد من تواجدها في قاعدة البيانات
            var car = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(carId); 

            // الشرط الثاني للتأكد أن السيارة ليست محذوفة 
            if (car == null || car.IsDeleted == true)
                return NotFound();

            // استدعاء دالة الحذف في حال كانت السيارة ليست محذوفة وموجودة في القاعدة
            await repositoryTest.DeleteItemAsync(carId);

            return NoContent();
        }







        // في الايند بوينت التالية حقنت الجينريك ريبوزيتوري الخاصة بالأجزاء حتى استدعي الدالة   
        // التي ستقوم بإضافة قائمة الاجزاء الخاصة بهذه السيارة التي سيتم اضافتها

        // و حقنت الجينريك ريبوزيتوري الخاصة بالموردين  حتى استدعي الدالة     
        // التي ستقوم بالتأكد من تواجد المورد الذي ورّد هذا الجزء 
        [HttpPost]
        public async Task<ActionResult> AddCar(
               [FromServices] IGenericRepository<Part, PartForCreate, PartForUpdate> genericRepositoryPart,
               [FromServices] IGenericRepository<Supplier, SupplierForCreate, SupplierForUpdate> genericRepositorySupplier,
               [FromQuery] CarForCreate carForCreate, // حيث body لأنه لايمكن تمرير بارامترين من [FromQuery] هذا البارمتر يتم تمريره بطريقة
               List<PartForCreate> parts)             // هذا البارامتر سيتم تمريره من البادي
        {
            // اضافة السيارة
            var car = await repositoryTest.AddItemAsync(carForCreate);

            // اضافة  قائمة أجزاءالسيارة
            car.Parts = await genericRepositoryPart.AddRangeAsync(parts);   

            // حلقة من اجل المرور على كل جزء والتحقق من تواجد المورد الذي ورّده
            foreach (var item in parts)
            {
                // جلب الموّرد للتأكد من تواجده في قاعدة البيانات
                var supplier = await genericRepositorySupplier.GetItemByIdForUpdateOrDeleteAsync(item.SupplierId);

                // الشرط الثاني للتأكد أن المورد ليس  محذوف  
                if (supplier == null || supplier.IsDeleted==true)
                    return NotFound();

                // قمت باستدعاء دالة الحفظ هنا وليس في الريبوزيتوري لأنني احتجت لفعل بعض الخطوات قبل الحفظ
                // و هذه الخطوات هي اضافة قائمة اجزاء لهذه السيارة والتحقق من وجود المورد الذي يورّد كل جزء
            }
                await repositoryTest.save();

            return CreatedAtRoute("GetCar", new { carId = car.Id }, car);
        }








        [HttpPut("{carId}")]
        public async Task<ActionResult> UpdateCar(int carId, CarForUpdate carForUpdate) // ايند بوينت تعديل سيارة
        {
            // جلب السيارة المُراد تعديلها للتأكد من تواجدها في قاعدة البيانات
            var car = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(carId); 

            // الشرط الثاني للتأكد أن السيارة ليست محذوفة  
            if (car == null || car.IsDeleted==true)
                return NotFound();

            //  استدعاء دالة التعديل في حال كانت السيارة موجودة في قاعدة البيانات وليست محذوفة 
            await repositoryTest.UpdateItemAsync(carId, carForUpdate);

            return NoContent();
        }








        [HttpPatch("{carId}")]
        public async Task<ActionResult> UpdatePartiallyCar(int carId,
            JsonPatchDocument<CarForUpdate> patchDocument) // ايند بوينت تعديل بشكل جزئي
        {
            // جلب السيارة المُراد تعديلها للتأكد من تواجدها في قاعدة البيانات
            var car = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(carId); 

            // الشرط الثاني للتأكد أن السيارة ليست محذوفة
            if (car == null || car.IsDeleted==true)
                return NotFound();

            // استدعاء دالة التعديل الجزئي في حال كانت السيارة موجودة في قاعدة البيانات وليست محذوفة
            await repositoryTest.UpdatePartiallyItemAsync(carId, patchDocument);

            return NoContent();
        }

    }
}
