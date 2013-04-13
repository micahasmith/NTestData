using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTestData.Models
{
    public class Settings
    {
        public string ConnString { get; set; }
        public string Provider { get; set; }

        public GenerateTestDataSettings GenerateTestData { get; set; }
    }

    public class GenerateTestDataSettings
    {
        public string ModelsNamespace { get; set; }
        public string[] IncludeTables { get; set; }
        public string WriteToFile { get; set; }
    }
}
