using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTestData
{
    public static class Extensions
    {
        public static void AppendLineFormat(this StringBuilder me, string format, params object[] args)
        {
            me.AppendFormat(format, args);
            me.AppendLine("");
        }

        public static void AppendLineIndent(this StringBuilder me, int tabs, string text)
        {
            for (int i = 0; i < tabs; i += 1)
                me.Append("\t");
            me.AppendLine(text);
        }
        public static void AppendIndent(this StringBuilder me, int tabs, string text)
        {
            for (int i = 0; i < tabs; i += 1)
                me.Append("\t");
            me.Append(text);
        }

        public static void AppendFormatIndent(this StringBuilder me, int tabs, string text,params object[] args)
        {
            for (int i = 0; i < tabs; i += 1)
                me.Append("\t");
            me.AppendFormat(text,args);
        }

        public static void AppendLineFormatIndent(this StringBuilder me, int tabs, string format, params object[] args)
        {
            for (int i = 0; i < tabs; i += 1)
                me.Append("\t");
            me.AppendLineFormat(format, args);
        }
    }
}
