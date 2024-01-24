using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignement3_Domain
{
    public class UserConstants
    {
        // We are not taking data from data base so we get data from constant
        // login التعريف المسبق للمستخدم من أجل عملية ال
        public static List<User> Users = new List<User>
        {
         new User { Id = 1 , Name="Asmaa" , Password="Asmaa" , Email="Asmaa@gmail.com" , Age=0 , Role="User" }
        };
    }
}
