using Assignement3_Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignement3_Domain.Handlers
{
    public class PartHandler
    {
        public void GetParts()
        {
            using (Assingment3DbContext context = new())

            {
                // جلب القطع الغير محذوفة مع قائمة السيارات التي تتواجد فيها كل قطعة
                var parts = context.Parts.Where(c => c.IsDeleted == false).Include(x => x.Cars).ToList();
                if (parts.Any())
                {
                    foreach (var item in parts)
                    {   // طباعة اسم كل قطعة 
                        Console.WriteLine($"This part is {item.Name}");
                        foreach (var item1 in item.Cars)
                        {
                            // طباعة موديل كل سيارة متواجدة فيها هذه القطعة 
                            Console.WriteLine($" The part belongs to this car {item1.Model}");
                        }
                    }

                }
                else
                    Console.WriteLine("The parts table doesn't has any data!");
            }
        }


        // دالة اضافة قطعة
        public void AddParts(Part part)
        {
            using (Assingment3DbContext context = new())

            {
                context.Parts.Add(part);
                context.SaveChanges();
            }
        }


        // دالة حذف قطعة 
        public void RemoveParts(String name)
        {
            using (Assingment3DbContext context = new())
            {
                // جلب اول قطعة تملك الاسم الذي ادخله المستخدم و غير محذوف مسبقا كحذف ناعم 
                var part = context.Parts.Where(p => p.IsDeleted != true).FirstOrDefault(p => p.Name == name);
                if (part != null)
                {
                    part.IsDeleted = true;
                    context.SaveChanges();
                }
                else
                    Console.WriteLine($"This is no part with this name {name}!");
            }
        }


        // دالة تعديل قطعة
        // حيث يقوم المستخدم بإدخال اسم الخاصية التي يريد تعديلها ثم يدخل القيمة القديمة للخاصية ثم الجديدة
        // لانه يحوي كل الانواع object لذلك استخدمت النوع   
        public void UpdateParts(object NameProperty, object oldValue, object newValue)
        {
            using (Assingment3DbContext context = new())
            {
                string NamePropertyAsString = (string)NameProperty;
                string oldValueAsString = "";
                string newValueAsString = "";
                // في حال كانت الخاصية التي ادخلها المستخدم هي اسم القطعة
                if (NamePropertyAsString == "Name" || NamePropertyAsString == "name")
                {
                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية اسم القطعة نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the part update function," +
                            "and to update the name of a car part ,you must enter string values for " +
                         "the old value and the new value");
                    }
                    // جلب اول قطعة تملك الاسم الذي ادخله المستخدم ليتم تعديله
                    var part = context.Parts.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Name == oldValueAsString);
                    if (part != null)
                    {
                        // تعديل اسم القطعة
                        part.Name = newValueAsString;
                        context.SaveChanges();
                    }
                    else // في حال كان الاسم الذي ادخله المستخدم غير موجود
                        Console.WriteLine($"This is no part with this name{oldValueAsString}!");

                }

                // في حال كانت الخاصية التي ادخلها المستخدم هي سعر القطعة
                else if (NamePropertyAsString == "Price" || NamePropertyAsString == "price")
                {

                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية سعر القطعة نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the part update function,and to update the price" +
                            "of a car part,you must enter string values for " +
                         "the old value and the new value");
                    }
                    // جلب اول قطعة تملك السعر الذي ادخله المستخدم ليتم تعديله
                    var part = context.Parts.Where(c => c.IsDeleted != true).FirstOrDefault(p => p.Price == oldValueAsString);
                    if (part != null)
                    {
                        // تعديل سعر القطعة
                        part.Price = newValueAsString;
                        context.SaveChanges();

                    }
                    else // في حال كان السعر الذي ادخله المستخدم غير موجود
                        Console.WriteLine($"This is no part with this price {oldValueAsString}!");

                }

                // Quantity في حال كانت الخاصية التي ادخلها المستخدم هي 
                else if (NamePropertyAsString == "Quantity" || NamePropertyAsString == "quantity")
                {
                    int oldValueAsInt = 0;
                    int newValueAsInt = 0;
                    try
                    {
                        // int  تحويل القيمتين القديمة والجديدة الى النوع
                        //int  نوعها Quantity لان خاصية
                        oldValueAsInt = (int)oldValue;
                        newValueAsInt = (int)newValue;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("In the part update function,and to update the quantity" +
                            "of a car part,you must enter integer values for " +
                         "the old value and the new value");
                    }
                    // الذي ادخله المستخدم ليتم تعديله Quantity جلب اول قطعة تملك
                    var part = context.Parts.Where(c => c.IsDeleted != true).FirstOrDefault(p => p.Quantity == oldValueAsInt);
                    if (part != null)
                    {
                        part.Quantity = newValueAsInt;
                        context.SaveChanges();
                    }
                    else //Quantity في حال كان 
                         // الذي ادخله المستخدم غير موجود
                        Console.WriteLine($"This is no parts with this Quantity{oldValueAsInt}!");

                }

                // في حال ادخل المستخدم اسم خاصية ليست موجودة في خصائص القطع 
                else
                    Console.WriteLine("The property you entered does not exist in the car class, enter an existing property !");

            }
        }
    }
}
