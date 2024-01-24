using Assignement3_Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignement3_Domain.Handlers
{
    public class CustomerHandler
    {

        public void GetCustomer()
        {
            using (Assingment3DbContext context = new())
            {
                // جلب الزبائن الغير محذوفين مع قائمة مبيعات كل زبون
                var Customers = context.Customers.Where(c => c.IsDeleted == false).Include(c => c.Sales).ToList();
                if (Customers.Any())
                {
                    foreach (var item in Customers)
                    {
                        Console.WriteLine("The customer name is " + item.Name);
                        foreach (var item1 in item.Sales)
                        {
                            // طباعة اجمالي كل عملية بيع لهذا الزبون 
                            Console.WriteLine(" Total sales " + item1.Total);
                        }
                    }
                }
                else
                    Console.WriteLine("The table customer doesn't has any data!");
            }
        }



        // دالة اضافة زبون 
        public void AddCustomer(Customer customer)
        {
            using (Assingment3DbContext context = new())
            {
                try
                {
                    context.Customers.Add(customer);
                    context.SaveChanges();
                }

                catch (DbUpdateException)
                {
                    // في حال تمت اضافة زبون مع وضع رقم سيارة تم بيعها في قائمة مبيعاته لان السيارة يتم بيعها 
                    // مرة واحدة فقط
                    Console.WriteLine("This car has been previously sold!");
                }

            }
        }


        // دالة حذف سيارة
        public void DeleteCustomer(String name)
        {
            using (Assingment3DbContext context = new())
            {
                // جلب اول زبون يملك الاسم الذي ادخله المستخدم و غير محذوف مسبقا كحذف ناعم 
                var customer = context.Customers.Where(p => p.IsDeleted != true).FirstOrDefault(c => c.Name == name);
                if (customer != null)
                {
                    customer.IsDeleted = true;
                    // استعلام حتى يتم جلب مبيعات هذا الزبون ثم حذفها ايضا 
                    var Customer_sales = context.Sales.Where(c => c.CustomerId == customer.Id).ToList();
                    // التأكد من ان هذا الزبون يحوي مبيعات
                    if (Customer_sales.Any())
                    {
                        foreach (var item in Customer_sales)
                        {
                            // حذف مبيعات هذا الزبون
                            item.IsDeleted = true;
                        }
                    }
                    context.SaveChanges();
                }
                else
                    Console.WriteLine("This name doesn't exist to delete!");
            }
        }


        // دالة تعديل زبون 
        // حيث يقوم المستخدم بإدخال اسم الخاصية التي يريد تعديلها ثم يدخل القيمة القديمة للخاصية ثم الجديدة
        // لانه يحوي كل الانواع object لذلك استخدمت النوع   
        public void UpdateCustomer(object NameProperty, object oldValue, object newValue)
        {
            using (Assingment3DbContext context = new())
            {
                string NamePropertyAsString = (string)NameProperty;

                // في حال كانت الخاصية التي ادخلها المستخدم هي اسم الزبون
                if (NamePropertyAsString == "Name" || NamePropertyAsString == "name")
                {
                    string oldValueAsString = "";
                    string newValueAsString = "";
                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية اسم الزبون نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the customer update function,and to update name of a customer you must enter string values for " +
                         "the old value and the new value");
                    }

                    // جلب اول زبون يملك الاسم الذي ادخله المستخدم ليتم تعديله
                    var customer = context.Customers.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Name == oldValueAsString);
                    if (customer != null)
                    {
                        // تعديل اسم الزبون
                        customer.Name = newValueAsString;
                        context.SaveChanges();
                    }
                    else // في حال كان الاسم الذي ادخله المستخدم غير موجود
                        Console.WriteLine("This name doesn't exist to updata!");
                }

                // في حال كان اسم الخاصية التي ادخلها المستخدم هي عنوان الزبون
                else if (NamePropertyAsString == "Address" || NamePropertyAsString == "address")
                {
                    string oldValueAsString = "";
                    string newValueAsString = "";
                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية عنوان الزبون نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the customer update function,and to update address of a customer you must enter string values for " +
                         "the old value and the new value");
                    }
                    // جلب اول زبون يملك العنوان الذي ادخله المستخدم ليتم تعديله
                    var customer = context.Customers.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Address == oldValueAsString);
                    if (customer != null)
                    {
                        // تعديل عنوان الزبون
                        customer.Address = newValueAsString;
                        context.SaveChanges();

                    }
                    else // في حال كان العنوان الذي ادخله المستخدم غير موجود
                        Console.WriteLine("This address doesn't exist!");
                }

                // في حال كانت الخاصية التي ادخلها المستخدم هي عمر الزبون
                else if (NamePropertyAsString == "Age" || NamePropertyAsString == "age")
                {

                    int oldValueAsInt = 0;
                    int newValueAsInt = 0;
                    try
                    {
                        // int لان خاصية عنوان الزبون نوعها int تحويل القيمتين القديمة والجديدة الى النوع  
                        oldValueAsInt = (int)oldValue;
                        newValueAsInt = (int)newValue;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("In the age update function,and to update the age of a customer you must enter integer values for " +
                         "the old value and the new value");
                    }
                    // جلب اول زبون يملك العمر الذي ادخله المستخدم ليتم تعديله
                    var customer = context.Customers.Where(c => c.IsDeleted != true).FirstOrDefault(c => c.Age == oldValueAsInt);
                    if (customer != null)
                    {
                        // تعديل عنوان الزبون
                        customer.Age = newValueAsInt;
                        context.SaveChanges();
                    }
                    else // في حال كان العمر الذي ادخله المستخدم غير موجود
                        Console.WriteLine("This age doesn't exist");
                }

                else // في حال كان اسم الخاصية التي ادخلها المستخدم ليست من خصائص الزبون
                    Console.WriteLine("The property you entered does not exist in the car class, enter an existing property !");

            }
        }
    }
}
