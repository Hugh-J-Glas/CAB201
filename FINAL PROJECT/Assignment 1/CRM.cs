using System;
using System.IO;
using System.Collections.Generic;

namespace MRRC
{
    //This class handles all the customer information in a 'database' structure
    public class CRM
    {
        //The database in which all the customer information is stored
        private List<customer> CustomerDataBase = new List<customer> { };
        //Public variable that could occur
        public bool fileOPEN = false;

        //This creates the database to store customers
        //If the file location doesn't have anything it will produce a simple error message
        //If it succeds it adds the customer information to the database
        public CRM(string crmFile)
        {
            fileOPEN = false;
            try
            {
                //Declare Variables for use throughout construction
                string CustomerINFO;
                string[] RawINFO;
                const char DELIM = ',';

                //Checks if the the file exists within the provided destination
                if (File.Exists(crmFile))
                {
                    //This sets up a skip for the first line of 'useless information
                    bool skipFirstLine = false;

                    //Opens the file so that the program can readinformation from it
                    FileStream CRMRAWFile = new FileStream(crmFile, FileMode.Open, FileAccess.Read);
                    StreamReader CRMReader = new StreamReader(CRMRAWFile);
                    CustomerINFO = CRMReader.ReadLine();
                    //Goes through file until no values or null are read from it
                    while (CustomerINFO != null)
                    {
                        //Splits information into an array
                        RawINFO = CustomerINFO.Split(DELIM);
                        if (skipFirstLine)
                        {
                            int FILEID = Convert.ToInt32(RawINFO[0]);
                            bool changeID = CHECKID(FILEID);
                            if (changeID)
                            {
                                FILEID = CustomerDataBase.Count + 1;
                            };
                            //Adds information from file into a 'customer' object
                            customer NewCustomer = new customer(FILEID, RawINFO[1], RawINFO[2], RawINFO[3], RawINFO[4], RawINFO[5]);
                            //Adds the new customer to database
                            CustomerDataBase.Add(NewCustomer);
                        }
                        else
                        {
                            skipFirstLine = true;
                        };
                        CustomerINFO = CRMReader.ReadLine();
                    };
                    CRMRAWFile.Close();
                    CRMReader.Close();
                };
            }
            catch(IOException)
            {
                fileOPEN = true;
            };
        }

        //This method checks if the customer existis in the current database
        //If it doesn't it will be added and return true
        //While if it does it will not be added and return false
        public bool AddCustomer(customer INFO)
        {
            //Initialises return value of 
            bool succesful = true;
            //If the customer ID is the same and it is not a previous customer 
            //fix this by appending to the end fo the list
            int currentLength = CustomerDataBase.Count + 1;
            bool fixID = false;
            string[] newCUSTName = INFO.GetName();
            string[] previousCUSTName;
            foreach (customer presentCustomer in CustomerDataBase)
            {

                //It will be considered the same customer if the First, Last Name and DOB are equvilent
                previousCUSTName = presentCustomer.GetName();


                bool decision11 = String.Equals(newCUSTName[0], previousCUSTName[0]);
                bool decision12 = String.Equals(newCUSTName[1], previousCUSTName[1]);
                bool decision1 = decision11 & decision12;

                bool decision2 = presentCustomer.GetDOB() == INFO.GetDOB();
                if (!fixID)
                {
                    fixID = presentCustomer.GetID() == INFO.GetID();
                };

                //If First Last name and DOB are equivlient then it is considered a duplicate entry
                if (decision1 & decision2)
                {
                    succesful = false;
                };
            }
            //If there was no duplicate add it to the main database
            if (succesful)
            {
                if (fixID)
                {
                    INFO.SetID(currentLength - 1);
                }
                CustomerDataBase.Add(INFO);
            };

            return succesful;
        }

        //The checks if the customer can be removed is done wtihin the fleet class
        //this will just remove
        public bool RemoveCustomer(customer IDtoDELETE)
        {
            CustomerDataBase.Remove(IDtoDELETE);
            return true;
        }

        //Reutnrs the whole customer database
        public List<customer> GetCustomers()
        {
            return CustomerDataBase;
        }


        //Checks if there is a double up 
        private bool CHECKID(int potentialID)
        {
            bool changeID = false;
            foreach (customer customerCheck in CustomerDataBase)
            {
                if (customerCheck.GetID() == potentialID)
                {
                    changeID = true;
                    break;
                }
            }
            return changeID;
        }

        //Returns the number of entries in the Customer Database currently
        public int numOfEntrys()
        {
            return CustomerDataBase.Count;
        }

