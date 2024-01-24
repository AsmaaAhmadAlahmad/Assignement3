using Assignement3_Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Channels;

namespace Assignement3_Data
{
    public class Assingment3DbContext : DbContext
    {

        public Assingment3DbContext()
        {

        }
        public Assingment3DbContext(DbContextOptions<Assingment3DbContext> options) : base(options)
        {

        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        // تم استخدام الطريقة التالية لبناء القاعدة وليس طريقة وضع الكونيكشن سترينغ في ملف
        // appSettings 
        // التي هي الطريقةالأفضل والأكثر أمانا  Assingment3Api ثم قرائته في الملف الرئيسي في مشروع
        // 😢ولكن لم أستخدمها بسبب ظهور الخطأ التالي الذي حاولت كثيرا ولم أعرف تصحيحه
        // No database provider has been configured for this DbContext.
        // A provider can be configured by overriding the
        // 'DbContext.OnConfiguring' method or by using 'AddDbContext' on the application
        // service provider. If 'AddDbContext' is used, then also ensure that your
        // DbContext type accepts a DbContextOptions<TContext> object in its constructor
        // and passes it to the base constructor for DbContext.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Assingment3DB;");
        }
    }
}
