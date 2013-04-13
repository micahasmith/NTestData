using NTestData.Models;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using NTestData.Services;
using ServiceStack.Text;

namespace NTestData
{
    public class Program
    {
        static void Main(string[] args)
        {
            //settings to make this work--
            //
            //eventually pass this in as args/path to settings file maybe
            //or do app.config....?
            //
            var settings = new Settings()
            {
                ConnString = @"data source=.\sqlexpress;initial catalog=mydb;integrated security=true;",
                Provider = "System.Data.SqlClient",
                GenerateTestData = new GenerateTestDataSettings()
                {
                    ModelsNamespace = "MyNamespace.Models",
                    IncludeTables = new string[]
                    {
                        "ShipmentMethods",
                        "ShippingCarrier_Methods",
                        "ShippingCarriers",
                        "OrderStatus",
                        "Boxes",
                        "BoxShipCosts",
                    }
                }
            };

            using (var db = Util.GetDbConnByProvider(settings.Provider)(settings.ConnString))
            {
                db.Open();

                //get the reader
                var reader = new SqlServerSchemaReader();
                var tables = reader.ReadSchema(db , DbProviderFactories.GetFactory(db ));

                if (settings.GenerateTestData != null)
                    new TestDataGenerator().Generate(tables, db, settings);


            }


        }
    }


    

}