        //Retrives the customer of the specified ID value
        public customer GetCustomer(int IDfind)
        {
            //Initilise iterable variables and also bounds
            int position = 0;

            //Iterators through list if there is a double up then 
            foreach (customer customerCheck in CustomerDataBase)
            {
                int ExistingCustomer = customerCheck.GetID();
                if (ExistingCustomer == IDfind)
                {
                    break;
                }
                position++;
            }
            if (position == CustomerDataBase.Count)
            {
                return null;
            }
            else
            {
                return CustomerDataBase[position];
            };
        }

        //This method upodates the information of a customer when requested
        //This takes a string array to either create or update a customer
        //The customer ID determines if this will update or add
        //Returns True or false depending how the operation went.
        public bool ModifyCustomer(string[] newUpdatedCustomer, int exisitingID = -1)
        {
            //Initialise variables
            bool succesful = false;

            //This occurs if this is a new customer that needs created
            if (exisitingID == -1)
            {
                customer updatedNewCustomer = new customer(1, newUpdatedCustomer[0], newUpdatedCustomer[1], newUpdatedCustomer[2], newUpdatedCustomer[3], newUpdatedCustomer[4]);
                succesful = AddCustomer(updatedNewCustomer);
            }
            //Updates exisiting customer
            else
            {
                bool goAhead = true;
                foreach (customer check in CustomerDataBase)
                {
                    string[] FirstLast = new string[] { newUpdatedCustomer[1], newUpdatedCustomer[2] };
                    string[] OriginalFirstLast = check.GetName();
                    bool decision11 = string.Equals(OriginalFirstLast[0], FirstLast[0]);
                    bool decision12 = string.Equals(OriginalFirstLast[1], FirstLast[1]);
                    bool decision1 = decision11 & decision12;
                    bool doubleCheck = exisitingID != check.GetID();
                    int counter = 0;

                    bool decision2 = check.GetDOB() == Convert.ToDateTime(newUpdatedCustomer[4]);
                    if (decision1 & decision2 & doubleCheck)
                    {
                        counter++;
                    };
                    if (counter > 0)
                    {
                        goAhead = false;
                        succesful = false;
                    };
                };
                if (goAhead)
                {
                    succesful = CustomerDataBase[exisitingID].UpdateCustomer(newUpdatedCustomer);
                };
            };
            return succesful;
        }

        //Returns the Capacity or the amount of memory slots of the list of customers
        public int CustDatabaseCap()
        {
            int length = CustomerDataBase.Capacity;
            return length;
        }

        //Saves the CRM to a file
        public void SaveToFile(string fileName, string directory)
        {
            string finalFileName = directory + "\\" + fileName;

            //If file already exists delete oriignal
            if (File.Exists(finalFileName))
            {
                File.Delete(finalFileName);
            }
            //Create new file

            //Variables for use when writing to file
            const string DELIM = ",";
            const string firstLine = "ID" + DELIM + "Title" + DELIM + "FirstName" + DELIM + "LastName" + DELIM + "Gender" + DELIM + "DOB";
            //Opens stream
            FileStream RawFile = new FileStream(finalFileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(RawFile);
            //Writes the first line
            writer.WriteLine(firstLine);
            foreach (customer toBeWriten in CustomerDataBase)
            {
                writer.WriteLine(toBeWriten.ToCSVString());
            };
            //Closes stream
            writer.Close();
            RawFile.Close();
        }

        //Adds more from file
        public void LoadFromFile(string filePath)
        {
            string CustomerINFO;
            string[] RawINFO;
            const char DELIM = ',';

            if (CustomerDataBase.Count != 0)
            {
                CustomerDataBase.Clear();
            }

            //This sets up a skip for the first line of 'useless information
            bool skipFirstLine = false;

            //Opens the file so that the program can readinformation from it
            FileStream CRMRAWFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader CRMReader = new StreamReader(CRMRAWFile);
            CustomerINFO = CRMReader.ReadLine();
            //Goes through file until no values or null are read from it
            while (CustomerINFO != null)
            {
                //Splits information into an array
                RawINFO = CustomerINFO.Split(DELIM);
                if (skipFirstLine)
                {
                    int FILEID = Convert.ToInt32(RawINFO[0]);
                    bool changeID = CHECKID(FILEID);
                    if (changeID)
                    {
                        FILEID = CustomerDataBase.Count + 1;
                    };
                    //Adds information from file into a 'customer' object
                    customer NewCustomer = new customer(FILEID, RawINFO[1], RawINFO[2], RawINFO[3], RawINFO[4], RawINFO[5]);
                    //Adds the new customer to database
                    CustomerDataBase.Add(NewCustomer);
                }
                else
                {
                    skipFirstLine = true;
                };
                CustomerINFO = CRMReader.ReadLine();
            };
            CRMRAWFile.Close();
            CRMReader.Close();
        }

    }
}