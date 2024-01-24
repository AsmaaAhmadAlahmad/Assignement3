using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Assignement3_Domain.Helper
{
    //  من أجل الكلاس الأصلي T
    // ApiModels  من أجل الكلاس الذي يُستخدم للإضافة والمتواجد في مجلد B
    // ApiModels  من أجل الكلاس الذي يُستخدم للتعديل والمتواجد في مجلد B

    public class HelperMapper<T, B, C>
    {
        public readonly Mapper  MapperForCreate;
        public readonly Mapper MapperForUpdate;
        public readonly Mapper MapperForPartiallyUpdate;
        public readonly Mapper MapperForAddRange;

        public HelperMapper()
        {
            MapperConfiguration configForCreate = new MapperConfiguration(cfg => cfg.CreateMap<B, T>());
            Mapper _mapperForCreate = new Mapper(configForCreate);
            MapperForCreate = _mapperForCreate;


            MapperConfiguration configForUpdate = new MapperConfiguration(cfg => cfg.CreateMap<C, T>());
            Mapper _mapperForUpdate = new Mapper(configForUpdate);
             MapperForUpdate = _mapperForUpdate;


            // mapper نعلم أننا في التعديل الجزئي أولا سنحتاج إلى عمل 
            // من الكائن الموجود في الكلاس الأساسي الى الكائن الموجود في الكلاس الذي
            // يُستخدم للتعديل عندها نستخدم 
            // MapperForPartiallyUpdate
            MapperConfiguration configForPartiallyUpdate = new MapperConfiguration(cfg => cfg.CreateMap<T, C>());
            Mapper _mapperForPartiallyUpdate = new Mapper(configForPartiallyUpdate);
            MapperForPartiallyUpdate = _mapperForPartiallyUpdate;


            //MapperConfiguration configForAddRange = new MapperConfiguration(cfg => cfg.CreateMap<List<B>, List<T>>());
            //Mapper _mapperForAddRange = new Mapper(configForAddRange);
            //MapperForAddRange = _mapperForAddRange;

        }


      }
}
