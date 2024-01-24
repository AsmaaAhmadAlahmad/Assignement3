
using Assignement3_Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Assignement3_Domain.Handlers;

public class CarHandler
{
   
    public void GetCars()
    {
        using (Assingment3DbContext context = new())
        {

            // جلب السيارات الغير محذوفة مع قائمة القطع لكل سيارة
            var cars= context.Cars.Where(c => c.IsDeleted == false).Include(c=>c.Parts)
                .ToList();
            if (cars.Any())
            {
                foreach (var item in cars)
                {   
                    // طباعة موديل السيارة 
                    Console.WriteLine($"The car model is {item.Model}");
                    foreach (var item1 in item.Parts)
                    {
                        // طباعة قائمة القطع التي تحتويها هذه السيارة
                        Console.WriteLine($"   The car contains this part {item1.Name}");
                    }
                }
            }
            else
                Console.WriteLine("The cars table doesn't has any data!");
       }
    }



    // تابع اضافة سيارة
    public void CarAdd(Car car)
    {
        using (Assingment3DbContext context = new())

        {   // اضافة سيارة
          if (context == null)
           {
               Console.WriteLine("Error: Assingment3DbContext is null");
           }
          context.Cars.Add(car);
          context.SaveChanges();
        }
    }
    


    // تابع حذف سيارة
    public void CarDelete(String model)
    {
        using (Assingment3DbContext context = new())
        {
            // جلب اول سيارة تملك الموديل الذي ادخله المستخدم و غير محذوفة مسبقا كحذف ناعم 
          var  car = context.Cars.Where(p => p.IsDeleted != true)
                .FirstOrDefault(c => c.Model == model);
            
            if (car != null)// للتأكد من تواجد الموديل  الذي سيدخله المستخدم في قاعدة البيانات  
            {
                car.IsDeleted = true;
                //  استعلام حتى يتم جلب مبيعات هذا الزبون ثم حذفها ايضا كحذف ناعم  
                context.SaveChanges();

                var Car_sales = context.Sales.Where(s => s.CarId == car.Id).ToList();
                foreach (var item in Car_sales)
                {
                    item.IsDeleted = true;
                    
                }
               
                context.SaveChanges();
            }
            else 
                Console.WriteLine($"There is no car with this model {model}!");
            
       }
    }



    // تابع تعديل معلومات سيارة
    // حيث يقوم المستخدم بإدخال اسم الخاصية التي يريد تعديلها ثم يدخل القيمة القديمة للخاصية ثم الجديدة
    // لانه يحوي كل الانواع object لذلك استخدمت النوع   
    public void CarUpdate(object propertyName, object oldValue , object newValue)
    {

        using (Assingment3DbContext context =new())
        {   
            string propertyNameAsString = (string)propertyName;
            string  oldValueAsString = "";
            string newValueAsString  =  "";

            // اختبار  اذا كانت الخاصية هي موديل السيارة
            if (propertyNameAsString == "model" || propertyNameAsString == "Model")
            {
                try
                {   // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية موديل السيارة نوعها سترينغ
                    oldValueAsString = (string)oldValue;
                    newValueAsString = (string)newValue;
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("In the car update function," +
                        "and to update the model of a car part ,you must enter string values for " +
                     "the old value and the new value");
                }
                // جلب السيارة المراد تعديلها والتأكد بأنها موجودة 
                var car = context.Cars.Where(c=>c.IsDeleted!=true).FirstOrDefault(c => c.Model == oldValueAsString);
                if (car != null)
                {
                    car.Model = newValueAsString;
                    context.SaveChanges();
                }
                else // التي أدخلها المستخدم غير متواجدة  model في حال كانت قيمة   
                    Console.WriteLine($"There is no car from this model{oldValue}!");
                
            }

            // Gear اختبار اذا كانت الخاصية هي  
            else if (propertyNameAsString == "Gear" || (string)propertyName == "Gear")
            {

                try
                {  // نوعها سترينغ Gear تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية 
                    oldValueAsString = (string)oldValue;
                    newValueAsString = (string)newValue;
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("In the car update function," +
                        "and to update the Gera of a car  ,you must enter string values for " +
                     "the old value and the new value");
                }
                // جلب السيارة المراد تعديلها والتأكد بأنها موجودة
                var car = context.Cars.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Gear == oldValueAsString);
                if (car != null)
                {
                    //  Gear تعديل الخاصية 
                    car.Gear = newValueAsString;
                    context.SaveChanges();

                }
                else // التي أدخلها المستخدم غير موجودة Gear في حال كانت قيمة 
                    Console.WriteLine("The specified gear value is not valid!");
                   
            }

            //  year اختبار اذا كانت الخاصية هي 
            else if (propertyNameAsString == "year" || propertyNameAsString == "Year")
            {
                int oldValueAsInt = 0;
                int newValueAsInt = 0;
                try
                {  // int نوعها year لان خاصية int تحويل القيمتين القديمة والجديدة الى النوع    
                    oldValueAsInt = (int)oldValue;
                    newValueAsInt = (int)newValue;
                }
                catch (Exception)
                {
                    Console.WriteLine("In the car update function,and to update year of a car, you must enter integer values for " +
                     "the old value and the new value");
                }
                // جلب السيارة المراد تعديلها والتأكد بأنها موجودة 
                var car = context.Cars.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Year == oldValueAsInt);
                if (car != null)
                {
                    car.Year = newValueAsInt;
                    context.SaveChanges();
                }
                else // التي أدخلها الزيون غير موجودة year  في حال كانت قيمة 
                    Console.WriteLine("The specified year value is not valid!");

            }

            //  km اختبار اذا كانت الخاصية هي 
            else if (propertyNameAsString == "Km" || propertyNameAsString == "km")
            {
                double oldValueAsDouble = 0.0;
                double newValueAsDouble = 0.0;
                try
                {
                    // double نوعها Km لان خاصية double تحويل القيمتين القديمة والجديدة الى النوع 
                    oldValueAsDouble = (double)oldValue;
                    newValueAsDouble = (double)newValue;
                }
                catch (Exception)
                {
                    Console.WriteLine("In the car update function," +
                        "and to update the km of a car  ,you must enter double values for " +
                     "the old value and the new value");
                }

                // جلب السيارة المراد تعديلها والتأكد بأنها موجودة 
                var car = context.Cars.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Km == oldValueAsDouble);
                if (car != null)
                {
                    car.Km = newValueAsDouble;
                    context.SaveChanges();
                }
                else // التي أدخلها الزيون غير موجودة km  في حال كانت قيمة 
                    Console.WriteLine("The km value is not valid!");
                 
            }

            // في حال ادخل المستخدم اسم خاصية ليست موجودة في خصائص السيارة 
            else
                Console.WriteLine("The property you entered does not exist in the car class, enter an existing property !");
             
            
            
        }
    }
}
