using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args) 
        {
            // From http://collab.kpsn.org/display/tc/ChrRaces
            //UInt32[] racemasks = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 
            //                     4096, 8192, 16384, 32768, 65536, 131072, 262144, 
            //                     524288, 1048576, 2097152 };
            CreatureModelData data = new CreatureModelData();
            data.loadDBCFile("Faction.dbc");

            Console.WriteLine("Input 0 to modify race masks, else input 1 to modify class masks:");
            bool raceMask = UInt32.Parse(Console.ReadLine()) == 0 ? true : false;

            Console.WriteLine("Input the mask you want to look for in each row:");
            UInt32 hasMask = UInt32.Parse(Console.ReadLine());

            Console.WriteLine("Input race mask you want to add, 0 if you wish to remove a rask mask:");
            bool remove = false;
            UInt32 modifyMask = UInt32.Parse(Console.ReadLine());
            if (modifyMask == 0)
            {
                remove = true;
                Console.WriteLine("Input race mask you want to remove:");
                modifyMask = UInt32.Parse(Console.ReadLine());
            }

            data.modifyBitMasks(hasMask, modifyMask, remove, raceMask);

            data.SaveDBCFile("new_Faction.dbc");
            
            Console.WriteLine("\nDone. Press any key to continue...");
            Console.ReadKey();
        }
    }
}
