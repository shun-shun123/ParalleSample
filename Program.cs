using System;
using System.Threading.Tasks;

namespace A
{
    class Program
    {
        private static int N = 1000;
        
        static void Main(string[] args)
        {
            // using a named method
            Parallel.For(0, N, Method2);

            // using an anonymous method
            Parallel.For(0, N, delegate(int i)
            {
                Console.WriteLine($"delegate: {i}");
            });

            // using a lambda expression
            Parallel.For(0, N, i =>
            {
                Console.WriteLine($"lambda: {i}");
            });
        }

        private static void Method2(int i)
        {
            Console.WriteLine($"Method2: {i}");
        }
    }
}
