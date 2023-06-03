using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beck_end_lib.Log_functions
{
    internal class LogClass
    {
        public void log_function(string String, bool errorIs, string type)
        {
            if (errorIs)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(String);
            }
            else
            {
                switch (type)
                {
                    case "GET":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "POST":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "PATCH":
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    case "DELETE":
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine(String);
            }
        }
    }
}
