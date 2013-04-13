using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTestData
{
    public class Util
    {
        public static Func<string,DbConnection> GetDbConnByProvider(string provider)
        {
            if (provider.Equals("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
                return (str) => new SqlConnection(str);
            return null;
        }
    }
}
