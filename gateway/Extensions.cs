using Gateways.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EfawateerGateway
{
    public static class Extensions
    {
        public static string Get(this StringList strings, string name)
        {
            if (strings.ContainsKey(name))
            {
                string result = strings[name];
                if (result.StartsWith("[#"))
                    return string.Empty;
                return strings[name];
            }
            var msg = string.Format("Параметр {0} не найден в коллекции '{1}'", name, strings.Strings);
            throw new KeyNotFoundException(msg);
        }


        public static string FormatParameters(this string parameters, string operatorFormatString)
        {
            var delimiter = "\n";
            if (parameters.Contains("\\n"))
                delimiter = "\\n";
            string[] lines = parameters.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            
            string[] opLines = operatorFormatString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            foreach (string line in opLines)
            {
                string[] prms = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (prms.Length < 2)
                    continue;

                keyValues.Add(prms[0], prms[1]);
            }

            Dictionary<string, string> paramValues = new Dictionary<string, string>();

            foreach(string line in lines)
            {
                string[] prms = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (prms.Length < 2)
                    continue;
                if (keyValues.ContainsKey(prms[0]))
                    keyValues[prms[0]] = prms[1];

                paramValues.Add(prms[0], prms[1]);
            }

            StringBuilder result = new StringBuilder();

            foreach( var pair in keyValues)
            {
                if (!pair.Value.StartsWith("[#"))    
                    result.AppendFormat("{0}={1};", pair.Key, pair.Value);
                
            }

            string sbResult = result.ToString();


            return sbResult.Substring(0, sbResult.Length - 1);

        }

        //public static string FormatParameters(this string parameters, string operatorFormatString)
        //{
        //    var delimiter = "\n";
        //    if (parameters.Contains("\\n"))
        //        delimiter = "\\n";
        //    string[] lines = parameters.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        //    string result = operatorFormatString;
        //    foreach (string line in lines)
        //    {
        //        string[] prms = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (prms.Length < 2)
        //            continue;
        //        result = result.Replace("[#" + prms[0].ToUpperInvariant() + "]", prms[1]);
        //    }

        //    if (result == operatorFormatString)
        //    {
        //        throw new Exception("Unexpected format string " + result);
        //    }
        //    return result;
        //}

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
