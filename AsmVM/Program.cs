using System;

namespace AsmVM
{
    class Program
    {
        static void Main(string[] args)
        {
            State x = new State();
            x.WDMU16(0, 0x0001);                  // 0000 MOV16 A, 1A4h
            x.WDMU16(2, 0x0002);                  // 0002
            x.WDMU16(4, 420);                     // 0004
                                                  // 0006 BRK
                                                  // 0008
            Console.WriteLine(x.RDRU16(0x0002));
            x.Execute();
            Console.WriteLine(x.RDRU16(0x0002));
            Console.ReadKey();
        }
    }
}
