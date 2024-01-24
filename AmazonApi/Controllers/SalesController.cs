using Assignement3_Domain.ApiModels;
using Assignement3_Domain.Services;
using Assignement3_Domain;
using Assingment3Api.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assingment3Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User", Policy = "ShouldBeUser")]

    public class SalesController : ControllerBase
    {
        private readonly IGenericRepository<Sales, SalesForCreate, SalesForUpdate> genericRepository;

        public SalesController(IGenericRepository<Sales, SalesForCreate, SalesForUpdate> genericRepository)
        {
            this.genericRepository = genericRepository;
        }


        [HttpGet("{salesId}", Name = "GetSales")]
        public async Task<ActionResult> GetSales(int salesId) // ايند بوينت لجلب عملية بيع واحدة فقط بحسب رقم الآي دي
        {
            // استدعاء دالة جلب كائن واحد
            var Sales = await genericRepository.GetItemByIdAsync(
                filterIdAndIsDeleted: s => s.Id == salesId && s.IsDeleted != true); // فلترة لجلب عملية البيع بحسب رقم الآي دي
                                                                                    // وأن تكون غبر محذوفة

            if (Sales == null)
                return NotFound();

            return Ok(Sales);
        }





        [HttpGet]
        public async Task<ActionResult> GetAllSales() // ايند بوينت لجلب كل المبيعات
        {
            // استدعاء دالة جلب كل الكائنات 
            var AllSales = await genericRepository.GetItemsAsync(filter:s=>s.IsDeleted==false); // الفلترة لجلب المبيعات الغير محذوفة فقط

            if (!AllSales.Any())
                return NotFound();

            return Ok(AllSales);
        }




       

        [HttpDelete("{salesId}")]
        public async Task<ActionResult> DeleteSales(int salesId) // ايند بوينت لحذف عملية بيع
        {
            // جلب عملية البيع للتأكد من تواجدها في قاعدة البيانات
            var sales = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(salesId);

            // الشرط الثاني للتأكد بأن عملية البيع هذه غير محذوفة
            if (sales == null  || sales.IsDeleted==true)
                return NotFound();

            // استدعاء دالة الحذف في حال كانت السيارة متواجدة في القاعدة وغير محذوفة
            await genericRepository.DeleteItemAsync(salesId);

            return NoContent();
        }





        [HttpPost]
        // تم حقن الجينريك ريبوزيتوري الخاصة بالزبائن هنا من اجل استدعاء الدالة التي ستقوم بالتأكد
        // من وجود الزبون الذي تمت عملية البيع هذه له
        //

        // تم حقن الجينريك ريبوزيتوري الخاصة بالزبائن هنا من اجل استدعاء الدالة التي ستقوم بالتأكد
        //  من وجود السيارة التي تم بيعها في عملية البيع هذه
        public async Task<ActionResult> AddSales([FromServices] IGenericRepository<Customer, CustomerForCreate, CustomerForUpdate> genericRepositoryCustomer,
           [FromServices] IGenericRepository<Car, CarForCreate, CarForUpdate> genericRepositoryCar
            , SalesForCreate salesForCreate)
        {
            // اضافة عملية البيع
            var sales = await genericRepository.AddItemAsync(salesForCreate);

           // جلب الزبون الذي ستتم عملية البيع هذه له للتأكد من تواجده في قاعدة البيانات
            var customer = await genericRepositoryCustomer.GetItemByIdForUpdateOrDeleteAsync(sales.CustomerId);

            // الشرط الثاني للتأكد بأن الزبون غير محذوف
            if (customer == null || customer.IsDeleted == true)
                return NotFound();

            // جلب السيارة التي سيتم بيعها في عملية البيع هذه للتأكد من تواجدها في قاعدة البيانات
            var car = await genericRepositoryCar.GetItemByIdForUpdateOrDeleteAsync(sales.CarId);

            // الشرط الثاني للتأكد بأن الزبون غير محذوف
            if (car == null || car.IsDeleted == true)
                return NotFound();

            try
            {
                // استدعيت دالة الحفظ هنا وليس في الريبوزيتوري بسبب تواجد بعض الخطوات التي يجب القيام بها قبل الحفظ
                // وهذه الخطوات هي التأكد من تواجد الزبون الذي تتم عملية البيع المُضافة له في القاعدة والتأكد من تواجد
                // السيارة التي يتم بيعها
                await genericRepository.save();

                return CreatedAtRoute("GetSales", new { salesId = sales.Id }, sales);
            }

            // هنا يتم معالجة الخطأ الذي  يظهر عند وضع رقم سيارة في عملية البيع المُضافة وهذا الرقم موجود مسبقا
            // في جدول المبيعات (يعني أن السيارة صاحبة هذا الرقم مٌباعة) لأن السيارة لايتم بيعها إلا مرة واحدة 
            // لأن العلاقة بين جدول السيارات وجدول المبيعات هي وان تو وان
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "This car has been previously sold!");
            }

        }




        [HttpPut("{salesId}")]
        public async Task<ActionResult> UpdateSales(int salesId, SalesForUpdate salesForUpdate) // ايند بوينت تعديل عملية بيع
        {
            // جلب عملية البيع للتأكد من تواجدها في القاعدة
            var sales = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(salesId);

            // الشرط الثاني للتأكد بأن عملية البيع هذه غير محذوفة
            if (sales == null || sales.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل في حال تواجدت السيارة في القاعدة ولم تكن محذوفة
            await genericRepository.UpdateItemAsync(salesId, salesForUpdate);

            return NoContent();
        }




        [HttpPatch("{salesId}")]
        public async Task<ActionResult> PartialllyUpdateSales(int salesId 
             , JsonPatchDocument<SalesForUpdate> patchDocument) // ايند بوينت تعديل عملية البيع بشكل جزئي
        {
            // جلب عملية البيع للتأكد من تواجدها في القاعدة
            var sales = await genericRepository.GetItemByIdForUpdateOrDeleteAsync(salesId);
            
            // الشرط الثاني للتأكد بأن عملية البيع هذه غير محذوفة
            if (sales == null || sales.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل الجزئي في حال تواجدت السيارة في القاعدة ولم تكن محذوفة
            await genericRepository.UpdatePartiallyItemAsync(salesId, patchDocument);

            return NoContent();
        }



    }
}
