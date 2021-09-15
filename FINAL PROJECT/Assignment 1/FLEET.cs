using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static MRRC.searchingAlgorithim;

namespace MRRC
{
    //This class handles both the vehicle database and also the link between vehicle and customer
    public class Fleet
    {

        private List<Vehicle> vehiclesDatabase = new List<Vehicle> { };
        private Dictionary<string, int> rentals = new Dictionary<string, int> { };

        public bool fleetOpen = false;
        public bool rentalsOpen = false;
        public bool fileOpen = false;

        //Constructor makes the databases in which the whole store operates from
        public Fleet(string fleetFile,
                    string rentalsFile)
        {
            fleetOpen = false;
            rentalsOpen = false;
            fileOpen = false;
            //Declare Variables for use throughout the addition of the database
            string fleetStringRAW;
            string rentalStringRAW;
            string[] fleetString;
            string[] rentalString;
            bool skipFirstLine = true;
            const char DELIM = ',';
            //Exception can occur if the files are currently open
            //Checks if files are located in directory provided
            bool fleetCheck = File.Exists(fleetFile);
            bool rentalCheck = File.Exists(rentalsFile);

            try
            {
                //When it is succesfully located files
                if (fleetCheck & rentalCheck)
                {
                    //Starts reading from the file
                    FileStream fleetRAWFile = new FileStream(fleetFile, FileMode.Open, FileAccess.Read);

                    StreamReader fleetReader = new StreamReader(fleetRAWFile);
                    fleetStringRAW = fleetReader.ReadLine();
                    while (fleetStringRAW != null)
                    {
                        //Skips first line once that has been done adds vehicle to the database
                        if (!skipFirstLine)
                        {
                            fleetString = fleetStringRAW.Split(DELIM);
                            Vehicle newVehicle = new Vehicle(fleetString[0],
                                                            fleetString[1],
                                                            fleetString[2],
                                                            fleetString[3],
                                                            Convert.ToInt32(fleetString[4]),
                                                            Convert.ToInt32(fleetString[5]),
                                                            fleetString[6], fleetString[7],
                                                            Convert.ToBoolean(fleetString[8]),
                                                            Convert.ToBoolean(fleetString[9]),
                                                            Convert.ToDouble(fleetString[10]),
                                                            fleetString[11]);
                            vehiclesDatabase.Add(newVehicle);
                        }
                        else
                        {
                            skipFirstLine = false;
                        };
                        fleetStringRAW = fleetReader.ReadLine();
                    };

                    //Opens the rental files and reads the inforamtion
                    skipFirstLine = true;
                    FileStream rentalRAWFile = new FileStream(rentalsFile, FileMode.Open, FileAccess.Read);
                    StreamReader rentalReader = new StreamReader(rentalRAWFile);
                    rentalStringRAW = rentalReader.ReadLine();
                    //Keeps reading until a null is returned
                    while (rentalStringRAW != null)
                    {
                        //Skips first line
                        if (!skipFirstLine)
                        {
                            rentalString = rentalStringRAW.Split(DELIM);
                            rentals.Add(rentalString[0], Convert.ToInt32(rentalString[1]));
                        }
                        else
                        {
                            skipFirstLine = false;
                        };
                        rentalStringRAW = rentalReader.ReadLine();
                    };
                    //Closes all of the reads on the file
                    rentalReader.Close();
                    rentalRAWFile.Close();
                    fleetRAWFile.Close();
                    fleetReader.Close();
                };
            }
            catch(IOException e)
            {
                if (e.Message.Contains("fleet(1).csv"))
                {
                    fleetOpen = true;
                }
                else if (e.Message.Contains("rentals.csv"))
                {
                    rentalsOpen = true;
                }
                else
                {
                    fileOpen = true;
                }
            }
        }

