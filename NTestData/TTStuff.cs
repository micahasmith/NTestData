using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NTestData
{
    public class Table
    {
        public List<Column> Columns;
        public List<TableIndex> Indices;
        public List<FKey> FKeys;
        public string Name;
        public string Schema;
        public bool IsView;
        public string CleanName;
        public string ClassName;
        public string SequenceName;
        public bool Ignore;
        public string SQL;

        public Column PK
        {
            get
            {
                return this.Columns.SingleOrDefault(x => x.IsPK);
            }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.Name, columnName, true) == 0);
        }

        public Column this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }

        public bool HasPK()
        {
            return ((PK != null) && (string.IsNullOrEmpty(PK.Name) != true));
        }
        public TableIndex GetIndex(string indexName)
        {
            return Indices.Single(x => string.Compare(x.Name, indexName, true) == 0);
        }
    }

    public class IndexColumn
    {
        public string Name;
        public bool IsAsc;
    }

    public class TableIndex
    {
        public string Name;
        public List<IndexColumn> IndexColumns;
        public bool IsUnique;
        public string SQL;
    }

    public class FKey
    {
        public string ToTable;
        public string FromColumn;
        public string ToColumn;
    }

    public class Column
    {
        public string Name;
        public string PropertyName;
        public string PropertyType;
        public bool IsPK;
        public bool IsNullable;
        public bool IsAutoIncrement;
        public bool Ignore;
        public int Size;
        public int Precision;
        public string DefaultValue;
        public string ProperPropertyType
        {
            get
            {
                if (IsNullable)
                {
                    if (PropertyType.Equals("string") == false)
                    {
                        return PropertyType + "?";
                    }
                }
                return PropertyType;
            }
        }
    }

    public class Tables : List<Table>
    {
        public Tables()
        {
        }

        public Table GetTable(string tableName)
        {
            return this.Single(x => string.Compare(x.Name, tableName, true) == 0);
        }

        public Table this[string tableName]
        {
            get
            {
                return GetTable(tableName);
            }
        }

    }








    public static class Inflector
    {
        private static readonly List<InflectorRule> _plurals = new List<InflectorRule>();
        private static readonly List<InflectorRule> _singulars = new List<InflectorRule>();
        private static readonly List<string> _uncountables = new List<string>();

        /// <summary>
        /// Initializes the <see cref="Inflector"/> class.
        /// </summary>
        static Inflector()
        {
            AddPluralRule("$", "s");
            AddPluralRule("s$", "s");
            AddPluralRule("(ax|test)is$", "$1es");
            AddPluralRule("(octop|vir)us$", "$1i");
            AddPluralRule("(alias|status)$", "$1es");
            AddPluralRule("(bu)s$", "$1ses");
            AddPluralRule("(buffal|tomat)o$", "$1oes");
            AddPluralRule("([ti])um$", "$1a");
            AddPluralRule("sis$", "ses");
            AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPluralRule("(hive)$", "$1s");
            AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
            AddPluralRule("(x|ch|ss|sh)$", "$1es");
            AddPluralRule("(matr|vert|ind)ix|ex$", "$1ices");
            AddPluralRule("([m|l])ouse$", "$1ice");
            AddPluralRule("^(ox)$", "$1en");
            AddPluralRule("(quiz)$", "$1zes");

            AddSingularRule("s$", String.Empty);
            AddSingularRule("ss$", "ss");
            AddSingularRule("(n)ews$", "$1ews");
            AddSingularRule("([ti])a$", "$1um");
            AddSingularRule("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingularRule("(^analy)ses$", "$1sis");
            AddSingularRule("([^f])ves$", "$1fe");
            AddSingularRule("(hive)s$", "$1");
            AddSingularRule("(tive)s$", "$1");
            AddSingularRule("([lr])ves$", "$1f");
            AddSingularRule("([^aeiouy]|qu)ies$", "$1y");
            AddSingularRule("(s)eries$", "$1eries");
            AddSingularRule("(m)ovies$", "$1ovie");
            AddSingularRule("(x|ch|ss|sh)es$", "$1");
            AddSingularRule("([m|l])ice$", "$1ouse");
            AddSingularRule("(bus)es$", "$1");
            AddSingularRule("(o)es$", "$1");
            AddSingularRule("(shoe)s$", "$1");
            AddSingularRule("(cris|ax|test)es$", "$1is");
            AddSingularRule("(octop|vir)i$", "$1us");
            AddSingularRule("(alias|status)$", "$1");
            AddSingularRule("(alias|status)es$", "$1");
            AddSingularRule("^(ox)en", "$1");
            AddSingularRule("(vert|ind)ices$", "$1ex");
            AddSingularRule("(matr)ices$", "$1ix");
            AddSingularRule("(quiz)zes$", "$1");

            AddIrregularRule("person", "people");
            AddIrregularRule("man", "men");
            AddIrregularRule("child", "children");
            AddIrregularRule("sex", "sexes");
            AddIrregularRule("tax", "taxes");
            AddIrregularRule("move", "moves");

            AddUnknownCountRule("equipment");
            AddUnknownCountRule("information");
            AddUnknownCountRule("rice");
            AddUnknownCountRule("money");
            AddUnknownCountRule("species");
            AddUnknownCountRule("series");
            AddUnknownCountRule("fish");
            AddUnknownCountRule("sheep");
        }

        /// <summary>
        /// Adds the irregular rule.
        /// </summary>
        /// <param name="singular">The singular.</param>
        /// <param name="plural">The plural.</param>
        private static void AddIrregularRule(string singular, string plural)
        {
            AddPluralRule(String.Concat("(", singular[0], ")", singular.Substring(1), "$"), String.Concat("$1", plural.Substring(1)));
            AddSingularRule(String.Concat("(", plural[0], ")", plural.Substring(1), "$"), String.Concat("$1", singular.Substring(1)));
        }

        /// <summary>
        /// Adds the unknown count rule.
        /// </summary>
        /// <param name="word">The word.</param>
        private static void AddUnknownCountRule(string word)
        {
            _uncountables.Add(word.ToLower());
        }

        /// <summary>
        /// Adds the plural rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddPluralRule(string rule, string replacement)
        {
            _plurals.Add(new InflectorRule(rule, replacement));
        }

        /// <summary>
        /// Adds the singular rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddSingularRule(string rule, string replacement)
        {
            _singulars.Add(new InflectorRule(rule, replacement));
        }

        /// <summary>
        /// Makes the plural.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakePlural(string word)
        {
            return ApplyRules(_plurals, word);
        }

        /// <summary>
        /// Makes the singular.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeSingular(string word)
        {
            return ApplyRules(_singulars, word);
        }

        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="rules">The rules.</param>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        private static string ApplyRules(IList<InflectorRule> rules, string word)
        {
            string result = word;
            if (!_uncountables.Contains(word.ToLower()))
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    string currentPass = rules[i].Apply(word);
                    if (currentPass != null)
                    {
                        result = currentPass;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the string to title case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            return Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])",
                delegate(Match match) { return match.Captures[0].Value.ToUpper(); });
        }

        /// <summary>
        /// Converts the string to human case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToHumanCase(string lowercaseAndUnderscoredWord)
        {
            return MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }


        /// <summary>
        /// Adds the underscores.
        /// </summary>
        /// <param name="pascalCasedWord">The pascal cased word.</param>
        /// <returns></returns>
        public static string AddUnderscores(string pascalCasedWord)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"), @"[-\s]", "_").ToLower();
        }

        /// <summary>
        /// Makes the initial caps.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialCaps(string word)
        {
            return String.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1).ToLower());
        }

        /// <summary>
        /// Makes the initial lower case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialLowerCase(string word)
        {
            return String.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
        }


        /// <summary>
        /// Determine whether the passed string is numeric, by attempting to parse it to a double
        /// </summary>
        /// <param name="str">The string to evaluated for numeric conversion</param>
        /// <returns>
        /// 	<c>true</c> if the string can be converted to a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStringNumeric(string str)
        {
            double result;
            return (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Adds the ordinal suffix.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string AddOrdinalSuffix(string number)
        {
            if (IsStringNumeric(number))
            {
                int n = int.Parse(number);
                int nMod100 = n % 100;

                if (nMod100 >= 11 && nMod100 <= 13)
                    return String.Concat(number, "th");

                switch (n % 10)
                {
                    case 1:
                        return String.Concat(number, "st");
                    case 2:
                        return String.Concat(number, "nd");
                    case 3:
                        return String.Concat(number, "rd");
                    default:
                        return String.Concat(number, "th");
                }
            }
            return number;
        }

        /// <summary>
        /// Converts the underscores to dashes.
        /// </summary>
        /// <param name="underscoredWord">The underscored word.</param>
        /// <returns></returns>
        public static string ConvertUnderscoresToDashes(string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }



        /// <summary>
        /// Summary for the InflectorRule class
        /// </summary>
        private class InflectorRule
        {
            /// <summary>
            /// 
            /// </summary>
            public readonly Regex regex;

            /// <summary>
            /// 
            /// </summary>
            public readonly string replacement;

            /// <summary>
            /// Initializes a new instance of the <see cref="InflectorRule"/> class.
            /// </summary>
            /// <param name="regexPattern">The regex pattern.</param>
            /// <param name="replacementText">The replacement text.</param>
            public InflectorRule(string regexPattern, string replacementText)
            {
                regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                replacement = replacementText;
            }

            /// <summary>
            /// Applies the specified word.
            /// </summary>
            /// <param name="word">The word.</param>
            /// <returns></returns>
            public string Apply(string word)
            {
                if (!regex.IsMatch(word))
                    return null;

                string replace = regex.Replace(word, replacement);
                if (word == word.ToUpper())
                    replace = replace.ToUpper();

                return replace;
            }
        }
    }





    public abstract class SchemaReader
    {
        public abstract Tables ReadSchema(DbConnection connection, DbProviderFactory factory);
        public void WriteLine(string o)
        {
            Console.WriteLine(o);
        }

        public static int GetDatatypePrecision(string type)
        {
            int startPos = type.IndexOf(",");
            if (startPos < 0)
                return -1;
            int endPos = type.IndexOf(")");
            if (endPos < 0)
                return -1;
            string typePrecisionStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result = -1;
            if (int.TryParse(typePrecisionStr, out result))
                return result;
            else
                return -1;
        }

        public static int GetDatatypeSize(string type)
        {
            int startPos = type.IndexOf("(");
            if (startPos < 0)
                return -1;
            int endPos = type.IndexOf(",");
            if (endPos < 0)
            {
                endPos = type.IndexOf(")");
            }
            string typeSizeStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result = -1;
            if (int.TryParse(typeSizeStr, out result))
                return result;
            else
                return -1;
        }

    }











    public class SqlServerSchemaReader : SchemaReader
    {

        static Regex rxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        static string[] cs_keywords = { "abstract", "event", "new", "struct", "as", "explicit", "null", 
	 "switch", "base", "extern", "object", "this", "bool", "false", "operator", "throw", 
	 "break", "finally", "out", "true", "byte", "fixed", "override", "try", "case", "float", 
	 "params", "typeof", "catch", "for", "private", "uint", "char", "foreach", "protected", 
	 "ulong", "checked", "goto", "public", "unchecked", "class", "if", "readonly", "unsafe", 
	 "const", "implicit", "ref", "ushort", "continue", "in", "return", "using", "decimal", 
	 "int", "sbyte", "virtual", "default", "interface", "sealed", "volatile", "delegate", 
	 "internal", "short", "void", "do", "is", "sizeof", "while", "double", "lock", 
	 "stackalloc", "else", "long", "static", "enum", "namespace", "string" };

        static Func<string, string> CleanUp = (str) =>
        {
            str = rxCleanUp.Replace(str, "_");

            if (char.IsDigit(str[0]) || cs_keywords.Contains(str))
                str = "@" + str;

            return str;
        };

        // SchemaReader.ReadSchema
        public override Tables ReadSchema(DbConnection connection, DbProviderFactory factory)
        {
            var result = new Tables();

            _connection = connection;
            _factory = factory;

            var cmd = _factory.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = TABLE_SQL;

            //pull the tables in a reader
            using (cmd)
            {

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Table tbl = new Table();
                        tbl.Name = rdr["TABLE_NAME"].ToString();
                        tbl.Schema = rdr["TABLE_SCHEMA"].ToString();
                        tbl.IsView = string.Compare(rdr["TABLE_TYPE"].ToString(), "View", true) == 0;
                        tbl.CleanName = CleanUp(tbl.Name);
                        tbl.ClassName = Inflector.MakeSingular(tbl.CleanName);

                        result.Add(tbl);
                    }
                }
            }

            foreach (var tbl in result)
            {
                tbl.Columns = LoadColumns(tbl);

                // Mark the primary key
                string PrimaryKey = GetPK(tbl.Name);
                var pkColumn = tbl.Columns.SingleOrDefault(x => x.Name.ToLower().Trim() == PrimaryKey.ToLower().Trim());
                if (pkColumn != null)
                {
                    pkColumn.IsPK = true;
                }
            }


            return result;
        }

        DbConnection _connection;
        DbProviderFactory _factory;


        List<Column> LoadColumns(Table tbl)
        {

            using (var cmd = _factory.CreateCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = COLUMN_SQL;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = tbl.Name;
                cmd.Parameters.Add(p);

                p = cmd.CreateParameter();
                p.ParameterName = "@schemaName";
                p.Value = tbl.Schema;
                cmd.Parameters.Add(p);

                var result = new List<Column>();
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Column col = new Column();
                        col.Name = rdr["ColumnName"].ToString();
                        col.PropertyName = CleanUp(col.Name);
                        col.PropertyType = GetPropertyType(rdr["DataType"].ToString());
                        col.Size = GetDatatypeSize(rdr["DataType"].ToString());
                        col.Precision = GetDatatypePrecision(rdr["DataType"].ToString());
                        col.IsNullable = rdr["IsNullable"].ToString() == "YES";
                        col.IsAutoIncrement = ((int)rdr["IsIdentity"]) == 1;
                        result.Add(col);
                    }
                }

                return result;
            }
        }

        string GetPK(string table)
        {

            string sql = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

            using (var cmd = _factory.CreateCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandText = sql;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tableName";
                p.Value = table;
                cmd.Parameters.Add(p);

                var result = cmd.ExecuteScalar();

                if (result != null)
                    return result.ToString();
            }

            return "";
        }

        string GetPropertyType(string sqlType)
        {
            string sysType = "string";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                    sysType = "DateTime";
                    break;
                case "datetimeoffset":
                    sysType = "DateTimeOffset";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                    break;
            }
            return sysType;
        }



        const string TABLE_SQL = @"SELECT *
		FROM  INFORMATION_SCHEMA.TABLES
		WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'";

        const string COLUMN_SQL = @"SELECT 
			TABLE_CATALOG AS [Database],
			TABLE_SCHEMA AS Owner, 
			TABLE_NAME AS TableName, 
			COLUMN_NAME AS ColumnName, 
			ORDINAL_POSITION AS OrdinalPosition, 
			COLUMN_DEFAULT AS DefaultSetting, 
			IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
			CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
			DATETIME_PRECISION AS DatePrecision,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
			COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
		FROM  INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
		ORDER BY OrdinalPosition ASC";

    }
}

