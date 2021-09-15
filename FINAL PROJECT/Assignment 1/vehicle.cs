using System;
using System.Collections.Generic;
using System.Text;

namespace MRRC
{
    //General Vehicle construction
    public class Vehicle
    {
        //INitialise all the variables
        private string regestration { get; set; }
        private int Vehiclegrade { get; set; }
        private string make { get; set; }
        private string model { get; set; }
        private int year { get; set; }
        private int numSeats { get; set; }
        private int transmission { get; set; }
        private int Fueltype { get; set; }
        private bool GPS { get; set; }
        private bool sunRoof { get; set; }
        private double dailyRate { get; set; }
        private string colour { get; set; }
        //Private varibales for use in other constructors
        //These will be the deaufult values of certain aspects
        public const int SeatsDeafult = 4;
        public const string TransDeafult = "Manual";
        public const string FuelDeafult = "Petrol";
        public const bool GPSDeafult = false;
        public const bool SunDeafult = false;
        public const double RateDeafult = 50;
        public const string ColourDeafult = "Black";


        //If minimum inputs are required
        public Vehicle(string REGO,
                       string gradeRAW,
                       string make,
                       string model,
                       int year)
        {
            this.regestration = regestration;
            this.Vehiclegrade = (int)Enum.Parse(typeof(VehicleGrade), gradeRAW);
            this.make = make;
            this.model = model;
            this.year = year;
            this.numSeats = SeatsDeafult;
            //Defualt is Manual
            this.transmission = (int)Enum.Parse(typeof(TransmissionType), TransDeafult);
            //Defualt is Petrol
            this.Fueltype = (int)Enum.Parse(typeof(FuelType), FuelDeafult);
            this.GPS = GPSDeafult;
            this.sunRoof = SunDeafult;
            this.dailyRate = RateDeafult;
            this.colour = ColourDeafult;
        }

        //If all inputs are put into file
        public Vehicle(string regestration,
                    string gradeRAW,
                    string make,
                    string model,
                    int year,
                    int numSeats,
                    string transmissionRAW,
                    string fuelRAW,
                    bool GPS,
                    bool sunRoof,
                    double dailyRate,
                    string colour)
        {
            //All variables are assigned
            this.regestration = regestration;
            this.Vehiclegrade = (int)Enum.Parse(typeof(VehicleGrade), gradeRAW);
            this.make = make;
            this.model = model;
            this.year = year;
            this.numSeats = numSeats;
            //Defualt is Manual
            this.transmission = (int)Enum.Parse(typeof(TransmissionType), transmissionRAW);
            //Defualt is Petrol
            this.Fueltype = (int)Enum.Parse(typeof(FuelType), fuelRAW);
            this.GPS = GPS;
            this.sunRoof = sunRoof;
            this.dailyRate = dailyRate;
            this.colour = colour;
        }


        //Returns the Regestration of the vehicle
        public string GetRego()
        {
            return regestration;
        }

        //Returns the string in a CSV file friendly format
        public string ToCSVString()
        {
            string toRETURN;
            const string DELIM = ",";
            toRETURN = regestration + DELIM + Enum.GetName(typeof(VehicleGrade), Vehiclegrade) + DELIM + make + DELIM + model + DELIM + year
                        + DELIM + numSeats + DELIM + Enum.GetName(typeof(TransmissionType), transmission) + DELIM + Enum.GetName(typeof(FuelType), Fueltype)
                        + DELIM + GPS + DELIM + sunRoof + DELIM + dailyRate + DELIM + colour;
            return toRETURN;
        }

        //Returns the whole detials of the indvidual vehicle
        //The boolean variable is for the searching algoritm
        public List<string> GetAttributList(bool searchingAttribute = false)
        {
            List<string> toBeReturned = new List<string> { };
            if (searchingAttribute)
            {
                toBeReturned.Add(this.regestration);
                toBeReturned.Add((Enum.GetName(typeof(VehicleGrade), Vehiclegrade)).ToLower());
                toBeReturned.Add(make.ToLower());
                toBeReturned.Add(model.ToLower());
                toBeReturned.Add(Convert.ToString(year));
                toBeReturned.Add(Convert.ToString(numSeats)+"-seater");
                toBeReturned.Add((Enum.GetName(typeof(TransmissionType), transmission)).ToLower());
                toBeReturned.Add((Enum.GetName(typeof(FuelType), Fueltype)).ToLower());
                toBeReturned.Add(BOOLScanThrough(GPS,true));
                toBeReturned.Add(BOOLScanThrough(sunRoof, false));
                toBeReturned.Add(string.Format("{0:C2}", dailyRate));
                toBeReturned.Add(colour.ToLower());
            }
            else
            {
                toBeReturned.Add(this.regestration);
                toBeReturned.Add(Enum.GetName(typeof(VehicleGrade), Vehiclegrade));
                toBeReturned.Add(make);
                toBeReturned.Add(model);
                toBeReturned.Add(Convert.ToString(year));
                toBeReturned.Add(Convert.ToString(numSeats));
                toBeReturned.Add(Enum.GetName(typeof(TransmissionType), transmission));
                toBeReturned.Add(Enum.GetName(typeof(FuelType), Fueltype));
                toBeReturned.Add(BoolTOString(GPS));
                toBeReturned.Add(BoolTOString(sunRoof));
                toBeReturned.Add(string.Format("{0:C2}", dailyRate));
                toBeReturned.Add(colour);
            };

            return toBeReturned;
        }

