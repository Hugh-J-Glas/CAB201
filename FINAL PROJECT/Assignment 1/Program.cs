using System;
using static System.Console;
using static MRRC.display;
using System.Collections.Generic;
using System.IO;

namespace MRRC
{
    //INITAL STARTUP of the Program
    public class mainProgram
    {
        
        private static string fileLocationCustomers = "..\\..\\..\\..\\Data\\customers.csv";
        private static string filelocationFleets = "..\\..\\..\\..\\Data\\fleet.csv";
        private static string filelocationRentals = "..\\..\\..\\..\\Data\\rentals.csv";
        
        public static CRM CRMMain = new CRM(fileLocationCustomers);
        public static Fleet fleetMain = new Fleet(filelocationFleets, filelocationRentals);
        
        
        public static bool NOTRecording;
             

        static public void Main()
        {
            NOTRecording = true;

                      
            MainScreen();
        }
    }
}
