using Assignement3_Domain.ApiModels;
using Assignement3_Domain.Services;
using Assignement3_Domain;
using Assingment3Api.ApiModels;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignement3_Domain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User", Policy = "ShouldBeUser")]



    public class CustomersController : ControllerBase
    {
        private readonly IGenericRepository<Customer, CustomerForCreate, CustomerForUpdate> repositoryTest;

        public CustomersController(IGenericRepository<Customer, CustomerForCreate, CustomerForUpdate> repositoryTest)
        {
            this.repositoryTest = repositoryTest;
        }




        [HttpGet("{customerId}", Name = "GetCustomer")]
        public async Task<ActionResult> GetCustomer(int customerId) // ايند بوينت لجلب زبون بواسطة الاي دي 
        {
            var customer = await repositoryTest.GetItemByIdAsync(
                filterIdAndIsDeleted: c => c.Id == customerId && c.IsDeleted == false, // الفلترة لجلب الزبون حسب الآي دي و
                                                                                       // وأن يكون غبر محذوف
                includeProperties: "Sales"); // تضمين قائمة المبيعات للزبون

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }







        [HttpGet]
        public async Task<ActionResult> GetCustomers() // الايند بوينت التي تجلب كل الزبائن
        {
            var customers = await repositoryTest.GetItemsAsync(filter: c => c.IsDeleted == false, // الفلترة لجلب الزبائن الغير محذوفين فقط
                includeProperties: "Sales"); // تضمين قوائم المبيعات للزبائن

            if (!customers.Any())
                return NotFound();

            return Ok(customers);
        }






        

        [HttpDelete("{customerId}")]
        public async Task<ActionResult> DeleteCustomer(int customerId) // ايند بوينت لحذف الزبون 
        {
            // جلب الزبون المُراد حذفه للتأكد من تواجده في قاعدة البيانات
            var customer = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(customerId);

            // الشرط الثاني للتأكد من ان الزبون غير محذوف
            if (customer == null || customer.IsDeleted == true)
                return NotFound();

            // استدعاء دالة الحذف في حال كان الزبون غير محذوف ومتواجد في القاعدة 
            await repositoryTest.DeleteItemAsync(customerId);

            return NoContent();
        }






        // في الايند بوينت التالية حقنت الجينريك ريبوزيتوري الخاصة بالمبيعات حتى استدعي الدالة   
        // التي ستقوم بإضافة قائمة المبيعات الخاصة بهذا الزبون الذي سيتم اضافته 

        // و حقنت الجينريك ريبوزيتوري الخاصة بالسيارات حتى استدعي الدالة     
        // التي ستقوم بالتأكد من تواجد السيارة التي تم بيعها في عملية البيع هذه  
        [HttpPost]
        public async Task<ActionResult> AddCustomer([FromServices] IGenericRepository<Sales, SalesForCreateListOfSalesForCustomer, SalesForUpdate> genericRepositorySales,
           IGenericRepository<Car, CarForCreate, CarForUpdate> genericRepositoryCar,
           [FromQuery] CustomerForCreate customerForCreate, // حيث body لأنه لايمكن تمرير بارامترين من [FromQuery] هذا البارمتر يتم تمريره بطريقة
            List<SalesForCreateListOfSalesForCustomer> sales) // هذا البارامتر سيتم تمريره من البادي
        {
            // اضافة الزبون
            var customer = await repositoryTest.AddItemAsync(customerForCreate);

            // اضافة قائمة مبيعات خاصة بهذا الزبون
            customer.Sales = await genericRepositorySales.AddRangeAsync(sales);

            // حلقة من اجل المرور على كل عملية بيع والتحقق من تواجد السيارة التي تم بيعها في هذه العملية 
            foreach (var item in sales)
            {
                var car = await genericRepositoryCar.GetItemByIdForUpdateOrDeleteAsync(item.CarId);

                // الشرط الثاني للتأكد من ان السيارة غير محذوفة
                if (car == null || car.IsDeleted == true)
                    return NotFound();
            }

            try
            {
                // قمت باستدعاء دالة الحفظ هنا وليس في الريبوزيتوري لأنني احتجت لفعل خطوة قبل الحفظ
                // و هذه الخطوة هي اضافة قائمة مبيعات لهذا الزبون 
                await repositoryTest.save();

                return CreatedAtRoute("GetCustomer", new { customerId = customer.Id }, customer);
            }

            // هنا يتم معالجة الخطأ الذي  يظهر عند وضع رقم سيارة في عملية البيع للزبون الذي تتم إضافته وهذا الرقم موجود مسبقا
            // في جدول المبيعات (يعني أن السيارة صاحبة هذا الرقم مٌباعة) لأن السيارة لايتم بيعها إلا مرة واحدة 
            // لأن العلاقة بين جدول السيارات وجدول المبيعات هي وان تو وان
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "This car has been previously sold!");
            }

        }






        [HttpPut("{customerId}")]
        public async Task<ActionResult> UpdateCustomer(int customerId, CustomerForUpdate customerForUpdate) // ايند بوينت تعديل السيارة
        {
            // جلب الزبون المُراد تعديله للتأكد من تواجده في قاعدة البيانات
            var customer = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(customerId);

            // الشرط الثاني للتأكد من ان الزبون غير محذوف
            if (customer == null || customer.IsDeleted == true)
                return NotFound();

            // استدعاء دالة التعديل في حال  كان الزبون غير محذوف ومتواجد في القاعدة
            await repositoryTest.UpdateItemAsync(customerId, customerForUpdate);

            return NoContent();
        }






        [HttpPatch("{customerId}")]
        public async Task<ActionResult> UpdatePartiallyCustomer(int customerId, 
            JsonPatchDocument<CustomerForUpdate> patchDocument) // ايند بوينت التعديل الجزئي للزبون
        {
            // جلب الزبون المُراد تعديله للتأكد من تواجده في قاعدة البيانات
            var customer = await repositoryTest.GetItemByIdForUpdateOrDeleteAsync(customerId);

            // الشرط الثاني للتأكد من ان الزبون غير محذوف
            if (customer == null || customer.IsDeleted==true)
                return NotFound();

            // استدعاء دالة التعديل الجزئي  في حال  كان الزبون غير محذوف ومتواجد في القاعدة
            await repositoryTest.UpdatePartiallyItemAsync(customerId, patchDocument);

            return NoContent();
        }
    }
}