        //Takes the input of the new information
        //Reuturns if the car is good to be updated in information
        public bool UpdateExisting(List<string> updatedFields)
        {
            string currentREGO = updatedFields[0];
            bool REGOEXITS = mainProgram.fleetMain.REGOExisiting(currentREGO);
            if (REGOEXITS)
            {
                return false;
            }
            else
            {
                regestration = updatedFields[0];
                Vehiclegrade = (int)Enum.Parse(typeof(VehicleGrade), updatedFields[1]);
                make = updatedFields[2];
                model = updatedFields[3];
                year = Convert.ToInt32(updatedFields[4]);
                numSeats = Convert.ToInt32(updatedFields[5]);
                transmission = (int)Enum.Parse(typeof(TransmissionType), updatedFields[6]);
                Fueltype = (int)Enum.Parse(typeof(FuelType), updatedFields[7]);
                GPS = StringTOBOOL(updatedFields[8]);
                sunRoof = StringTOBOOL(updatedFields[9]);
                dailyRate = float.Parse(updatedFields[10].Substring(1, updatedFields[10].Length - 1));
                colour = updatedFields[11];
                return true;
            };
        }

        //Converts a string YES or NO to a bool
        private bool StringTOBOOL(string input)
        {
            bool toReturn = false;
            bool STRINGTrue = "Yes" == input;
            if (STRINGTrue)
            {
                toReturn = true;
            }
            return toReturn;

        }

        //Converts a bool for GPS or Sunroof for searching algorithim
        //Takes too boolean input is either the vehicle has the aspect
        //Gps is wither or not sunroof
        private string BOOLScanThrough(bool input, bool GPS)
        {
            string toReturn;
            if(GPS)
            {
                if(input)
                {
                    toReturn = "gps";
                }
                else
                {
                    toReturn = "no-gps";
                };
            }
            else
            {
                if(input)
                {
                    toReturn = "sunroof";
                }
                else
                {
                    toReturn = "no-sunroof";
                };
            };
            return toReturn;
        }

        //Converts a bool to a YES or NO string
        private string BoolTOString(bool input)
        {
            string toReturn;
            if (input)
            {
                toReturn = "Yes";
            }
            else
            {
                toReturn = "No";
            };
            return toReturn;
        }

        //Enumeration of certain ranges of cars etc
        enum VehicleGrade
        {
            Economy,
            Family,
            Luxury,
            Commercial
        };

        enum TransmissionType
        {
            Manual,
            Automatic
        };

        enum FuelType
        {
            Petrol,
            Diesel
        };

    }
    
    //Economy Vehicle constructor
    public class VehicleEconomy : Vehicle
    {
        //UNiqness of this grade
        private const string TransDefualtECON = "Automatic";
        public VehicleEconomy(string REGO,
                              string ECONMYGRADE,
                              string MAKE,
                              string model,
                              int year)
                            : base(REGO,
                              ECONMYGRADE,
                              MAKE,
                              model,
                              year,
                              SeatsDeafult,
                              TransDefualtECON,
                              FuelDeafult,
                              GPSDeafult,
                              SunDeafult,
                              RateDeafult,
                              ColourDeafult)
        {

        }

    }

    //Family Vehicle constructer
    public class VehicleFamily : Vehicle
    {
        //UNiqness of this grade
        private const double FAMILYRateDeafult = 80;

        public VehicleFamily(string REGO,
                      string FAMILYGRADE,
                      string MAKE,
                      string model,
                      int year) : base(REGO,
                                     FAMILYGRADE,
                                     MAKE,
                                     model,
                                     year,
                                     SeatsDeafult,
                                     TransDeafult,
                                     FuelDeafult,
                                     GPSDeafult,
                                     SunDeafult,
                                     FAMILYRateDeafult,
                                     ColourDeafult)
        {

        }
    }

    //Class with Luxry vehicles defualts programed
    public class VehicleLuxry : Vehicle
    {
        //UNiqness of this grade
        private const bool LUXGPSDeafult = true;
        private const bool LUXSunDeafult = true;
        private const double LUXRateDeafult = 120;

        public VehicleLuxry(string REGO,
                            string LUXGRADE,
                            string MAKE,
                            string model,
                            int year) : base(REGO,
                                           LUXGRADE,
                                           MAKE,
                                           model,
                                           year,
                                           SeatsDeafult,
                                           TransDeafult,
                                           FuelDeafult,
                                           LUXGPSDeafult,
                                           LUXSunDeafult,
                                           LUXRateDeafult,
                                           ColourDeafult)
        {

        }

    }

    //Commerical Constructor
    public class VehicleCommercial : Vehicle
    {
        //UNiqness of this grade
        private const string COMFuelDeafult = "Diesel";
        private const double COMRateDeafult = 130;

        public VehicleCommercial(string REGO,
                          string COMGRADE,
                          string MAKE,
                          string MODEL,
                          int year) : base(REGO,
                                         COMGRADE,
                                         MAKE,
                                         MODEL,
                                         year,
                                         SeatsDeafult,
                                         TransDeafult,
                                         COMFuelDeafult,
                                         GPSDeafult,
                                         SunDeafult,
                                         COMRateDeafult,
                                         ColourDeafult)
        {

        }
    }
}
