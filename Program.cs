using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace LogErrorValidator
{
    internal class Program
    {
        // list of errors to catch
        readonly StringCollection ErrorsToCheck = Properties.Settings.Default.ErrorLogs;

        const string pattern = @"\(.*&.*\)";

        public bool Checking(string logFile, string Path)
        {
            // Perform check based on the regex pattern and the list of errors in app.config
            foreach (string error in ErrorsToCheck)
            {
                if (Regex.IsMatch(error, pattern))
                {
                    string str1 = error.Substring(error.IndexOf("(") + 1, error.IndexOf("&") - 1);
                    string str2 = error.Substring(error.IndexOf("&") + 2);
                    str2 = str2.Remove(str2.IndexOf(')'));

                    if ((logFile.Contains(str1) && logFile.Contains(str2))) return true;
                }
                if (logFile.IndexOf(error, StringComparison.CurrentCultureIgnoreCase) > -1) return true;
            }

            if ((logFile.Contains("Failed to load") && !(logFile.Contains("Failed to load stock history info")))) return true;

            if (logFile.Contains("Info: Total of [") && logFile.Contains("] message(s) in queue"))
            {
                int amount = Convert.ToInt16(logFile.Substring(logFile.IndexOf("[") + 1, (logFile.IndexOf("]") - logFile.IndexOf("[")) - 1));
                if (amount > 100) return true;
            }

            if ((Path.ToUpper().Contains("SMIS3")) && logFile.Contains("Real-time connection status (now/max/avg)") || logFile.Contains("HTTP connection status (now/max/avg/request)"))
            {
                string strTemp = logFile.Substring(logFile.IndexOf("):") + 2);
                if (Convert.ToInt16(logFile.Substring(0, strTemp.IndexOf("/"))) >= 500) return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            int lineCounter = 1;
            Program instance = new Program();

            foreach (string error in File.ReadLines(@"C:\Users\Razer Stealth Blade\source\repos\LogErrorValidator\test.txt"))
            {
                var result = instance.Checking(error, "");
                if (result == true)
                    Console.WriteLine($"Error of \"{error}\" detected in line {lineCounter}");
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