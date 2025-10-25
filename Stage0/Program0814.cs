using System;

namespace Stage0
{

    partial class Program
    {
        
        static void Main(string[] args)
        {
            Welcome0814();
            Welcome8695();
            Console.WriteLine("press any key to continue...");
            Console.ReadKey();

        }

        static partial void Welcome8695();
        private static  void Welcome0814()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0} welcom to my first console appliction", name);
        }
    }

}