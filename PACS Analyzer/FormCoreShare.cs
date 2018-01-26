using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS_Analyzer
{
    class FormCoreShare
    {
        public string filePath;
        public string fileName;
        //public int fileSize;
        //public List<string> fileLines;
        public int fileLinesNumber;
        public Dictionary<User, int> userList;
        //public List<Zone> zoneList;
        //public List<Floor> floorList;
        //public List<Department> departmentList;
        public List<DateTime> comboBoxTimes;
        public DateTime timeFrom;
        public DateTime timeTill;
        public string connectionString;
        
        public string errorText;
    }
}
