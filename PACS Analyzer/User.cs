using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS_Analyzer
{
    class User
    {
        public int id;
        public string empID;
        public int a;
        public string firstName;
        public string lastName;
        //public int departmentID;
        public string departmentID;
        //public int officeID;
        //public int typeID;
        //public string proxID;

        public bool isOpen = false;
        public int openZone;
        public DateTime openTime;

        /*
         * O - Foreign Object
         * */
        public User(int idO, string empIDO, int aO, string firstNameO, string lastNameO, string departmentIDO)
        {
            id = idO;
            empID = empIDO;
            a = aO;
            firstName = firstNameO;
            lastName = lastNameO;
            departmentID = departmentIDO;
            //officeID = officeIDO;
            //typeID = typeIDO;
            //proxID = proxID;
        }
    }
}
