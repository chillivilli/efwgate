using Gateways.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EfawateerGateway
{
    public static class Extensions
    {
        public static string Get(this StringList strings, string name)
        {
            if (strings.ContainsKey(name))
                return strings[name];
            var msg = string.Format("Параметр {0} не найден в коллекции '{1}'", name, strings.Strings);
            throw new KeyNotFoundException(msg);
        }

        public static string FormatParameters(this string parameters, string operatorFormatString)
        {
            var delimiter = "\n";
            if (parameters.Contains("\\n"))
                delimiter = "\\n";
            string[] lines = parameters.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            string result = operatorFormatString;
            foreach (string line in lines)
            {
                string[] prms = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (prms.Length < 2)
                    continue;
                result = result.Replace("[#" + prms[0] + "]", prms[1]);
            }

            if (result == operatorFormatString)
            {
                throw new Exception("Unexpected format string " + result);
            }
            return result;
        }

        public static string GetParamsString(this StringList strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var str in strings)
            {
                sb.AppendFormat("{0}={1}\\n", str.Key, str.Value);
            }

            return sb.ToString();
        }

    }
}
