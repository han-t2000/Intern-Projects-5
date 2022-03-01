using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LogErrorValidator
{
    internal class Program
    {
        // list of errors to catch
        readonly System.Collections.Specialized.StringCollection ErrorsToCheck = Properties.Settings.Default.ErrorLogs;

        const string reg = @"^\(.*\)$";
        const string reg1 = @".*&.*";
        const string reg2 = @"^~\(.*\)$";
        const string reg3 = @".*\[.*\].*";

        public bool Checking(string logLine, string Path)
        {
            // Perform check based on the regex pattern and the list of errors in app.config
            foreach (string errorCode in ErrorsToCheck)
            {
                if (Regex.IsMatch(errorCode, reg))
                {
                    if (Match(logLine, errorCode))
                        return true;

                    continue;
                }
                if (logLine.IndexOf(errorCode, StringComparison.CurrentCultureIgnoreCase) > -1)
                    return true;
            }

            if ((Path.ToUpper().Contains("SMIS3")) && logLine.Contains("Real-time connection status (now/max/avg)") || logLine.Contains("HTTP connection status (now/max/avg/request)"))
            {
                String strTemp = logLine.Substring(logLine.IndexOf("):") + 2);
                if (Convert.ToInt16(logLine.Substring(0, strTemp.IndexOf("/"))) >= 500)
                    return true;
            }
            return false;
        }

        public bool Match(string value, string errorCode)
        {
            if (Regex.IsMatch(errorCode, reg))
            {
                errorCode = GetString(errorCode, '(', true);

                return Match(value, errorCode);
            }

            if (Regex.IsMatch(errorCode, reg1))
            {
                var str1 = errorCode.Substring(0, errorCode.IndexOf("&")).Trim();
                var str2 = errorCode.Substring(errorCode.IndexOf("&") + 1).Trim();

                return (Match(value, str1) && Match(value, str2));
            }

            if (Regex.IsMatch(errorCode, reg2))
            {
                errorCode = GetString(errorCode, '(');

                return !Match(value, errorCode);
            }

            if (Regex.IsMatch(errorCode, reg3) && Regex.IsMatch(value, reg3))
            {
                var stringWithin = GetString(errorCode, '[');
                var symbol = stringWithin.Substring(0, 1);
                var valueToCheckAgainst = int.Parse(stringWithin.Substring(stringWithin.IndexOf(symbol) + 1));
                var actualValue = int.Parse(GetString(value, '['));

                if (symbol == ">")
                    if (actualValue > valueToCheckAgainst) return true;
                if (symbol == "<")
                    if (actualValue < valueToCheckAgainst) return true;

                return false;
            }
            return value.Contains(errorCode);
        }

        private readonly Dictionary<char, char> Enclosing = new Dictionary<char, char>() {
            { '[', ']' },
            { '(', ')' }
        };

        private string GetString(string value, char startingChar, bool getLast = false)
        {
            if (!Enclosing.ContainsKey(startingChar))
                throw new ArgumentException("startingChar argument not found in Enclosing Dictionary!");

            int closingIndex = (getLast) ? value.LastIndexOf(Enclosing[startingChar]) : value.IndexOf(Enclosing[startingChar]);

            return value.Substring(value.IndexOf(startingChar) + 1, closingIndex - value.IndexOf(startingChar) - 1).Trim();
        }

        static void Main(string[] args)
        {
            int lineCounter = 1;
            Program instance = new Program();

            foreach (string line in File.ReadLines(@"C:\Temp\test.txt"))
            {
                if (instance.Checking(line, ""))
                    Console.WriteLine($"Detected in line {lineCounter}: {line}");
                lineCounter++;
            }

            Console.ReadKey();
        }
    }
}


//using System;
//using System.IO;
//using System.Text.RegularExpressions;

//namespace LogErrorValidator
//{
//    internal class Program
//    {
//        //list of errors
//        string[] check = Properties.Settings.Default.Setting.Split(',');

//        string reg = @"\(.* &!.*\)";

//        public bool Checking(string value)
//        {
//            foreach (string item in check)
//            {
//                if (value.Contains(item))
//                    return true;
//                //if (Regex.IsMatch(item, reg))
//                //    return true;
//                //Regex.Replace(item, reg, "");
//                //Console.WriteLine(item);
//                ///* string[] list = item.Split('&', '!');
//                // foreach (string i in list)
//                // {
//                //     if(i[0] == )

//                // }*/
//            }
//            return false;
//        }


//        static void Main(string[] args)
//        {
//            //log value            
//            string value = File.ReadAllText(@"C:\Temp\test.txt");

//            Program instance = new Program();

//            //foreach (string item in instance.check)
//            //{
//            //    Console.Write(item);
//            //}

//            Console.WriteLine(instance.Checking(value));

//            Console.ReadKey();
//        }


//    }
//}