        //Checks if the porvided regestration exists in the database
        //Returns true or false if there is multiple existing
        public bool REGOExisiting(string newREGO)
        {
            int count = 0;
            foreach (Vehicle vehicleCheck in vehiclesDatabase)
            {
                string oldREGO = vehicleCheck.GetRego();
                if (newREGO == oldREGO)
                {
                    count++;
                }
            };
            //As long as there one Rego in the database it is fine
            //(this is becuase the currently editing vehicle could not have changed rego)
            //If there is 2 then it returns as a false result
            if (count > 1)
            {
                return true;
            }
            else
            {
                return false;
            };
        }

        //This takes the operation AND/OR and returns the matching results if it is true
        //The inputs are dependent on the exisiting information found and also the operator type
        public List<Vehicle> SearchThrough(BinarayTokens Operator = null, VALUEToken firstValue = null, List<Vehicle> exisitngResults = null, List<Vehicle> secoundResults = null, VALUEToken secoundValue = null)
        {
            List<Vehicle> toBeReturned = new List<Vehicle> { };
            List<string> vehicleInfor = new List<string> { };

            bool twoLists = firstValue == null;
            bool processSelection = secoundValue != null;
            bool singleQuery = Operator == null & firstValue != null;
            bool matchingCritera;
            string nameOfOperator;
            bool alreadyExists;
            bool ORoperation;

            //If it is a simple TERM search
            if(singleQuery)
            {
                foreach (Vehicle currentVehicle in mainProgram.fleetMain.GetVehicles())
                {
                    vehicleInfor = currentVehicle.GetAttributList(true);
                    matchingCritera = vehicleInfor.Contains(firstValue.TERM());
                    if(matchingCritera)
                    {
                        toBeReturned.Add(currentVehicle);
                    }
                }
            }
            //If it is a simple TERM TERM operator in shunting yard
            else if (processSelection)
            {
                nameOfOperator = Operator.OperatorName();
                ORoperation = nameOfOperator == "OR";
                foreach (Vehicle currentVehicle in mainProgram.fleetMain.GetVehicles())
                {
                    vehicleInfor = currentVehicle.GetAttributList(true);
                    if(ORoperation)
                    {
                        matchingCritera = vehicleInfor.Contains(firstValue.TERM()) | vehicleInfor.Contains(secoundValue.TERM());
                        if(matchingCritera)
                        {
                            toBeReturned.Add(currentVehicle);
                        };
                    }
                    else
                    {
                        matchingCritera = vehicleInfor.Contains(firstValue.TERM()) & vehicleInfor.Contains(secoundValue.TERM());
                        if(matchingCritera)
                        {
                            toBeReturned.Add(currentVehicle);
                        };
                    };
                };
            }
            //If it is a LIST LIST operator in shunting yard
            else if (twoLists)
            {
                nameOfOperator = Operator.OperatorName();
                ORoperation = nameOfOperator == "OR";
                if (ORoperation)
                {
                    toBeReturned = exisitngResults;
                    foreach(Vehicle toBeAdded in secoundResults)
                    {
                        toBeReturned.Add(toBeAdded);
                    };
                }
                else
                {
                    bool existsInBoth;
                    foreach(Vehicle toBeChecked in exisitngResults)
                    {
                        existsInBoth = secoundResults.Contains(toBeChecked);
                        if(existsInBoth)
                        {
                            toBeReturned.Add(toBeChecked);
                        }
                    }
                }
            }
            //If it is a TERM result operator in shunting yard
            else
            {
                nameOfOperator = Operator.OperatorName();
                ORoperation = nameOfOperator == "OR";
                if (ORoperation)
                {
                    foreach (Vehicle currentVehicle in mainProgram.fleetMain.GetVehicles())
                    {
                        toBeReturned = exisitngResults;
                        vehicleInfor = currentVehicle.GetAttributList(true);
                        matchingCritera = vehicleInfor.Contains(firstValue.TERM());
                        alreadyExists = !exisitngResults.Contains(currentVehicle);
                        if(matchingCritera & alreadyExists)
                        {
                            toBeReturned.Add(currentVehicle);
                        };
                    };
                }
                else
                {
                    List<int> placeHolderToDelete = new List<int> { };
                    int iterator = 0;
                    foreach (Vehicle currentVehicle in exisitngResults)
                    {
                        vehicleInfor = currentVehicle.GetAttributList(true);
                        matchingCritera = vehicleInfor.Contains(firstValue.TERM());
                        if(!matchingCritera)
                        {
                            placeHolderToDelete.Add(iterator);
                        };
                        iterator++;
                    };
                    for(int position = 0; position < placeHolderToDelete.Count; position++)
                    {
                        int positionDelet = placeHolderToDelete[position];
                        exisitngResults.RemoveAt(positionDelet);
                        for(int i = 0; i < placeHolderToDelete.Count; i++)
                        {
                            placeHolderToDelete[i] = placeHolderToDelete[i]-1;
                        };
                    };
                    toBeReturned = exisitngResults;
                }
            };

            return toBeReturned;
        }

