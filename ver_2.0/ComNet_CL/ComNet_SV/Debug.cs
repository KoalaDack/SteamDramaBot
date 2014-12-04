using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComNet_SV
{
    public class Debug
    {
        public static void Print( string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(string.Format("[Message] {0}", message));
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format("[ERROR] {0}", message.ToUpper()));
            Console.ResetColor();
        }

        public static void PrintDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format("[Debug] {0}", message));
            Console.ResetColor();
        }

        public static void PrintEvent(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine(string.Format("[Event] {0}", message));
            Console.ResetColor();
        }
    }
}
