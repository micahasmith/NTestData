using NTestData.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using CuttingEdge.Conditions;
using System.Data;
using System.IO;

namespace NTestData.Services
{
    public class TestDataGenerator
    {
        public void Generate(Tables tables,DbConnection connection,Settings settings)
        {
            Condition.Ensures(settings).IsNotNull();
            Condition.Ensures(tables).IsNotNull();
            Condition.Ensures(connection).IsNotNull();
            Condition.Ensures(connection).Evaluate(i=>i.State==ConnectionState.Open);
            Condition.Ensures(settings.GenerateTestData).IsNotNull();
            Condition.Ensures(settings.GenerateTestData.IncludeTables).IsNotNull();
            Condition.Ensures(settings.GenerateTestData.ModelsNamespace).IsNotNull();

            //get the tables settings says we want test data for
            IEnumerable<Table> workset = null;
            if (settings.GenerateTestData.IncludeTables.First() == "*") workset = tables;
            else
                workset = tables.Where(i=>
                    settings.GenerateTestData.IncludeTables.Any(j=>j.Equals(i.Name,StringComparison.InvariantCultureIgnoreCase)));

            //begin writing the CS file
            var dataFile = new StringBuilder(@"
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
");
            //add a reference to the namespace the models are in
            dataFile.AppendLineFormat("using {0};",settings.GenerateTestData.ModelsNamespace);

            //create the file's namespace
            dataFile.AppendLineFormat("namespace {0}.TestData {{", settings.GenerateTestData.ModelsNamespace);

            //create the file's container class
            dataFile.AppendLineIndent(1,"public class TestDataContainer {");

            
            foreach(var table in workset)
            {
                //create the List<T>
                dataFile.AppendLineFormatIndent(2,"public List<{0}> {1}  = new List<{0}>() {{",table.ClassName,table.Name);

                
                var records = connection.Query("select * from "+table.Name);
                foreach(var record in records)
                {
                    //had to make this public in Dapper's source
                    var drow = record as Dapper.SqlMapper.DapperRow;
                    dataFile.AppendLineFormatIndent(3,"new {0} (){{",table.ClassName);

                    //append the data from each table into the class       
                    bool isFirst = true;
                    foreach (var coldef in table.Columns)
                    {
                        object val = null; 
                        drow.TryGetValue(coldef.PropertyName,out val);
                        if (val == null || val.ToString()=="")
                            dataFile.AppendFormat("{0} = null , ", coldef.PropertyName);
                        else
                        {
                            string start = null;
                            string end = null;
                            if (coldef.PropertyType == "string")
                            {
                                start = "\"";
                                end = "\"";
                            }
                            else if (coldef.PropertyType == "decimal")
                            {
                                end = "m";
                            }
                            else if (coldef.PropertyType == "DateTime")
                            {
                                 val = "DateTime.Parse(\""+val+"\")";
                            }
                            if(isFirst)
                                dataFile.AppendFormatIndent(4,"{0} = {1}{2}{3} , ",coldef.PropertyName, start,val,end);
                            else
                                dataFile.AppendFormat( "{0} = {1}{2}{3} , ", coldef.PropertyName, start, val, end);
                            isFirst = false;
                        }
                    }
                    //remove the last comma
                    dataFile = dataFile.Remove(dataFile.Length - 2, 2);
                    //close out the class
                    dataFile.AppendLine("},");
                }
                //remove last comma
                dataFile = dataFile.Remove(dataFile.Length - 2, 1);
            }

            //close the List<T>
            dataFile.AppendLineIndent(2, "};");
            //close the container
            dataFile.AppendLineIndent(1,"}");

            //close the namespace
            dataFile.Append("}");

            string finalPath = null;
            if (string.IsNullOrEmpty(settings.GenerateTestData.WriteToFile))
            {
                var dir = Environment.CurrentDirectory;
                finalPath = Path.Combine(dir, "TestData.cs");
            }
            else
            {
                finalPath = settings.GenerateTestData.WriteToFile;
            }
            File.WriteAllText(finalPath, dataFile.ToString());
        }

    }
}