        //Returns true or false if the customer/car exists in the database
        public bool checkIfRenting(bool custExts, string ID, bool cust = true)
        {
            bool exits = false;
            if (cust)
            {
                if (custExts)
                {
                    int custID = Int32.Parse(ID);
                    exits = rentals.ContainsValue(custID);
                }
            }
            else
            {
                exits = rentals.ContainsKey(ID);
            };
            return exits;
        }

        //Adds vehicle if it does not exists
        //Returns true if succesful
        //Returns false if already exists
        public bool AddVehicle(Vehicle newVehicle)
        {
            bool canBeAdded = true;
            foreach (Vehicle oldVehicle in vehiclesDatabase)
            {
                canBeAdded = !(oldVehicle.GetRego() == newVehicle.GetRego());
                if (!canBeAdded)
                {
                    break;
                }
            }
            if (canBeAdded)
            {
                vehiclesDatabase.Add(newVehicle);
            };
            return canBeAdded;
        }

        //Retrives one Vehicle Based on the index location in the list
        public Vehicle GetVehicle(int option)
        {
            return vehiclesDatabase[option];
        }


        //Returns the amount of vheicles in database
        public int VehicleCurrentAmount()
        {
            return vehiclesDatabase.Count;
        }

        //Returns the amount of memory slots have been reserved for the list
        public int VehcileCapacity()
        {
            return vehiclesDatabase.Capacity;
        }

        //Returns the whole list of Vehicles in the Database
        public List<Vehicle> GetVehicles()
        {
            return vehiclesDatabase;
        }


        //Deletes the vehicle from the database that matches the vheicle passed
        public void DELETEVehicle(Vehicle vehicleDELETE)
        {
            vehiclesDatabase.Remove(vehicleDELETE);
        }

