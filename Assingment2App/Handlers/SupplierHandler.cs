using Assignement3_Data;
namespace Assignement3_Domain.Handlers
{
    public class SupplierHandler
    {
        public void GetSupplier()
        {
            using (Assingment3DbContext context = new())
            {
                // جلب الموردين الغير محذوفين 
                var suppliers = context.Suppliers.Where(c => c.IsDeleted == false).ToList();

                if (suppliers.Any())
                {
                    foreach (var item in suppliers)
                    {
                        Console.WriteLine("The supplier name is" + " " + item.Name);

                    }
                }
                else
                    Console.WriteLine("The supplier table doesn't has any data!");

            }
        }



        // دالة اضافة مورد
        public void AddSupplier(Supplier supplier)
        {
            using (Assingment3DbContext context = new())
            {
                context.Suppliers.Add(supplier);
                context.SaveChanges();
            }
        }



        // دالة حذف مورد
        public void DeleteSupplier(string name)
        {
            using (Assingment3DbContext context = new())
            {
                // جلب اول مورد يملك الاسم الذي ادخله المستخدم و غير محذوف مسبقا كحذف ناعم 
                var supplier = context.Suppliers.Where(p => p.IsDeleted != true)
                   .FirstOrDefault(s => s.Name == name);

                if (supplier != null)
                {
                    supplier.IsDeleted = true;

                    // استعلام حتى يتم جلب القطع التي قام بتوريدها هذا المورد  
                    //  وحذفها كحذف ناعم 
                    var ThePartProvidedByThisSupplier = context.Parts.Where(
                                                         p => p.SupplierId == supplier.Id).ToList();

                    foreach (var item in ThePartProvidedByThisSupplier)
                    {
                        item.IsDeleted = true;
                    }

                    context.SaveChanges();
                }
                else
                    Console.WriteLine($"There is no supplier with this name {name}");
            }
        }





        // دالة تعديل مورد 
        // حيث يقوم المستخدم بإدخال اسم الخاصية التي يريد تعديلها ثم يدخل القيمة القديمة للخاصية ثم الجديدة
        // لانه يحوي كل الانواع object لذلك استخدمت النوع 
        public void UpdateSupplier(object NameProperty, object oldValue, object newValue)
        {
            using (Assingment3DbContext context = new())
            {
                string NamePropertyAsString = (string)NameProperty;
                string oldValueAsString = "";
                string newValueAsString = "";
                // في حال كانت الخاصية التي ادخلها المستخدم هي اسم المورد
                if (NamePropertyAsString == "Name" || NamePropertyAsString == "name")
                {
                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية اسم المورد نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the suppliers update function," +
                            "and to update the name of a supplier  ,you must enter string values for " +
                         "the old value and the new value");
                    }

                    // جلب اول مورد يملك الاسم الذي ادخله المستخدم ليتم تعديله
                    var supplier = context.Suppliers.Where(c => c.IsDeleted != true).FirstOrDefault(s => s.Name == oldValueAsString);

                    if (supplier != null)
                    {
                        // تعديل اسم المورد
                        supplier.Name = newValueAsString;
                        context.SaveChanges();
                    }
                    else // في حال كان الاسم الذي ادخله المستخدم غير موجود
                        Console.WriteLine($"There is no supplier with this name {oldValueAsString}");
                }

                // في حال كانت الخاصية التي ادخلها المستخدم هي عنوان المورد
                if (NamePropertyAsString == "Address" || NamePropertyAsString == "address")
                {
                    try
                    {
                        // تحويل القيمتين القديمة والجديدة الى النوع سترينغ لان خاصية عنوان المورد نوعها سترينغ
                        oldValueAsString = (string)oldValue;
                        newValueAsString = (string)newValue;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("In the suppliers update function," +
                            "and to update the address of a supplier  ,you must enter string values for " +
                         "the old value and the new value");
                    }

                    // جلب اول مورد يملك العنوان الذي ادخله المستخدم ليتم تعديله
                    var supplier = context.Suppliers.Where(c => c.IsDeleted != true).FirstOrDefault(s => s.Address == oldValueAsString);
                    if (supplier != null)
                    {
                        // تعديل عنوان المورد
                        supplier.Address = newValueAsString;
                        context.SaveChanges();
                    }
                    else // في حال كان العنوان الذي ادخله المستخدم غير موجود
                        Console.WriteLine($"There is no supplier with this address {oldValueAsString}");
                }

                else // في حال كان اسم الخاصية التي ادخلها المستخدم ليست من خصائص المورد
                    Console.WriteLine("The property you entered does not exist in the car class, enter an existing property !");
            }
        }

    }

}
