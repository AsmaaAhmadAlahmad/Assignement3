using Assignement3_Data;
using Assignement3_Domain;
using Assignement3_Domain.Handlers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Headers;

namespace Assingment2App
{
    internal class Program
    {


        public static void Main()
        {


            //ماتم طلبه في الطلب الاخير 

            // انشاء مصنع لإضافته  
            Supplier supplier1 = new Supplier()
            {
                Name = "YousefAlahmed",
                Address = "Homs",
                Parts = new List<Part>
                  {
                      new Part
                      {
                          Name="Filters",Quantity=100,Price="603$"
                      },
                  }
            };
            SupplierHandler supplierHandler1 = new SupplierHandler();
            supplierHandler1.AddSupplier(supplier1); // إضافة المصنع




            // إنشاء سيارة لإضافتها 
            Car car1 = new Car
            {
                Model = "Honda Accord2005",
                Gear = "Automatic",
                Km = 60.43,
                Year = 2004,
                Parts = new List<Part>
                {
                    new Part
                    {
                        Name    = "Car Radio" ,
                        Price   = "384$",Quantity=80 ,
                       Supplier = new Supplier { Name="Asmaa", Address = "Hama"}
                    },

                    new Part
                    {
                        Name="Car Glass",
                        Price="123$",Quantity=40,
                        //SupplierId=1,
                        Supplier = new Supplier { Name="Anas", Address = "Hama"}
                    } ,
                }
            };

            CarHandler carHandler1 = new CarHandler();
            carHandler1.CarAdd(car1); // اضافة السيارة
            carHandler1.GetCars();




            // إنشاء جزء لإضافته
            Part part1 = new Part
            {
                Name = "Oil Filter2",
                Price = "200$",
                Quantity = 324,
                Supplier = new Supplier { Name = "Ahmed", Address = "Idleb" },
                Cars = new List<Car>
                {
                      new Car
                      {
                          Model="Jeep Wrangler2005",
                          Gear="Automatic",
                          Km=40,Year=2005
                      },

                       new Car
                       {
                           Model="Tesla Model S2006",
                           Gear="Automatic",
                           Km=60,
                           Year=2006
                       }
                }
            };
            PartHandler partHandler1 = new PartHandler();
            partHandler1.AddParts(part1); // إضافة الجزء




































            ////            //this is test 
            ////            //for the car class

            //Car car = new Car
            //{
            //    Model = "Audi A1",
            //    Gear = "Automatic",
            //    Km = 100.43,
            //    Parts = new List<Part>
            //                {
            //                    new Part
            //                    {
            //                        Name="Engine",
            //                        Price="800",
            //                        Quantity=30,
            //                        Supplier = new Supplier { Name = "Omar", Address = "Aleppo" }
            //                    },
            //                }
            //};
            //CarHandler carHandler = new CarHandler();
            //carHandler.GetCars();
            //carHandler.CarDelete("Audi A4");
            //carHandler.CarAdd(car);
            //carHandler.CarUpdate("model", " Range Rover", " Range Rover2009");



            ////  //    for the customer class
            //Customer Anas = new Customer
            //{
            //    Name = "Anas",
            //    Address = "Idleb",
            //    Age = 28,
            //    Sales = new List<Sales>
            //                        {
            //                           new Sales
            //                           {
            //                               Total = "53.2"
            //                               ,Car  =  new Car
            //                               {
            //                                           Model = "Honda Accord2005",
            //                                           Gear = "Automatic",
            //                                           Km = 60.43,
            //                                           Year = 2004,
            //                               }
            //                           }
            //                         }
            //};

            //CustomerHandler customerHandler = new CustomerHandler();
            //customerHandler.AddCustomer(Anas);
            //customerHandler.UpdateCustomer("Name", "Anas", "Asmaa");
            //customerHandler.GetCustomer();
            //customerHandler.DeleteCustomer("Anas");



            ////for the part class
            //Part part = new Part
            //{
            //    Name = "Wheels",
            //    Price = "100$",
            //    Quantity = 1000,
            //    Supplier = new Supplier { Name = "Mohamed", Address = "Homs" },
            //    Cars = new List<Car>
            //                         {
            //                            new Car { Model=" Range Rover",Gear="Automatic",Km=80,Year=2000},
            //                            new Car { Model="Mercedes-Benz",Gear="Automatic",Km=80,Year=1993}
            //                         }

            //};
            //PartHandler handler = new PartHandler();
            //handler.AddParts(part);
            //handler.GetParts();
            //handler.UpdateParts("Quantity", 1000, 500);
            //handler.RemoveParts("Wheels");

            //////for the sale class
            //Sales sales = new Sales
            //{
            //    Total = "234.6",
            //    Car = new Car { Gear = "test ", Km = 43, Model = "test", Year = 2001 },
            //    Customer = new Customer { Name = "test", Address = "Test" },
            //};
            //SalesHandler salesHandler = new SalesHandler();
            //salesHandler.AddSales(sales);
            //salesHandler.UpdateSales("total", "234.6", "547.8");
            //salesHandler.GetSales();

            //////for supplier class
            //SupplierHandler supplierHandler = new SupplierHandler();
            ////supplierHandler.GetSupplier();

            //Supplier supplier = new Supplier()
            //{
            //    Name = "Ahmed",
            //    Address = "Idleb",
            //    Parts = new List<Part>
            //                  {
            //                      new Part
            //                      {
            //                          Name="Battery",Quantity=34,Price="400$"
            //                      },
            //                  }
            //};
            //supplierHandler.AddSupplier(supplier);
            //supplierHandler.DeleteSupplier("osame");
            //supplierHandler.UpdateSupplier("name", "Ahmed", "osame");

        }


    }

}