        //This take a file name and directory and saves the current vehicle database to it
        public void SavVEHICLEDatabase(string FileName, string desiredDIR)
        {
            //Creates final save location/filename
            string finalFileName = desiredDIR + "\\" + FileName;

            //Variables for use when writing to file
            const string DELIM = ",";
            const string firstLine = "Registration" + DELIM + "Grade" + DELIM + "Make" + DELIM + "Model" + DELIM + "Year" + DELIM
                                    + "NumSeats" + DELIM + "Transmission" + DELIM + "Fuel" + DELIM + "GPS" + DELIM + "SunRoof" + DELIM
                                    + "DailyRate" + DELIM + "Colour";
            //Opens stream
            FileStream RawFile = new FileStream(finalFileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(RawFile);
            //Writes the first line
            writer.WriteLine(firstLine);
            foreach (Vehicle toBeWriten in vehiclesDatabase)
            {
                writer.WriteLine(toBeWriten.ToCSVString());
            };
            //Closes stream
            writer.Close();
            RawFile.Close();
        }

        //DELETE previous database and loads the file specified
        public void LoadVEHICLEDatabase(string File)
        {
            //For ease of creating vehicle
            bool skipFirstLine = true;
            string fleetStringRAW;
            string[] fleetString;
            const string DELIM = ",";
           

            //Opens Stream
            FileStream fleetRAWFile = new FileStream(File, FileMode.Open, FileAccess.Read);
            StreamReader fleetReader = new StreamReader(fleetRAWFile);
            vehiclesDatabase.Clear();
            fleetStringRAW = fleetReader.ReadLine();
            while (fleetStringRAW != null)
            {
                //Skips first line once that has been done adds vehicle to the database
                if (!skipFirstLine)
                {
                    fleetString = fleetStringRAW.Split(DELIM);
                    Vehicle newVehicle = new Vehicle(fleetString[0],
                                                    fleetString[1],
                                                    fleetString[2],
                                                    fleetString[3],
                                                    Convert.ToInt32(fleetString[4]),
                                                    Convert.ToInt32(fleetString[5]),
                                                    fleetString[6], fleetString[7],
                                                    Convert.ToBoolean(fleetString[8]),
                                                    Convert.ToBoolean(fleetString[9]),
                                                    Convert.ToDouble(fleetString[10]),
                                                    fleetString[11]);
                    vehiclesDatabase.Add(newVehicle);
                }
                else
                {
                    skipFirstLine = false;
                };
                fleetStringRAW = fleetReader.ReadLine();
            };
        }

        //Takes file name and directory Name and saves it in the appororate location
        public void saveFleetDatabase(string fileName, string directory)
        {
            string headers = "Registration,CustomerID";
            string finalSave = directory + "\\" + fileName;
            string DELIM = ",";
            string linetoWrite;

            FileStream fleetRawFile = new FileStream(finalSave, FileMode.Create, FileAccess.Write);
            StreamWriter filePen = new StreamWriter(fleetRawFile);

            filePen.WriteLine(headers);
            
            foreach(KeyValuePair<string,int> lineToConvert in rentals)
            {
                linetoWrite = lineToConvert.Key + DELIM + Convert.ToString(lineToConvert.Value);
                filePen.WriteLine(linetoWrite);
            }

            filePen.Close();
            fleetRawFile.Close();
        }

        //Loads the fleet database at the specified directory
        public void loadRENTALDatabase(string filename)
        {
            bool skipFirstLine = true;
            string rentalStringRaw;
            string[] rentalsInfo = new string[2];
            string DELIM = ",";
            rentals.Clear();

            FileStream rentalStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader lookingGlass = new StreamReader(rentalStream);

            rentalStringRaw = lookingGlass.ReadLine();
            while(rentalStringRaw != null)
            {
                if(skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    rentalsInfo = rentalStringRaw.Split(DELIM);
                    rentals.Add(rentalsInfo[0], Int32.Parse(rentalsInfo[1]));
                };
                rentalStringRaw = lookingGlass.ReadLine();
            };

            lookingGlass.Close();
            rentalStream.Close();
        }
        
        //Gets the database length
        public int RentingLength()
        {
            return rentals.Count;
        }

        //Commits the rent to the database
        //Takes the vehicle and customer input to add to dictionary
        public void COMMITRENT(Vehicle vehRENT, customer custRENT)
        {
            string REGO;
            int custID;

            REGO = vehRENT.GetRego();
            custID = custRENT.GetID();

            rentals.Add(REGO, custID);
        }

        //Returns the Dictonary for use to display the report
        public Dictionary<string, int> ReturnRentals()
        {
            return rentals;
        }

        //Retrive the vehicle based on the vehicle regestration
        //Passes a rego string
        //Returns a vehicle
        public Vehicle GetVehicleRego(string REGO)
        {
            Vehicle toBeReturned = null;
            foreach(Vehicle tobeChecked in vehiclesDatabase)
            {
                if(tobeChecked.GetRego() == REGO)
                {
                    toBeReturned = tobeChecked;
                    break;
                }
            }
            return toBeReturned;
        }

        //Deletes the rental from the database
        public void DELETERental(KeyValuePair<string,int> ToBeDeleted)
        {
            string regestration = ToBeDeleted.Key;

            rentals.Remove(regestration);
        }
    }
}
