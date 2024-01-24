using Assignement3_Domain.Services;
using Assignement3_Data;
using Assignement3_Domain;
using Assignement3_Domain.Helper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using AutoMapper;
using Assignement3_Domain.ApiModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Assignement3_Domain.Services
{
    // من أجل الكلاس الأصلي T
    //  ApiModels  من أجل الكلاس الذي يُستخدم للإضافة والمتواجد في مجلد B
    //  ApiModels من أجل الكلاس الذي يُستخدم للتعديل والمتواجد في مجلد C
    public class GenericRepository<T, B, C> : IGenericRepository<T, B, C> where T : class where B : class where C : class
    {
        private readonly Assingment3DbContext context1;
        private readonly HelperMapper<T, B, C> mapper;
        private readonly DbSet<T> _Set;




        // Assingment2Context1 في المشيد التالي تم حقن الكلاس
        // AutoMapper وتم حقن الكلاس الذي يحوي اعدادت من اجل استخدام
        public GenericRepository(Assingment3DbContext context1, HelperMapper<T, B, C> mapper)
        {

            this.context1 = context1;

            this.mapper = mapper;

            _Set = context1.Set<T>();
        }
       



        // دالة جلب كائن
        public async Task<T> GetItemByIdAsync(
        Expression<Func<T, bool>> filterIdAndIsDeleted = null // هذا البارامتر من أجل وضع فلتر ما على البيانات وقد استخدمته في التطبيق للفلترة حتى يتم جلب كائن برقم آي دي معين ولجلب الكائنات الغير محذوفة فقط 
        , string includeProperties = "") // هذا البارامتر من أجل تضمين خصائص القوائم للكلاسات
        {
            IQueryable<T> query =_Set;
            
            if (includeProperties != "")
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filterIdAndIsDeleted != null)
            {
                query=query.Where(filterIdAndIsDeleted);
            }

            return await query.FirstOrDefaultAsync();

        }



        // دالة جلب كل الكائنات
        // تمت الاستفادة من الرابط التالي من اجل وضع فلترة لجلب فقط الكائنات الغير
        //  محذوفة وإمكانية التضمين لجلب قائمة ما خاصة بهذا الكائن  في الريبوزيتوري العام 
        // https://stackoverflow.com/questions/48671263/generic-repository-includes-and-filtering
        public async Task<IEnumerable<T>> GetItemsAsync(
         Expression<Func<T, bool>> filter = null // هذا البارامتر من أجل وضع فلتر ما على البيانات وقد استخدمته في التطبيق للفلترة لجلب الكائنات الغير محذوفة فقط 
        , string includeProperties = "") // هذا البارامتر من أجل تضمين خصائص القوائم للكلاسات
        {

            IQueryable<T> query = _Set;
            if (includeProperties != "")
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();

        }





        // دالة حذف كائن كحذف ناعم
        public async Task DeleteItemAsync(int id)
        {
            var item = await GetItemByIdForUpdateOrDeleteAsync(id);

            if (item != null)
            {
                if (item is Car car)
                {
                    car.IsDeleted = true;

                   // استعلام من أجل حذف مبيعات هذه السيارة كحذف ناعم
                   var car_sales = await context1.Sales.Where(s => s.CarId == car.Id).ToListAsync();

                    foreach (var item1 in car_sales)
                    {
                        item1.IsDeleted = true;
                    }
                }

                else if (item is Supplier supplier)
                {
                    supplier.IsDeleted = true;

                   // استعلام من أجل حذف الاجزاء التي وردها هذا المورد كحذف ناعم
                   var part_supplier = await context1.Parts.Where(s => s.SupplierId == supplier.Id).ToListAsync();

                    foreach (var item1 in part_supplier)
                    {
                        item1.IsDeleted = true;
                    }
                }

                else if (item is Customer customer)
                {
                    customer.IsDeleted = true;

                    // استعلام من أجل حذف مبيعات هذا الزبون كحذف ناعم
                   var customer_sales = await context1.Sales.Where(s => s.CustomerId == customer.Id).ToListAsync();

                    foreach (var item1 in customer_sales)
                    {
                        item1.IsDeleted = true;
                    }

                }

                else if (item is Sales sales)
                    sales.IsDeleted = true;

                else if (item is Part part)
                    part.IsDeleted = true;

            }

            await context1.SaveChangesAsync();
        }


        


        // دالة جلب كائن للتأكد من وجوده فقط و يتم استدعائها في دوال الحذف والتعديل
        // حتى يتم التأكد من تواجد هذا الكائن قبل حذفه أو تعديله
        public async Task<T> GetItemByIdForUpdateOrDeleteAsync(int id)
        {
            var item =await _Set.FindAsync(id);

            return item;
        }





        // دالة اضافة كائن
        public async Task<T> AddItemAsync(B item, int supplierId = 0, int customerId = 0, int carId = 0)
        {
            var itemToAdd = mapper.MapperForCreate.Map<B, T>(item);

            _Set.Add(itemToAdd);

           // قمت بتعليق السطر السفلي لأنه في دالة الاضافة في جميع المتحكمات 
           // هناك اجراءات يجب القيام بها قبل حفظ التغييرات في القاعدة
          // await context1.SaveChangesAsync();

            return itemToAdd;
        }





       // دالة تعديل كائن
        public async Task UpdateItemAsync(int id, C item)
        {
            // جلب الكائن المراد تعديله
            var itemThatWillUpdate = await GetItemByIdForUpdateOrDeleteAsync(id);

            // T إلى النوع C عمل ماب لهذا من النوع 
            mapper.MapperForUpdate.Map(item, itemThatWillUpdate);

            await context1.SaveChangesAsync();
        }





       // دالة تعديل جزئي
        public async Task UpdatePartiallyItemAsync(int id, JsonPatchDocument<C> patchDocument)
        {
            // جلب الكائن المراد تعديله جزئيا
            var itemThatWillUpdate = await GetItemByIdForUpdateOrDeleteAsync(id);

            // لأننا في التعديل الجزئي نحتاج أولا لعمل ماب من النوع الأصلي  C إلى النوع T عمل ماب لهذا من النوع 
           // إلى النوع الذي يُستخدم للتعديل حتى نطبق التعديلات عليه والخاصية التي لم نعدلها ستحافظ على قيمتها 
            var itemForUpdate = mapper.MapperForPartiallyUpdate.Map<T, C>(itemThatWillUpdate);

            // C تطبيق التعديلات على الكائن الذي من النوع 
            patchDocument.ApplyTo(itemForUpdate);

            // T إلى النوع C عمل ماب لهذا من النوع 
            mapper.MapperForUpdate.Map(itemForUpdate, itemThatWillUpdate);

            await context1.SaveChangesAsync();
        }




        // SaveChangesAsync هذه الدالة أنشأتها بهذا الشكل لأننا احتجت الى استدعاء الدالة
        // الإضافة في جميع المتحكمات Actions في 
        public async Task save()
        {
            await context1.SaveChangesAsync();
        }





        // هذه الدالة من أجل اضافة قائمة كائنات تابعة لكائن ما  
        public async Task<List<T>> AddRangeAsync(List<B> bs) 
        {
            // T إنشاء قائمة من النوع
            List<T> itemsThatWillAdd = new List<T>();

            // T إلى القائمة التي نوعها B عمل ماب من القائمة التي نوعها 
            itemsThatWillAdd = mapper.MapperForCreate.Map<List<T>>(bs);

            // اضافة قائمة العناصر للقاعدة
            await _Set.AddRangeAsync(itemsThatWillAdd);

            return itemsThatWillAdd;

        }

    }
}
