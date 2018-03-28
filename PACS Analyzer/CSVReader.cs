using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace PACS_Analyzer
{
    [IgnoreFirst(1)]
    [IgnoreEmptyLines()]
    [DelimitedRecord(",")]
    public sealed class CSVReader
    {
        [FieldTrim(TrimMode.Both)]
        public String empid;

        [FieldTrim(TrimMode.Both)]
        public int a;

        [FieldTrim(TrimMode.Both)]
        public String lastname;

        [FieldTrim(TrimMode.Both)]
        public String firstname;

        [FieldTrim(TrimMode.Both)]
        public String department;

        [FieldTrim(TrimMode.Both)]
        public int office;

        [FieldTrim(TrimMode.Both)]
        // Parse these dates: 2016-05-31 07:17:05
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime timestamp;

        [FieldTrim(TrimMode.Both)]
        public String type;

        [FieldTrim(TrimMode.Both)]
        public String proxId;

        [FieldTrim(TrimMode.Both)]
        public Int32 floor;

        [FieldTrim(TrimMode.Both)]
        public String zone;
    }
}
