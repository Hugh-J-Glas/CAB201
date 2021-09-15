using System;
using System.IO;
using static System.Console;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace MRRC
{
    class display
    {
        //Constant Variables for spacing and sizing of the display screen
        private const string symbol = "*";
        private const int borderSize = 120;
        private const int screenSize = 20;

        //Used to help determine if input at time is correct
        public enum Location
        {
            ScreenMain,
            CustomerMain,
            VehicleMain,
            SaveLoadData,
            RentalMangament
        }
        //Used to make error messaging easier with integers tied to type
        enum ErrorType
        {
            NoError,
            IncorrectInput,
            InvalidInput,
            NotPartOfGender,
            NotProperTitle,
            DateFormateError,
            RegoIncorrect
        }

        //Inputs that are correct in the rental string
        enum ValidInputsRenting
        {
            Back = -1,
            New = 1,
            Delete,
            Current
        }

        //Inputs that are correct at this point
        enum ValidInputsMain
        {
            QUIT = -1,
            Customer = 1,
            Vechicle,
            NEW,
            DOC

        }
        //Inputs that are correct within customers
        enum ValidInputsCustMain
        {
            Back = -1,
            Add = 1,
            Modify,
            Search,
            Delete
        }

        //Valid Inputs for the customer Modify screen
        enum ValidInputsCustModify
        {
            Back = -1,
            Title = 1,
            FName,
            LName,
            Gender,
            DOB
        }

        //Valid INputs for the Vehicle Main screen
        enum ValidInputsVehicleMain
        {
            Back = -1,
            Add = 1,
            Delete,
            List,
            Search
        }

        //Valid inputs for the save load screen
        enum ValidInputsSaveLoad
        {
            Back = -1,
            SaveCust = 1,
            LoadCust,
            SaveVeh,
            LoadVeh,
            SaveFleet,
            LoadFleet
        }

        //Valid Inputs for the vehicle adjustment
        enum ValidInputsAdjustVeh
        {
            Back = -1,
            REGO = 1,
            GRADE,
            MAKE,
            MODEL,
            YEAR,
            SEATS,
            TRANSMISSION,
            FUELTYPE,
            GPS,
            SUNROOF,
            RATE,
            COLOUR
        }

        //Valid inputs for the lists
        enum ValidInputsLists
        {
            Back = -1,
            PreviousPage = 1,
            NextPage
        }

        //Copies of private Enumerations in the customers class to check if inputs are correct
        enum Title
        {
            Mr,
            Mrs,
            Ms,
            Miss,
            Dr,
            Lord,
            Lady,
            Mx
        }

        enum Gender
        {
            Male,
            Female,
            Other
        }

        //This method takes a string and formates it to the center of the screen and provides symbols around it
        public static void Heading(string TITLE)
        {
            string toDisplay = "";
            int titleLength = TITLE.Length;
            decimal spacing = borderSize - titleLength;

            for (int iterator = 0; iterator < borderSize; iterator++)
            {
                if (iterator == decimal.Floor(spacing / 2))
                {
                    toDisplay += " ";
                    toDisplay += TITLE + " ";
                    iterator += titleLength;
                    iterator++;
                }
                else
                {
                    toDisplay += symbol;
                }
            }
            Border();
            WriteLine(toDisplay);
            Border();
        }

        //Creats the solid line of symbols 
        public static void Border()
        {
            string topBottomBorder = "";
            for (int iterator = 0; iterator < borderSize; iterator++)
            {
                topBottomBorder += symbol;
            };
            WriteLine(topBottomBorder);
        }

        //A formating class to produce the same amount of spacing 
        //The string input is the phrase to be written
        //The symbol change is used mainly for errors chanign the edge from * to !
        //the boolean is for the vehicle list page to allow for more space
        public static void BasicBody(string phrase = "", string symbolChange = symbol, bool tabbing = true)
        {
            string toDispaly = "";
            int numOfTabs = 2;
            toDispaly += symbolChange;
            int reminder;
            if (tabbing)
            {
                for (int tabIterator = 0; tabIterator < numOfTabs; tabIterator++)
                {
                    toDispaly += "\t";
                };
            }
            toDispaly += phrase;
            if (tabbing)
            {
                reminder = borderSize - toDispaly.Replace("\t", "       ").Length - 2;
            }
            else
            {
                reminder = borderSize - toDispaly.Length - 1;
            }
            for (int iterator = 0; iterator < reminder; iterator++)
            {
                toDispaly += " ";
            };
            toDispaly += symbolChange;
            WriteLine(toDispaly);
        }

        //Back fills the bottom of the screen with empty lines
        public static void bottomScreenBackfill(int lineNumber)
        {
            for (int iterator = lineNumber; iterator <= screenSize; iterator++)
            {
                if (iterator == screenSize)
                {
                    Border();
                }
                else
                {
                    BasicBody();
                };
            };
        }

        //Produces a message based on the type of error
        public static void ErrorMessage(int type)
        {
            string message = "";
            switch (type)
            {
                case ((int)ErrorType.InvalidInput):
                    message = "!!! The input you have entered is not recongnised please try again !!!";
                    break;
                case ((int)ErrorType.IncorrectInput):
                    message = "!!! Please Enter a key that is in display !!!";
                    break;
                case ((int)ErrorType.NotPartOfGender):
                    message = "!!! Please enter Male/Female/Other !!!";
                    break;
                case ((int)ErrorType.NotProperTitle):
                    message = "!!! Enter a valid title !!!";
                    break;
                case ((int)ErrorType.DateFormateError):
                    message = "!!! Enter Date as DD/MM/YYYY !!!";
                    break;
                case ((int)ErrorType.RegoIncorrect):
                    message = "!!! Regestration needs to be 3 numbers then 3 letters !!!";
                    break;
            }
            BasicBody(message, "!");
        }

        //Takes a Enum location area and checks if inputs are valid
        //Returns an array the first beinga a true/false 1/0 for an error
        //The secound integers can be throught of as an error code
        public static int[] UserPrompt(Location currentLocation)
        {
            int unsuccesfulescape = 0;
            int goBackAPage = -1;
            int[] response = new int[2];
            Write(">>>");
            string currentInput = ReadLine();
            currentInput = currentInput.ToLower();
            if (currentInput == "b" | currentInput == "q")
            {
                response = new int[] { (int)ErrorType.NoError, goBackAPage };
                return response;
            }
            else
            {
                bool firstCheck = int.TryParse(currentInput, out int userInputINT);
                if (firstCheck)
                {
                    bool secoundCheck;
                    switch (currentLocation)
                    {
                        case Location.ScreenMain:
                            secoundCheck = Enum.IsDefined(typeof(ValidInputsMain), userInputINT);
                            if (secoundCheck)
                            {
                                response = new int[] { (int)ErrorType.NoError, userInputINT };
                                return response;
                            }
                            else
                            {
                                response = new int[] { (int)ErrorType.IncorrectInput, unsuccesfulescape };
                                return response;
                            };
                        case Location.CustomerMain:
                            secoundCheck = Enum.IsDefined(typeof(ValidInputsCustMain), userInputINT);
                            if (secoundCheck)
                            {
                                response = new int[] { (int)ErrorType.NoError, userInputINT };
                                return response;
                            }
                            else
                            {
                                response = new int[] { (int)ErrorType.IncorrectInput, unsuccesfulescape };
                                return response;
                            }
                        case Location.VehicleMain:
                            secoundCheck = Enum.IsDefined(typeof(ValidInputsVehicleMain), userInputINT);
                            if (secoundCheck)
                            {
                                response = new int[] { (int)ErrorType.NoError, userInputINT };
                                return response;
                            }
                            else
                            {
                                response = new int[] { (int)ErrorType.IncorrectInput, unsuccesfulescape };
                                return response;
                            }
                        case Location.SaveLoadData:
                            secoundCheck = Enum.IsDefined(typeof(ValidInputsSaveLoad), userInputINT);
                            if (secoundCheck)
                            {
                                response = new int[] { (int)ErrorType.NoError, userInputINT };
                                return response;
                            }
                            else
                            {
                                response = new int[] { (int)ErrorType.NoError, unsuccesfulescape };
                                return response;
                            }
                        case Location.RentalMangament:
                            secoundCheck = Enum.IsDefined(typeof(ValidInputsRenting), userInputINT);
                            if (secoundCheck)
                            {
                                response = new int[] { (int)ErrorType.NoError, userInputINT };
                                return response;
                            }
                            else
                            {
                                response = new int[] { (int)ErrorType.IncorrectInput, unsuccesfulescape };
                                return response;
                            };
                        default:
                            response = new int[] { (int)ErrorType.IncorrectInput, unsuccesfulescape };
                            return response;
                    }
                }
                else
                {
                    response = new int[] { (int)ErrorType.InvalidInput, unsuccesfulescape };
                    return response;
                };
            }
        }


        //This is the main customer mangament screen
        public static void CustomerManagmentInitialScreen()
        {
            int[] error = new int[2] { 0, 0 };
            bool inputs = true;
            //General Dsiplay
            while (inputs)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("MRRC - Customers");
                BasicBody();
                BasicBody();
                BasicBody("1) New Customer");
                BasicBody("2) Modify Customer");
                BasicBody("3) Search Customers");
                BasicBody("4) Delete Customers");
                BasicBody();
                BasicBody("B) Back to Main Menue");
                if (error[0] == 0)
                {
                    bottomScreenBackfill(9);
                }
                else
                {
                    BasicBody();
                    BasicBody();
                    ErrorMessage(error[0]);
                    bottomScreenBackfill(12);
                };
                error = UserPrompt(Location.CustomerMain);
                //If there is no errors made with the inputs then the program changes screen
                if (error[0] == 0)
                {
                    //The secound integer is where the program is heading next
                    switch (error[1])
                    {
                        case (int)ValidInputsCustMain.Back:
                            inputs = false;
                            MainScreen();
                            break;
                        case (int)ValidInputsCustMain.Add:
                            inputs = false;
                            CustomerAddScreen();
                            break;
                        case (int)ValidInputsCustMain.Modify:
                        case (int)ValidInputsCustMain.Search:
                            inputs = false;
                            CustomerModifyScreen();
                            break;
                        case (int)ValidInputsCustMain.Delete:
                            inputs = false;
                            deleteScreen(true);
                            break;
                    }
                }
            }
        }

        //The idea of this function is that is to be used recursivle so that it can passed the ID field to delete
        public static void deleteScreen(bool custList, string selection = "-1")
        {
            //Initilising variables for use throughout
            bool canDELETE = false;
            bool succesfulDelete = false;
            bool inputting = true;
            bool error = false;
            string userInput;

            //This is if a customer is to be deleted
            if (custList)
            {
                if (selection == "-1")
                {
                    ListofInformation(custList, true);
                }
                else
                {
                    canDELETE = mainProgram.fleetMain.checkIfRenting(custList, selection);
                    if (!canDELETE)
                    {
                        //Retrives information so that the user can confirm this is the correct customer to delete
                        customer custTODELETE = mainProgram.CRMMain.GetCustomer(Int32.Parse(selection));
                        string[] info = custTODELETE.GetEverything();
                        while (inputting)
                        {
                            if (mainProgram.NOTRecording)
                            {
                                Clear();
                            };
                            Heading("DELETE CUSTOMER CONFIRMATION");
                            BasicBody();
                            BasicBody();
                            BasicBody("1) Title:         " + info[0]);
                            BasicBody();
                            BasicBody("2) First Name:    " + info[1]);
                            BasicBody();
                            BasicBody("3) Last Name:     " + info[2]);
                            BasicBody();
                            BasicBody("4) Gender:        " + info[3]);
                            BasicBody();
                            BasicBody("5) Date of Birth: " + info[4]);
                            BasicBody();
                            BasicBody("This is the customer you wish to delete", "!");
                            BasicBody("Would you like to proceed (Warning this cannot be undone)", "!");
                            BasicBody("(Y) Yes / (N) No");
                            if (error)
                            {
                                BasicBody("That is not a valid input", "!");
                                bottomScreenBackfill(15);
                                error = false;
                            }
                            else
                            {
                                bottomScreenBackfill(16);
                            };
                            Write(">>>");
                            userInput = ReadLine();
                            switch (userInput.ToLower())
                            {
                                case "y":
                                    succesfulDelete = mainProgram.CRMMain.RemoveCustomer(custTODELETE);
                                    inputting = false;
                                    break;
                                case "n":
                                    inputting = false;
                                    CustomerManagmentInitialScreen();
                                    break;
                                default:
                                    error = true;
                                    break;
                            };
                        }
                    }
                    //Message upon reaching an error currently renting
                    else
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("ERROR WITH DELETING");
                        BasicBody();
                        BasicBody();
                        BasicBody("That customer is currently renting", "!");
                        BasicBody("They cannot be deleted unless you remove they're renting", "!");
                        bottomScreenBackfill(8);
                        Write(">>>");
                        ReadLine();
                    };
                };
                //This is if the customer/Vehicle can be deleted
                if (!canDELETE)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("SUCCESFUL DELETE");
                    BasicBody();
                    BasicBody();
                    BasicBody("The customer has been deleted");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    CustomerManagmentInitialScreen();
                }
                else
                {
                    CustomerManagmentInitialScreen();
                };
            }
            //When a vehicle is being deleted
            else
            {
                //Variables for use throughout 
                int vehicleID = Convert.ToInt32(selection) - 1;
                Vehicle vehicleToDelete = mainProgram.fleetMain.GetVehicle(vehicleID);
                List<string> VehicleINFO = vehicleToDelete.GetAttributList();
                string stringtoDisplay;
                string INPUTuser;
                string[] placeHolder = new string[12] { "Regestration:", "Grade:", "Make:", "Model:", "Year:", "Seats:", "Transmission:", "Fuel:", "GPS:", "Sunroof:", "Rate:", "Colour:" };
                int maxrecords = 5;
                bool currentlyRenting = false;
                while (inputting)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Confirmation of Delete - Vehicle");
                    BasicBody();
                    //Simply Display of information before final confirmation
                    for (int iterator = 0; iterator < maxrecords; iterator++)
                    {
                        stringtoDisplay = string.Format("{0,-2}){4,-15}{1,-15}{2,-2}){5,-15}{3,-15}",
                                                       iterator + 1, VehicleINFO[iterator], iterator + 6, VehicleINFO[iterator + 7], placeHolder[iterator], placeHolder[iterator + 7]);
                        BasicBody();
                        BasicBody(stringtoDisplay);
                    };
                    BasicBody();
                    BasicBody("This is the vehicle selected to be deleted");
                    BasicBody("Once deleted this cannot be reverted", "!");
                    BasicBody("Confirm if you would like delete (Y)Yes or (N)No or go (B)Back", "!");
                    BasicBody();
                    if (error)
                    {
                        if (currentlyRenting)
                        {
                            BasicBody("The current Vehicle cannot be deleted as it is currently being rented please change this to delete", "!");
                        }
                        else
                        {
                            BasicBody("Sorry that is not a valid input", "!");
                        };
                        error = false;
                    }
                    else
                    {
                        BasicBody();
                    };
                    bottomScreenBackfill(16);
                    Write(">>>");
                    INPUTuser = ReadLine();
                    switch (INPUTuser.ToLower())
                    {
                        case "y":
                            currentlyRenting = mainProgram.fleetMain.checkIfRenting(true, VehicleINFO[0], false);
                            if (currentlyRenting)
                            {
                                error = true;
                            }
                            else
                            {
                                mainProgram.fleetMain.DELETEVehicle(vehicleToDelete);
                                inputting = false;
                            };
                            break;
                        case "n":
                        case "b":
                            inputting = false;
                            currentlyRenting = true;
                            break;
                        default:
                            error = true;
                            break;
                    };
                };
                if (!currentlyRenting)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("VEHICLE DELTED SUCCESFULLY");
                    BasicBody();
                    BasicBody("Congratulations the vehicle has been deleted");
                    bottomScreenBackfill(5);
                    Write(">>>");
                    ReadLine();
                };
                VehicleManagmentInitialScreen();
            };
        }


        //This is a customer modify screen if the customer object is passed it will change the functionality
        //If an array is passed from the customer Add screen it will be treated as a new customer
        public static void CustomerModifyScreen(customer fixCustomer = null, string[] fixCustomerRaw = null)
        {
            //Initilise variables for modifying
            bool inputs = true;
            bool changes = false;
            bool existingCust = fixCustomer == null;
            bool needTofindCust = existingCust & fixCustomerRaw == null;

            //This will bring up a list of customers to find the customer to add it
            if (needTofindCust)
            {
                ListofInformation(true);
            }
            //This will bring up the customer information to be editied
            else
            {
                //Initialising Values for later editing
                string[] customerINFO;
                bool saving = false;
                bool editing = false;
                bool error = false;
                bool correctinput = true;
                bool doubleup = false;
                int editingField = 0;
                string userInput;

                if (fixCustomer != null)
                {
                    customerINFO = fixCustomer.GetEverything();
                }
                else
                {
                    customerINFO = fixCustomerRaw;
                }
                //While the edits are occuring the program goes into a loop
                while (inputs)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("CUSTOMER MODIFY");
                    BasicBody();
                    BasicBody();
                    BasicBody("1) Title:         " + customerINFO[0]);
                    BasicBody();
                    BasicBody("2) First Name:    " + customerINFO[1]);
                    BasicBody();
                    BasicBody("3) Last Name:     " + customerINFO[2]);
                    BasicBody();
                    BasicBody("4) Gender:        " + customerINFO[3]);
                    BasicBody();
                    BasicBody("5) Date of Birth: " + customerINFO[4]);
                    BasicBody();
                    //Depending on inputs the display will be altered (this is the confirmation page for saving details)
                    if (saving & !editing)
                    {
                        if (error)
                        {
                            BasicBody("THat was not a valid input", "!");
                        }
                        else if (doubleup)
                        {
                            BasicBody("That customer exists with the Name and Date of Birth", "!");
                        }
                        else
                        {
                            BasicBody();
                        };
                        BasicBody("Is this information is correct? (Y)Yes/(N)No");
                        bottomScreenBackfill(17);
                        Write(">>>");
                        userInput = ReadLine().ToLower();
                        error = false;
                        switch (userInput)
                        {
                            case "y":
                                if (!existingCust)
                                {
                                    if (changes)
                                    {
                                        //This is if there is an existing customer passing the ID
                                        doubleup = !mainProgram.CRMMain.ModifyCustomer(customerINFO, fixCustomer.GetID());
                                    }
                                    else
                                    {
                                        doubleup = false;
                                    }
                                }
                                else
                                {
                                    if (changes)
                                    {
                                        //This is if there is a new customer being added
                                        doubleup = !mainProgram.CRMMain.ModifyCustomer(customerINFO);
                                    }
                                    else
                                    {
                                        doubleup = false;
                                    };
                                };
                                if (!doubleup)
                                {
                                    //If no error occurs the loop is broken
                                    inputs = false;
                                }
                                break;
                            case "n":
                                //Goes back to the editing screen
                                saving = false;
                                break;
                            default:
                                //If the input is invalid
                                error = true;
                                break;
                        };
                    }
                    //The editing screen of each piece of information
                    else if (!saving & editing)
                    {
                        BasicBody();
                        BasicBody();
                        //If error occurs then the error messssage is supplied
                        if (!correctinput)
                        {
                            BasicBody();
                            BasicBody("That value is not correct for the entered type", "!");
                        };
                        //Brings up previous information to be edited
                        switch (editingField)
                        {
                            case (int)ValidInputsCustModify.Title:
                                BasicBody("Editing Title");
                                BasicBody("Previous Title:" + customerINFO[0]);
                                break;
                            case (int)ValidInputsCustModify.FName:
                                BasicBody("Editing First Name");
                                BasicBody("Previous First Name " + customerINFO[1]);
                                break;
                            case (int)ValidInputsCustModify.LName:
                                BasicBody("Editing Last Name");
                                BasicBody("Previous Last Name: " + customerINFO[2]);
                                break;
                            case (int)ValidInputsCustModify.DOB:
                                BasicBody("Editing Date of Birth");
                                BasicBody("Previous Date of Birth: " + customerINFO[4]);
                                break;
                            case (int)ValidInputsCustModify.Gender:
                                BasicBody("Editing Gender");
                                BasicBody("Previous Gender: " + customerINFO[3]);
                                break;
                        };
                        Border();
                        Write(">>>");
                        userInput = ReadLine();
                        //Checks if the information provided matches the data type
                        if (userInput.ToLower() == "b")
                        {
                            editing = false;
                        }
                        else
                        {
                            correctinput = INFOCheck(userInput, editingField - 1);
                            if (correctinput)
                            {
                                //Overwrites the current piece of data for the new one
                                customerINFO[editingField - 1] = userInput;
                                changes = true;
                                editing = false;
                            };
                        }
                    }
                    //Default screen of modifying customer
                    else
                    {
                        if (!error)
                        {
                            BasicBody();
                        }
                        else if (saving & error)
                        {
                            BasicBody("A customer with the same DOB, First Name and Last Name exists", "!");
                            error = false;
                            saving = false;
                        }
                        else
                        {
                            BasicBody("That was not a valid input", "!");
                            error = false;
                        }
                        BasicBody("Select the number of the detail you wish to edit");
                        BasicBody("Quit this menu by entering 'B' Warning! all progress will be lost");
                        BasicBody("When ready to save the modification type 'S'");
                        bottomScreenBackfill(19);
                        Write(">>>");
                        userInput = ReadLine().ToLower();
                        switch (userInput)
                        {
                            case "1":
                                editing = true;
                                editingField = (int)ValidInputsCustModify.Title;
                                break;
                            case "2":
                                editing = true;
                                editingField = (int)ValidInputsCustModify.FName;
                                break;
                            case "3":
                                editing = true;
                                editingField = (int)ValidInputsCustModify.LName;
                                break;
                            case "4":
                                editing = true;
                                editingField = (int)ValidInputsCustModify.Gender;
                                break;
                            case "5":
                                editing = true;
                                editingField = (int)ValidInputsCustModify.DOB;
                                break;
                            case "s":
                                saving = true;
                                break;
                            case "b":
                                inputs = false;
                                break;
                            default:
                                error = true;
                                break;
                        };
                    };
                };
                if (saving)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Customer Modify");
                    BasicBody();
                    BasicBody();
                    BasicBody("The customer has succesfully been modified");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                };
                CustomerManagmentInitialScreen();
            };
        }

        //Method to check the inputs of within the List screen and returns if there was an error or not
        public static bool ListInputChecks(string userInput)
        {
            //Initilise varible to output
            bool interger = Int32.TryParse(userInput, out int option);
            bool errorNeeded = true;

            if (!interger)
            {
                switch (userInput.ToLower())
                {
                    case "c":
                    case "b":
                    case "n":
                    case "p":
                        errorNeeded = false;
                        break;
                    default:
                        break;
                };
            }
            else
            {
                //Checks the interger is in range of the screen display
                errorNeeded = option <= 0 | option >= screenSize - 5;
            }

            return errorNeeded;
        }

        //This method produces a list of information depending on the bool pass
        //If it's true it produces customer
        //If it's false it produces the vehicles
        public static void ListofInformation(bool customerList, bool delete = false, bool renting = false)
        {
            if (customerList)
            {
                //Initilse the variables for use throughout the method
                int customerDatabaseID = mainProgram.CRMMain.CustDatabaseCap();
                int customerDatabaseLength = mainProgram.CRMMain.numOfEntrys();
                int missingData = customerDatabaseID - customerDatabaseLength;
                int maximuminfoCUST = screenSize - 5;
                int currentLoops = 0;
                decimal numOfPagesCust = decimal.Ceiling(Convert.ToDecimal(customerDatabaseLength) / Convert.ToDecimal(maximuminfoCUST));
                string toBePrinted;
                bool error = false;
                bool inputs = true;
                int option = 0;
                string[] custINFO;
                //If more pages are required then the formating will be different
                bool morepages = customerDatabaseLength > maximuminfoCUST;
                bool Empty = customerDatabaseLength == 0;
                if (!morepages & !Empty)
                {
                    while (inputs)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Select Customer");
                        BasicBody("      Title:       First:           Last:             DOB:");
                        int listIterator = 0;
                        foreach (customer indviudalCust in mainProgram.CRMMain.GetCustomers())
                        {
                            listIterator++;
                            //Retrives the current customer information and puts it in array
                            custINFO = indviudalCust.GetEverything();
                            //Fromates the string to align to the screen
                            toBePrinted = String.Format("{0,-3}){1,8}{2,15}{3,15}{4,20}", listIterator, custINFO[0], custINFO[1], custINFO[2], custINFO[4]);
                            BasicBody(toBePrinted);

                        };
                        BasicBody();
                        BasicBody("Input the number to modify the customer");
                        BasicBody("Enter B to back out of this screen to the previous screen");
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            listIterator += 6;
                        }
                        else
                        {
                            listIterator += 5;
                        };
                        bottomScreenBackfill(listIterator);
                        Write(">>>");
                        string userIn = ReadLine();
                        //Check inputs if it is valid in this area
                        error = ListInputChecks(userIn);
                        if (!error)
                        {
                            bool interger = Int32.TryParse(userIn, out option);
                            if (interger)
                            {
                                //Passes customer to modify screen
                                option--;
                                inputs = false;
                            }
                            else
                            {
                                switch (userIn.ToLower())
                                {
                                    case "b":
                                        inputs = false;
                                        CustomerManagmentInitialScreen();
                                        break;
                                };
                            }
                        };
                    };
                }
                else if (customerDatabaseLength == 0)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("List of Customers");
                    BasicBody();
                    BasicBody();
                    BasicBody("No Customers to Display please Load or Add new customers", "!");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    CustomerManagmentInitialScreen();
                }
                else
                {
                    while (inputs)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        //Multiple Pages to display
                        Heading("Select Customer");
                        string heading2 = string.Format("      Title:       First:           Last:             DOB:      Page: {0} of {1}", currentLoops + 1, numOfPagesCust);
                        BasicBody(heading2);
                        //Only displays the number of entrys of the maximum entrys specified
                        for (int iterator = 0; iterator < maximuminfoCUST + missingData; iterator++)
                        {
                            int currentRecord = iterator + maximuminfoCUST * currentLoops;
                            customer currentCUST = mainProgram.CRMMain.GetCustomer(currentRecord);
                            if (currentCUST != null)
                            {
                                custINFO = currentCUST.GetEverything();
                                toBePrinted = String.Format("{0,-3}){1,8}{2,15}{3,15}{4,20}", iterator + 1, custINFO[0], custINFO[1], custINFO[2], custINFO[4]);
                                BasicBody(toBePrinted);
                            }
                            if (currentRecord == customerDatabaseLength - 1)
                            {
                                for (int emptyspaces = iterator; emptyspaces < maximuminfoCUST; emptyspaces++)
                                {
                                    BasicBody();
                                };
                                iterator = maximuminfoCUST;
                            };
                        }
                        BasicBody();
                        BasicBody("Input the number to modify the customer");
                        BasicBody("Enter B to back out of this screen to the previous screen");
                        BasicBody("Enter N for the Next Page or P for the previous Page");
                        bottomScreenBackfill(20);
                        Write(">>>");
                        string userIn = ReadLine();
                        error = ListInputChecks(userIn);
                        if (!error)
                        {
                            bool interger = Int32.TryParse(userIn, out option);
                            if (interger)
                            {
                                option = (option - 1) + currentLoops * maximuminfoCUST;
                                inputs = false;
                            }
                            else
                            {
                                switch (userIn.ToLower())
                                {
                                    case "b":
                                        inputs = false;
                                        CustomerManagmentInitialScreen();
                                        break;
                                    case "n":
                                        currentLoops++;
                                        break;
                                    case "p":
                                        currentLoops--;
                                        break;
                                };
                            }
                        };
                    }
                };
                if (!delete)
                {
                    CustomerModifyScreen(mainProgram.CRMMain.GetCustomer(option));
                }
                else
                {
                    deleteScreen(true, Convert.ToString(option));
                };

            }
            //When lisitng vehicles
            else
            {
                //Decide weather or not more pages are needed
                int currentLength = mainProgram.fleetMain.VehicleCurrentAmount();
                int trueLength = mainProgram.fleetMain.VehcileCapacity();
                int maximumInfoVeh = screenSize - 5;
                int option;
                int iterator = 1;
                bool morepages = currentLength > maximumInfoVeh;
                bool inputting = true;
                bool error = false;
                bool empty = currentLength == 0;
                //If more pages arn't required
                if (!morepages & !empty)
                {
                    while (inputting)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        //Prints List of vehicles
                        Heading("Vehicle List");
                        BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);

                        foreach (Vehicle currentVehicle in mainProgram.fleetMain.GetVehicles())
                        {
                            string Print;
                            List<string> toBePrinted = currentVehicle.GetAttributList();
                            string modelLength = Convert.ToString(30 - toBePrinted[2].Length);
                            Print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                    iterator, toBePrinted[0], toBePrinted[1], toBePrinted[2], toBePrinted[3], toBePrinted[5], toBePrinted[6], toBePrinted[7], toBePrinted[8], toBePrinted[9], toBePrinted[11], toBePrinted[10]);
                            iterator++;
                            BasicBody(Print, "*", false);
                        };
                        BasicBody();
                        if (error)
                        {
                            BasicBody("Please enter a valid input", "!");
                        }
                        else
                        {
                            BasicBody();
                        };

                        BasicBody("Select the number of Vehicle to be Modified");
                        BasicBody("Eneter (B) Back to return to the previous menu");
                        bottomScreenBackfill((iterator) + 4);
                        Write(">>>");
                        string userInput = ReadLine();
                        error = ListInputChecks(userInput);
                        if (!error)
                        {
                            bool interger = Int32.TryParse(userInput, out option);
                            if (!interger)
                            {
                                inputting = false;
                                VehicleManagmentInitialScreen();
                            }
                            else
                            {
                                inputting = false;
                                if (delete)
                                {
                                    deleteScreen(false, userInput);
                                }
                                else
                                {
                                    VehicleModifyScreen(mainProgram.fleetMain.GetVehicle(option - 1));
                                };
                            };
                        }
                    };
                }
                else if (currentLength == 0)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Vehicle List");
                    BasicBody();
                    BasicBody();
                    BasicBody("No Vehicles to Display please Add or load vehicles to Database", "!");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    VehicleManagmentInitialScreen();
                }
                else
                {
                    //Variables for use throughout the display of this system
                    List<Vehicle> vehicleList = mainProgram.fleetMain.GetVehicles();
                    Vehicle toBePrinted;
                    List<string> vehicleINFO = new List<string> { };
                    int currentloop = 0;
                    string print;
                    int lineNO;
                    decimal noPages = decimal.Ceiling(Convert.ToDecimal(trueLength) / Convert.ToDecimal(maximumInfoVeh));

                    //While vehicle is getting retrived to be printed in an apporirate format
                    while (inputting)
                    {
                        lineNO = 5;
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Vehicle Lists");
                        BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);
                        for (int importantIterator = 0; importantIterator < maximumInfoVeh; importantIterator++)
                        {
                            toBePrinted = vehicleList[importantIterator + currentloop * maximumInfoVeh];
                            vehicleINFO = toBePrinted.GetAttributList();
                            string modelLength = Convert.ToString(30 - vehicleINFO[2].Length);
                            print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                        importantIterator + 1, vehicleINFO[0], vehicleINFO[1], vehicleINFO[2], vehicleINFO[3], vehicleINFO[5], vehicleINFO[6], vehicleINFO[7], vehicleINFO[8], vehicleINFO[9], vehicleINFO[11], vehicleINFO[10]);
                            BasicBody(print, "*", false);
                            lineNO++;
                            if ((importantIterator + 1) + currentloop * maximumInfoVeh == trueLength)
                            {
                                importantIterator = maximumInfoVeh;
                            };
                        };
                        string pageINFO = string.Format("Page {0} of {1}", currentloop + 1, noPages);
                        BasicBody(pageINFO);
                        BasicBody("Press N for the next page or P for the previous Page");
                        BasicBody("Enter the number to Modify/Delete or enter B to go back to the previous menu");
                        if (error)
                        {
                            ErrorMessage((int)ErrorType.InvalidInput);
                            error = false;
                        }
                        else
                        {

                            BasicBody();
                        };
                        bottomScreenBackfill(lineNO);
                        Write(">>>");
                        string user = ReadLine();
                        //Checks input is correct
                        error = ListInputChecks(user);
                        //If no error occurs
                        if (!error)
                        {
                            bool interger = Int32.TryParse(user, out int dataPoint);
                            if (!interger)
                            {
                                switch (user.ToLower())
                                {
                                    case "n":
                                        currentloop++;
                                        break;
                                    case "p":
                                        currentloop--;
                                        break;
                                    case "b":
                                        inputting = false;
                                        VehicleManagmentInitialScreen();
                                        break;
                                };
                            }
                            else
                            {
                                inputting = false;
                                Vehicle vehicleToPass = mainProgram.fleetMain.GetVehicle((dataPoint - 1) + currentloop * maximumInfoVeh);
                                string vehicleIDToPASS = Convert.ToString((dataPoint - 1) + currentloop * maximumInfoVeh);
                                if (delete)
                                {
                                    deleteScreen(false, vehicleIDToPASS);
                                }
                                else
                                {
                                    VehicleModifyScreen(vehicleToPass);
                                };
                            }
                        };
                    };
                };
            }
        }

        //This is to make sure the information entered for the customer is correct formate
        //It takes the raw input and also the stageor field that is being entered
        public static bool INFOCheck(string checking, int stage)
        {
            bool goodToMoveAHEAD = false;
            string subString;
            switch (stage)
            {
                case 0:
                    subString = checking.Substring(0, 1);
                    checking = checking.Replace(subString, subString.ToUpper());
                    goodToMoveAHEAD = Enum.IsDefined(typeof(Title), checking);
                    break;
                case 1:
                    goodToMoveAHEAD = !(Int32.TryParse(checking, out int empty));
                    break;
                case 2:
                    goodToMoveAHEAD = !(Int32.TryParse(checking, out empty));
                    break;
                case 3:
                    subString = checking.Substring(0, 1);
                    checking = checking.Replace(subString, subString.ToUpper());
                    goodToMoveAHEAD = Enum.IsDefined(typeof(Gender), checking);
                    break;
                case 4:
                    goodToMoveAHEAD = DateTime.TryParse(checking, out DateTime empty2);
                    break;
            }
            return goodToMoveAHEAD;
        }

        //This screen allows a new customer to be added to the database
        public static void CustomerAddScreen()
        {
            //Intilise Variables for use
            string[] potentialCustomer = new string[5];
            bool error = false;
            bool exit = false;

            //This for loop allows for the iteration throw the cutomer information
            for (int iterator = 0; iterator <= potentialCustomer.Length; iterator++)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("MRRC - Customer - NEW");
                BasicBody();
                BasicBody();
                BasicBody("1) Title:         " + potentialCustomer[0]);
                BasicBody();
                BasicBody("2) First Name:    " + potentialCustomer[1]);
                BasicBody();
                BasicBody("3) Last Name:     " + potentialCustomer[2]);
                BasicBody();
                BasicBody("4) Gender:        " + potentialCustomer[3]);
                BasicBody();
                BasicBody("5) Date of Birth: " + potentialCustomer[4]);
                BasicBody();
                BasicBody("Enter Details the form will fill in as inputs are valid.");
                BasicBody();
                BasicBody("Enter B to return to the previous menu (Warning! information will be lost)");
                //If an error occurs the relvant Error Message is displayed depending on which stage has the error
                if (error)
                {
                    BasicBody();
                    BasicBody();
                    switch (iterator)
                    {
                        case 0:
                            ErrorMessage((int)ErrorType.NotProperTitle);
                            break;
                        case 1:
                            ErrorMessage((int)ErrorType.InvalidInput);
                            break;
                        case 2:
                            ErrorMessage((int)ErrorType.InvalidInput);
                            break;
                        case 3:
                            ErrorMessage((int)ErrorType.NotPartOfGender);
                            break;
                        case 4:
                            ErrorMessage((int)ErrorType.DateFormateError);
                            break;
                    };
                    bottomScreenBackfill(19);
                }
                else
                {
                    bottomScreenBackfill(16);
                };
                error = false;
                //This occurs when all fields are succesful and this customer can be added to the database
                if (iterator == potentialCustomer.Length)
                {
                    BasicBody("Are these Details Correct (Y)Yes/(N)No");
                    Write(">>>");
                    string toCheck = ReadLine();
                    toCheck = toCheck.ToLower();
                    //Passes information to the Modify screen for further editing before saving
                    if (toCheck == "n")
                    {
                        CustomerModifyScreen(null, potentialCustomer);
                    }
                }
                else
                {
                    Write(">>>");
                    string toCheck = ReadLine();
                    //Pases the input and what stage the customer information is on
                    //If this is correct the program continues
                    bool DoubleCheck = INFOCheck(toCheck, iterator);
                    if (toCheck.ToLower() == "b")
                    {
                        iterator = potentialCustomer.Length + 1;
                        exit = true;
                    }
                    else
                    {
                        if (DoubleCheck)
                        {
                            potentialCustomer[iterator] = toCheck;
                        }
                        //Forces the iterator back so the adding stage remains the same
                        else
                        {
                            iterator--;
                            error = true;
                        }
                    };
                };
            };
            if (exit)
            {
                CustomerManagmentInitialScreen();
            }
            else
            {
                string substring1;
                string substring2;
                //This is to make sure the inputs are in the correct formate for the prgam to recognise
                substring1 = potentialCustomer[0].Substring(0, 1);
                potentialCustomer[0] = potentialCustomer[0].Replace(substring1, substring1.ToUpper());

                substring2 = potentialCustomer[3].Substring(0, 1);
                potentialCustomer[3] = potentialCustomer[3].Replace(substring2, substring2.ToUpper());

                customer newCustomer = new customer(1, potentialCustomer[0], potentialCustomer[1], potentialCustomer[2], potentialCustomer[3], potentialCustomer[4]);
                bool finalpage = mainProgram.CRMMain.AddCustomer(newCustomer);

                if (finalpage)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("MRRC - Customer - NEW");
                    BasicBody();
                    BasicBody();
                    BasicBody("Customer Succesfully added");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    CustomerManagmentInitialScreen();
                }
                else
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("MRRC - Customer - ERROR");
                    BasicBody();
                    BasicBody();
                    BasicBody("A customer with that name and DOB already exists", "!");
                    BasicBody("You'll be take to the customer managament screen");
                    bottomScreenBackfill(7);
                    Write(">>>");
                    ReadLine();
                    CustomerManagmentInitialScreen();
                }
            }

        }

        //General main screen of the program this allows the program to go to new screens
        public static void MainScreen()
        {
            int[] error = new int[2] { 0, 0 };
            bool inputs = true;
            int currentLIne = 0;
            while (inputs)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("WELCOME TO MRRC");
                BasicBody();
                BasicBody("Please enter the number to access the option");
                BasicBody();
                BasicBody("1) Customer Managment");
                BasicBody("2) Vehicle Managment");
                BasicBody("3) Rental Managment");
                BasicBody("4) Open/Save Database");
                BasicBody("Enter Q to quit the program (NO CHANGES WILL BE SAVED)", "!");
                currentLIne = 11;
                if (error[0] == 0)
                {
                    if(mainProgram.CRMMain.fileOPEN)
                    {
                        BasicBody("The Customer is currently open, close and reload the database or program", "!");
                        currentLIne++;
                    }
                    else if (mainProgram.CRMMain.numOfEntrys() == 0)
                    {
                        BasicBody("No Customers Loaded please Load a database or create Customers", "!");
                        currentLIne++;
                    };
                    if (mainProgram.fleetMain.fleetOpen)
                    {
                        BasicBody("Fleet File appears to be open please close it and reload the database or program", "!");
                        currentLIne++;
                    }
                    if(mainProgram.fleetMain.rentalsOpen)
                    {
                        BasicBody("Rental File appears to be open please close it and reload the database or program", "!");
                        currentLIne++;
                    }
                    if(mainProgram.fleetMain.rentalsOpen)
                    {
                        BasicBody("An essential File appears to be open please close it and reload the database or program", "!");
                        currentLIne++;
                    }
                    else if (mainProgram.fleetMain.VehicleCurrentAmount() == 0)
                    {
                        BasicBody("No Vehicles Loaded please Load a database or create Vehicles", "!");
                        currentLIne++;
                    }
                    bottomScreenBackfill(currentLIne);
                }
                else
                {
                    BasicBody();
                    BasicBody();
                    ErrorMessage(error[0]);
                    currentLIne += 3;
                    if (mainProgram.CRMMain.numOfEntrys() == 0)
                    {
                        BasicBody("No Customers Loaded please Load a database or create Customers", "!");
                        currentLIne++;
                    }
                    if (mainProgram.fleetMain.VehicleCurrentAmount() == 0)
                    {
                        BasicBody("No Vehicles Loaded please Load a database or create Vehicles", "!");
                        currentLIne++;
                    }
                    bottomScreenBackfill(currentLIne);
                };
                error = UserPrompt(Location.ScreenMain);
                if (error[0] == 0)
                {
                    switch (error[1])
                    {
                        case (int)ValidInputsMain.Customer:
                            inputs = false;
                            CustomerManagmentInitialScreen();
                            break;
                        case (int)ValidInputsMain.Vechicle:
                            inputs = false;
                            VehicleManagmentInitialScreen();
                            break;
                        case (int)ValidInputsMain.NEW:
                            inputs = false;
                            RentingMain();
                            break;
                        case (int)ValidInputsMain.DOC:
                            inputs = false;
                            SaveLoad();
                            break;
                        case (int)ValidInputsMain.QUIT:
                            inputs = false;
                            Process.GetCurrentProcess().Kill();
                            break;
                    };
                };
            };
        }

        //This is the menu to load or save the databse in the current state
        public static void SaveLoad()
        {
            //Variables to use throughout the menu
            bool acceptingInputing = true;
            int[] response = new int[2];

            while (acceptingInputing)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                }
                Heading("Database Mnagement");
                BasicBody();
                BasicBody();
                BasicBody("1) Save Customer Database");
                BasicBody("2) Load Customer Database");
                BasicBody("3) Save Vehicle Database");
                BasicBody("4) Load Vehicle Database");
                BasicBody("5) Save Renting Database");
                BasicBody("6) Load Renting Database");
                BasicBody();
                BasicBody("B) Back to Main Menu");
                BasicBody();
                if (response[0] != 0)
                {
                    ErrorMessage((int)ErrorType.InvalidInput);
                }
                else
                {
                    BasicBody();
                }
                bottomScreenBackfill(16);
                response = UserPrompt(Location.SaveLoadData);
                if (response[0] == 0)
                {
                    acceptingInputing = false;
                    switch (response[1])
                    {
                        case (int)ValidInputsSaveLoad.SaveCust:
                            DatabaseManagement(true);
                            break;
                        case (int)ValidInputsSaveLoad.LoadCust:
                            DatabaseManagement(false);
                            break;
                        case (int)ValidInputsSaveLoad.SaveVeh:
                            DatabaseManagement(true, false);
                            break;
                        case (int)ValidInputsSaveLoad.LoadVeh:
                            DatabaseManagement(false, false);
                            break;
                        case (int)ValidInputsSaveLoad.SaveFleet:
                            DatabaseManagement(true, false, true);
                            break;
                        case (int)ValidInputsSaveLoad.LoadFleet:
                            DatabaseManagement(false, false, true);
                            break;
                        default:
                            MainScreen();
                            break;
                    };
                };
            };

        }

        //Save/Load customers database screen
        //First pass through is whether or not the database is being saved
        //Secound Pass through is if it is a customer datbase or not
        //And the last pass through is if it is the renting database or not
        static public void DatabaseManagement(bool savingDatabase, bool customerDATABSE = true, bool rentingDATABASE = false)
        {
            string defualtDirectory = @"..\..\..\..\\Data";
            //This Regular Expression is to check the file name is valid
            string filenameREGEX = @"^[A-Za-z0-9][A-Za-z0-9]+";
            Regex searchpattern = new Regex(filenameREGEX);

            //Process of saving or loading for customers
            if (customerDATABSE)
            {
                if (savingDatabase)
                {
                    //Variables for use for saving database
                    string fileNameDefualt = "Customer";
                    string fileActual = fileNameDefualt + ".csv";
                    string directoryActual = defualtDirectory;
                    string line1;
                    string line2;
                    string lineEditing;
                    string user;

                    int[] editingField = new int[2] { 0, 0 };
                    bool inputting = true;
                    bool editing = false;

                    while (inputting)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        //Displays Information before the user saves
                        Heading("Saving Customer Database");
                        BasicBody();
                        BasicBody();
                        line1 = "1) File Name:      " + fileActual;
                        BasicBody(line1);
                        BasicBody();
                        line2 = "2) Directory Name: " + directoryActual;
                        BasicBody(line2);
                        BasicBody();
                        if (editingField[0] == 1)
                        {
                            ErrorMessage((int)ErrorType.InvalidInput);
                            editingField[0] = 0;
                        }
                        else
                        {
                            BasicBody();
                        };
                        switch (editingField[1])
                        {
                            case 1:
                                lineEditing = "Currently Editing File name: " + fileActual;
                                BasicBody(lineEditing);
                                break;
                            case 2:
                                lineEditing = "Currently Editing Directory: " + directoryActual;
                                BasicBody(lineEditing);
                                break;
                            default:
                                BasicBody();
                                break;
                        };
                        BasicBody();
                        BasicBody("Enter the number to edit the field, or type B to reutrn to the previous screen");
                        BasicBody("When ready type S to save if a file with the same name exits in the Directory it will be ovenwritten");
                        bottomScreenBackfill(15);
                        Write(">>>");
                        user = ReadLine();
                        //Checks input is correct
                        if (!editing)
                        {
                            switch (user.ToLower())
                            {
                                case "1":
                                    editing = true;
                                    editingField[1] = 1;
                                    break;
                                case "2":
                                    editing = true;
                                    editingField[1] = 2;
                                    break;
                                case "b":
                                    inputting = false;
                                    SaveLoad();
                                    break;
                                case "s":
                                    mainProgram.CRMMain.SaveToFile(fileActual, directoryActual);
                                    inputting = false;
                                    break;
                                default:
                                    editingField[0] = 1;
                                    break;
                            };
                        }
                        //Checks if file name is correct and directory is existing
                        else
                        {
                            if (editingField[1] == 1)
                            {
                                bool noERROR = searchpattern.IsMatch(user);
                                if (noERROR)
                                {
                                    fileActual = user + ".csv";
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            }
                            else
                            {
                                bool noERROR = Directory.Exists(user);
                                if (noERROR)
                                {
                                    directoryActual = user;
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            };
                        };
                    };
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Database sucesfully Saved");
                    BasicBody();
                    BasicBody();
                    BasicBody("The database is saved sucesfully");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    SaveLoad();
                }
                else
                {
                    //Variables to check 
                    bool checkfile1 = false;
                    bool checkfile2 = false;
                    bool finalCheck = false;
                    bool error = false;
                    bool errorOpen = false;
                    string inputs;
                    string potentialDirectory = "";
                    string fileName = "";
                    string[] listOfFiles;

                    //First Check is for if the directory currently exits
                    while (!checkfile1)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load a customer Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("Enter the directory the database is loaded from:");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That Directory does not exist make sure it is correct", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(8);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        if (Directory.Exists(inputs))
                        {
                            checkfile1 = true;
                            potentialDirectory = inputs;
                        }
                        else
                        {
                            error = true;
                        };
                    };
                    listOfFiles = Directory.GetFiles(potentialDirectory);

                    //Select a file
                    while (!checkfile2)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        int minValue = 1;
                        int MaxValue = minValue;
                        Heading("Load Database");
                        BasicBody();
                        foreach (string currentFile in listOfFiles)
                        {
                            //Make sure file is a .csv file
                            int length = currentFile.Length;
                            int lengthDIR = potentialDirectory.Length;
                            bool isCSV = currentFile.Substring(length - 4, 4) == ".csv";
                            if (isCSV)
                            {
                                string print = MaxValue.ToString() + ") " + currentFile.Substring(lengthDIR, length - lengthDIR);
                                MaxValue++;
                                BasicBody(print);
                            };
                        };
                        BasicBody();
                        BasicBody("Enter the number of the proper file name");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(6 + MaxValue);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        error = !(Int32.TryParse(inputs, out int value));
                        if (!error)
                        {
                            if (value <= MaxValue & value >= minValue)
                            {
                                fileName = listOfFiles[value - 1];
                                checkfile2 = true;
                            }
                            else
                            {
                                error = true;
                            }
                        };
                    };

                    //Final check for the user
                    while (!finalCheck)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("This is the file name:");
                        int length = fileName.Length;
                        int lengthDIR = potentialDirectory.Length;
                        BasicBody(fileName.Substring(lengthDIR, length - lengthDIR));
                        BasicBody("This is located here:");
                        BasicBody(potentialDirectory, "*", false);
                        BasicBody();
                        BasicBody("If you are ready to load the database Type (Y) Yes");
                        BasicBody("Warning this will delete all work previous to this", "!");
                        BasicBody("If you are not ready type (N) No");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        };
                        bottomScreenBackfill(15);
                        Write(">>>");
                        inputs = ReadLine();
                        switch (inputs.ToLower())
                        {
                            case "y":
                                finalCheck = true;
                                try
                                {
                                    mainProgram.CRMMain.LoadFromFile(fileName);
                                }
                                catch(IOException)
                                {
                                    errorOpen = true;
                                }
                                break;
                            case "n":
                                finalCheck = false;
                                SaveLoad();
                                break;
                            default:
                                error = true;
                                break;
                        };

                    };
                    if (!errorOpen)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Succesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Customer Database has been loaded succesfully");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                    else
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Unsuccesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Customer Database could not be loaded as it is open");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                };
            }
            //Process for loading or saving the renting database
            else if (rentingDATABASE)
            {
                if (savingDatabase)
                {
                    string fileNameDefualt = "Fleet";
                    string fileActual = fileNameDefualt + ".csv";
                    string dirActual = defualtDirectory;
                    bool inputting = true;
                    bool editing = false;
                    string line1;
                    string line2;
                    string lineEditing;
                    string user;
                    int[] editingField = new int[2] { 0, 0 };


                    while (inputting)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Saving Rental Database");
                        BasicBody();
                        BasicBody();
                        line1 = "1) File name: " + fileActual;
                        BasicBody(line1);
                        BasicBody();
                        line2 = "2) Save Directory: " + defualtDirectory;
                        BasicBody(line2);
                        BasicBody();
                        if (editingField[0] == 1)
                        {
                            ErrorMessage((int)ErrorType.InvalidInput);
                            editingField[0] = 0;
                        }
                        else
                        {
                            BasicBody();
                        };
                        switch (editingField[1])
                        {
                            case 1:
                                lineEditing = "Currently editing File name: " + fileActual;
                                BasicBody(lineEditing);
                                break;
                            case 2:
                                lineEditing = "Currently editing directory: " + dirActual;
                                BasicBody(lineEditing);
                                break;
                            default:
                                BasicBody();
                                break;
                        };
                        BasicBody();
                        BasicBody("Enter the number to edit the field, or type B to reutrn to the previous screen");
                        BasicBody("When ready type S to save if a file with the same name exits in the Directory it will be ovenwritten");
                        bottomScreenBackfill(15);
                        Write(">>>");
                        user = ReadLine();
                        //Checks input is correct
                        if (!editing)
                        {
                            switch (user.ToLower())
                            {
                                case "1":
                                    editing = true;
                                    editingField[1] = 1;
                                    break;
                                case "2":
                                    editing = true;
                                    editingField[1] = 2;
                                    break;
                                case "b":
                                    inputting = false;
                                    SaveLoad();
                                    break;
                                case "s":
                                    mainProgram.fleetMain.saveFleetDatabase(fileActual, dirActual);
                                    inputting = false;
                                    break;
                                default:
                                    editingField[0] = 1;
                                    break;
                            };
                        }
                        //Checks if file name is correct and directory is existing
                        else
                        {
                            if (editingField[1] == 1)
                            {
                                bool noERROR = searchpattern.IsMatch(user);
                                if (noERROR)
                                {
                                    fileActual = user + ".csv";
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            }
                            else
                            {
                                bool noERROR = Directory.Exists(user);
                                if (noERROR)
                                {
                                    dirActual = user;
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            };
                        };
                    };
                    Heading("Database Succesfully Saved");
                    BasicBody();
                    BasicBody();
                    BasicBody("The database has been succesfully saved");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    SaveLoad();
                }
                else
                {
                    //Variables to check 
                    bool checkfile1 = false;
                    bool checkfile2 = false;
                    bool finalCheck = false;
                    bool error = false;
                    bool fileError = false;
                    string inputs;
                    string potentialDirectory = "";
                    string fileName = "";
                    string[] listOfFiles;

                    //First Check is for if the directory currently exits
                    while (!checkfile1)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load a rentals Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("Enter the directory the database is loaded from:");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That Directory does not exist make sure it is correct", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(8);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        if (Directory.Exists(inputs))
                        {
                            checkfile1 = true;
                            potentialDirectory = inputs;
                        }
                        else
                        {
                            error = true;
                        };
                    };
                    listOfFiles = Directory.GetFiles(potentialDirectory);

                    //Select a file
                    while (!checkfile2)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        int minValue = 1;
                        int MaxValue = minValue;
                        Heading("Load Database");
                        BasicBody();
                        foreach (string currentFile in listOfFiles)
                        {
                            //Make sure file is a .csv file
                            int length = currentFile.Length;
                            int lengthDIR = potentialDirectory.Length;
                            bool isCSV = currentFile.Substring(length - 4, 4) == ".csv";
                            if (isCSV)
                            {
                                string print = MaxValue.ToString() + ") " + currentFile.Substring(lengthDIR, length - lengthDIR);
                                MaxValue++;
                                BasicBody(print);
                            };
                        };
                        BasicBody();
                        BasicBody("Enter the number of the proper file name");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(6 + MaxValue);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        error = !(Int32.TryParse(inputs, out int value));
                        if (!error)
                        {
                            if (value <= MaxValue & value >= minValue)
                            {
                                fileName = listOfFiles[value - 1];
                                checkfile2 = true;
                            }
                            else
                            {
                                error = true;
                            }
                        };
                    };

                    //Final check for the user
                    while (!finalCheck)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("This is the file name:");
                        int length = fileName.Length;
                        int lengthDIR = potentialDirectory.Length;
                        BasicBody(fileName.Substring(lengthDIR, length - lengthDIR));
                        BasicBody("This is located here:");
                        BasicBody(potentialDirectory, "*", false);
                        BasicBody();
                        BasicBody("If you are ready to load the database Type (Y) Yes");
                        BasicBody("Warning this will delete all work previous to this", "!");
                        BasicBody("If you are not ready type (N) No");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        };
                        bottomScreenBackfill(15);
                        Write(">>>");
                        inputs = ReadLine();
                        switch (inputs.ToLower())
                        {
                            case "y":
                                finalCheck = true;
                                try
                                {
                                    mainProgram.fleetMain.loadRENTALDatabase(fileName);
                                    fileError = false;
                                }
                                catch (IOException)
                                {
                                    fileError = true;
                                }
                                break;
                            case "n":
                                finalCheck = false;
                                SaveLoad();
                                break;
                            default:
                                error = true;
                                break;
                        };

                    };
                    if (!fileError)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Succesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Rental Database has been loaded succesfully");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                    else
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Unuccesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Rental Database has been loaded unsuccesfully, it appears to be open currently");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                }
            }
            //Process for loading or saving the fleet database
            else
            {
                if (savingDatabase)
                {
                    //Variables for use for saving database
                    string fileNameDefualt = "Vehicle";
                    string fileActual = fileNameDefualt + ".csv";
                    string directoryActual = defualtDirectory;
                    string line1;
                    string line2;
                    string lineEditing;
                    string user;
                    //This Regular Expression is to check the file name is valid
                    int[] editingField = new int[2] { 0, 0 };
                    bool acceptingINPUT = true;
                    bool editing = false;

                    while (acceptingINPUT)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        //Displays Information before the user saves
                        Heading("Saving Vehicle Database");
                        BasicBody();
                        BasicBody();
                        line1 = "1) File Name:      " + fileActual;
                        BasicBody(line1);
                        BasicBody();
                        line2 = "2) Directory Name: " + directoryActual;
                        BasicBody(line2);
                        BasicBody();
                        if (editingField[0] == 1)
                        {
                            ErrorMessage((int)ErrorType.InvalidInput);
                            editingField[0] = 0;
                        }
                        else
                        {
                            BasicBody();
                        };
                        switch (editingField[1])
                        {
                            case 1:
                                lineEditing = "Currently Editing File name: " + fileActual;
                                BasicBody(lineEditing);
                                break;
                            case 2:
                                lineEditing = "Currently Editing Directory: " + directoryActual;
                                BasicBody(lineEditing);
                                break;
                            default:
                                BasicBody();
                                break;
                        };
                        BasicBody();
                        BasicBody("Enter the number to edit the field, or type B to reutrn to the previous screen");
                        BasicBody("When ready type S to save if a file with the same name exits in the Directory it will be ovenwritten");
                        bottomScreenBackfill(15);
                        Write(">>>");
                        user = ReadLine();
                        //Checks input is correct
                        if (!editing)
                        {
                            switch (user.ToLower())
                            {
                                case "1":
                                    editing = true;
                                    editingField[1] = 1;
                                    break;
                                case "2":
                                    editing = true;
                                    editingField[1] = 2;
                                    break;
                                case "b":
                                    acceptingINPUT = false;
                                    SaveLoad();
                                    break;
                                case "s":
                                    mainProgram.fleetMain.SavVEHICLEDatabase(fileActual, directoryActual);
                                    acceptingINPUT = false;
                                    break;
                                default:
                                    editingField[0] = 1;
                                    break;
                            };
                        }
                        //Checks if file name is correct and directory is existing
                        else
                        {
                            if (editingField[1] == 1)
                            {
                                bool noERROR = searchpattern.IsMatch(user);
                                if (noERROR)
                                {
                                    fileActual = user + ".csv";
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            }
                            else
                            {
                                bool noERROR = Directory.Exists(user);
                                if (noERROR)
                                {
                                    directoryActual = user;
                                    editingField[1] = 0;
                                    editing = false;
                                }
                                else
                                {
                                    editingField[0] = 1;
                                };
                            };
                        };
                    };
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Database sucesfully Saved");
                    BasicBody();
                    BasicBody();
                    BasicBody("The database is saved sucesfully");
                    bottomScreenBackfill(6);
                    Write(">>>");
                    ReadLine();
                    SaveLoad();
                }
                else
                {
                    //Variables to check 
                    bool checkfile1 = false;
                    bool checkfile2 = false;
                    bool finalCheck = false;
                    bool error = false;
                    bool errorFile = false;
                    string inputs;
                    string potentialDirectory = "";
                    string fileName = "";
                    string[] listOfFiles;

                    //First Check is for if the directory currently exits
                    while (!checkfile1)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load a vehicle Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("Enter the directory the database is loaded from:");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That Directory does not exist make sure it is correct", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(8);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        if (Directory.Exists(inputs))
                        {
                            checkfile1 = true;
                            potentialDirectory = inputs;
                        }
                        else
                        {
                            error = true;
                        };
                    };
                    listOfFiles = Directory.GetFiles(potentialDirectory);

                    //Select a file
                    while (!checkfile2)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        int minValue = 1;
                        int MaxValue = minValue;
                        Heading("Load Database");
                        BasicBody();
                        foreach (string currentFile in listOfFiles)
                        {
                            //Make sure file is a .csv file
                            int length = currentFile.Length;
                            int lengthDIR = potentialDirectory.Length;
                            bool isCSV = currentFile.Substring(length - 4, 4) == ".csv";
                            if (isCSV)
                            {
                                string print = MaxValue.ToString() + ") " + currentFile.Substring(lengthDIR, length - lengthDIR);
                                MaxValue++;
                                BasicBody(print);
                            };
                        };
                        BasicBody();
                        BasicBody("Enter the number of the proper file name");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        }
                        bottomScreenBackfill(6 + MaxValue);
                        Write(">>>");
                        inputs = ReadLine();
                        if (inputs.ToLower() == "b")
                        {
                            SaveLoad();
                        }
                        error = !(Int32.TryParse(inputs, out int value));
                        if (!error)
                        {
                            if (value <= MaxValue & value >= minValue)
                            {
                                fileName = listOfFiles[value - 1];
                                checkfile2 = true;
                            }
                            else
                            {
                                error = true;
                            }
                        };
                    };

                    //Final check for the user
                    while (!finalCheck)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Load Database");
                        BasicBody();
                        BasicBody();
                        BasicBody("This is the file name:");
                        int length = fileName.Length;
                        int lengthDIR = potentialDirectory.Length;
                        BasicBody(fileName.Substring(lengthDIR, length - lengthDIR));
                        BasicBody("This is located here:");
                        BasicBody(potentialDirectory, "*", false);
                        BasicBody();
                        BasicBody("If you are ready to load the database Type (Y) Yes");
                        BasicBody("Warning this will delete all work previous to this", "!");
                        BasicBody("If you are not ready type (N) No");
                        BasicBody();
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                            error = false;
                        }
                        else
                        {
                            BasicBody();
                        };
                        bottomScreenBackfill(15);
                        Write(">>>");
                        inputs = ReadLine();
                        switch (inputs.ToLower())
                        {
                            case "y":
                                finalCheck = true;
                                try
                                {
                                    mainProgram.fleetMain.LoadVEHICLEDatabase(fileName);
                                    errorFile = false;
                                }
                                catch (IOException)
                                {
                                    errorFile = true;
                                }
                                break;
                            case "n":
                                finalCheck = false;
                                SaveLoad();
                                break;
                            default:
                                error = true;
                                break;
                        };

                    };
                    if (!errorFile)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Succesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Vehicle Database has been loaded succesfully");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                    else
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        }
                        Heading("Unuccesfully Loaded");
                        BasicBody();
                        BasicBody();
                        BasicBody("The Vehicle Database appears to be currently open and is unale to be loaded");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        SaveLoad();
                    }
                };
            };
        }



        //This is the main screen for vehicle managment and changes screen accordingly
        public static void VehicleManagmentInitialScreen()
        {
            //Initilise variables to use throughout function
            bool acceptingInput = true;
            int[] error = new int[] { 0, 0 };

            while (acceptingInput)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("Vehicle Management -  Main Screen");
                BasicBody();
                BasicBody();
                BasicBody("1) New Vehicle");
                BasicBody("2) Delete Vehicle");
                BasicBody("3) List Vehicles");
                BasicBody("4) Search Vehicles");

                if (error[0] != 0)
                {
                    ErrorMessage((int)ErrorType.InvalidInput);
                }
                else
                {
                    BasicBody();
                }
                BasicBody("Press B to go Back");
                bottomScreenBackfill(11);
                //Passes input through chekcing and directing the program to the next screen
                error = UserPrompt(Location.VehicleMain);
                if (error[0] == 0)
                {
                    switch (error[1])
                    {
                        case (int)ValidInputsVehicleMain.Add:
                            acceptingInput = false;
                            VehicleAddScreen();
                            break;
                        case (int)ValidInputsVehicleMain.Delete:
                            acceptingInput = false;
                            ListofInformation(false, true);
                            break;
                        case (int)ValidInputsVehicleMain.List:
                            acceptingInput = false;
                            ListofInformation(false);
                            break;
                        case (int)ValidInputsAdjustVeh.Back:
                            acceptingInput = false;
                            MainScreen();
                            break;
                        case (int)ValidInputsVehicleMain.Search:
                            acceptingInput = false;
                            SearchVehicles();
                            break;
                    };
                }

            };
        }

        //This screen will start the Vehicle adding screen till the grade
        //Oonce the grade has been filled in the standards will prefill and this information can be passed
        //To the modify screen
        public static void VehicleAddScreen()
        {
            //Variables for use throughout screen
            string[] newVehicleMax = new string[12];
            string[] newVehicleMin = new string[5];
            List<string> VehicleINFO = new List<string> { };
            string[] placeHolder = new string[12] { "Regestration:", "Grade:", "Make:", "Model:", "Year:", "Seats:", "Transmission:", "Fuel:", "GPS:", "Sunroof:", "Rate:", "Colour:" };
            string userINPUT = "";
            bool inputting = true;
            int minimumInput = 5;
            int rowsToDisplay = 6;
            bool error = false;
            //Initial first 5 fields for input
            for (int stageNO = 0; stageNO < minimumInput; stageNO++)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                //Consistent format similiar to the modify screen
                Heading("Vehicle - Add");
                BasicBody();
                for (int toprint = 0; toprint < rowsToDisplay; toprint++)
                {
                    string stringtoDisplay = string.Format("{0,-2}){4,-15}{1,-15}{2,-2}){5,-15}{3,-15}",
                                                      toprint + 1, newVehicleMax[toprint], toprint + 7, newVehicleMax[toprint + 6], placeHolder[toprint], placeHolder[toprint + 6]);
                    BasicBody(stringtoDisplay);
                    BasicBody();
                };
                BasicBody("Enter the correct response to each field press B to go back no progress will be saved.");
                BasicBody();
                //Different prompts for each field
                switch (stageNO + 1)
                {
                    case (int)ValidInputsAdjustVeh.REGO:
                        BasicBody("Currently adding Regestration.");
                        BasicBody();
                        break;
                    case (int)ValidInputsAdjustVeh.GRADE:
                        BasicBody("Currently adding Vehicle Grade");
                        BasicBody("Plese enter (C)Commerical/(E)Economy/(L)Luxry/(F)Family");
                        break;
                    case (int)ValidInputsAdjustVeh.MAKE:
                        BasicBody("Currently adding vehicle make.");
                        BasicBody();
                        break;
                    case (int)ValidInputsAdjustVeh.MODEL:
                        BasicBody("Currently adding vehicle model.");
                        BasicBody();
                        break;
                    case (int)ValidInputsAdjustVeh.YEAR:
                        BasicBody("Currently adding vehicle year");
                        BasicBody();
                        break;
                };
                //When an error occurs different messages appear
                if (error)
                {
                    switch (stageNO)
                    {
                        case 0:
                            ErrorMessage((int)ErrorType.RegoIncorrect);
                            break;
                        default:
                            ErrorMessage((int)ErrorType.InvalidInput);
                            break;
                    };
                    error = false;
                }
                else
                {
                    BasicBody();
                };
                bottomScreenBackfill(19);
                Write(">>>");
                userINPUT = ReadLine();
                if (userINPUT.ToLower() == "b")
                {
                    inputting = false;
                    break;
                };
                error = CheckifValidVeh(userINPUT, stageNO);
                if (error)
                {
                    stageNO--;
                }
                else
                {
                    if (stageNO == (int)ValidInputsAdjustVeh.REGO)
                    {
                        switch (userINPUT.ToLower())
                        {
                            case "c":
                                userINPUT = "Commercial";
                                break;
                            case "l":
                                userINPUT = "Luxury";
                                break;
                            case "e":
                                userINPUT = "Economy";
                                break;
                            case "f":
                                userINPUT = "Family";
                                break;
                        };
                    };
                    newVehicleMax[stageNO] = userINPUT;
                    newVehicleMin[stageNO] = userINPUT;
                };
            }
            //Depending on grade a different constructor is called
            Vehicle AddVehicle = null;
            switch (newVehicleMax[1])
            {
                case "Commercial":
                    AddVehicle = new VehicleCommercial(newVehicleMin[0], newVehicleMin[1], newVehicleMin[2], newVehicleMin[3], Int32.Parse(newVehicleMin[4]));
                    VehicleINFO = AddVehicle.GetAttributList();
                    break;
                case "Luxury":
                    AddVehicle = new VehicleLuxry(newVehicleMin[0], newVehicleMin[1], newVehicleMin[2], newVehicleMin[3], Int32.Parse(newVehicleMin[4]));
                    VehicleINFO = AddVehicle.GetAttributList();
                    break;
                case "Economy":
                    AddVehicle = new VehicleEconomy(newVehicleMin[0], newVehicleMin[1], newVehicleMin[2], newVehicleMin[3], Int32.Parse(newVehicleMin[4]));
                    VehicleINFO = AddVehicle.GetAttributList();
                    break;
                case "Family":
                    AddVehicle = new VehicleFamily(newVehicleMin[0], newVehicleMin[1], newVehicleMin[2], newVehicleMin[3], Int32.Parse(newVehicleMin[4]));
                    VehicleINFO = AddVehicle.GetAttributList();
                    break;
            };
            while (inputting)
            {
                //Displays the information for a final confirmation
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("Vehicle - Add");
                BasicBody();
                for (int toprint = 0; toprint < rowsToDisplay; toprint++)
                {
                    string stringtoDisplay = string.Format("{0,-2}){4,-15}{1,-15}{2,-2}){5,-15}{3,-15}",
                                                          toprint + 1, VehicleINFO[toprint], toprint + 7, VehicleINFO[toprint + 6], placeHolder[toprint], placeHolder[toprint + 6]);
                    BasicBody(stringtoDisplay);
                    BasicBody();
                };
                BasicBody();
                BasicBody();
                BasicBody("This is the information we have on some defualts");
                BasicBody("Is this infomration correct (Y)Yes or (N) No");
                if (error)
                {
                    ErrorMessage((int)ErrorType.InvalidInput);
                    error = false;
                }
                else if (error & userINPUT.ToLower() == "y")
                {
                    BasicBody("There is a car with that exisiting in the database please edit the REGO", "!");
                }
                else
                {
                    BasicBody();
                };
                bottomScreenBackfill(18);
                Write(">>>");
                userINPUT = ReadLine();
                switch (userINPUT.ToLower())
                {
                    case "y":
                        error = !mainProgram.fleetMain.AddVehicle(AddVehicle);
                        inputting = false;
                        break;
                    case "n":
                        VehicleModifyScreen(AddVehicle, true);
                        break;
                    default:
                        error = true;
                        break;
                };
            };
            if (mainProgram.NOTRecording)
            {
                Clear();
            };
            if (userINPUT.ToLower() == "y")
            {
                Heading("Vehicle Added Succesfully");
                BasicBody();
                BasicBody();
                BasicBody("The Vehicle has been added to the database succesfully");
                bottomScreenBackfill(6);
                Write(">>>");
                ReadLine();
            }
            VehicleManagmentInitialScreen();
        }

        //Checks if the input is Valid for that specific scenario
        //Returns if there was an Error present within the input
        public static bool CheckifValidVeh(string input, int stageTOCheck)
        {
            bool ErrorPresent = false;
            int generalLength = input.Length;
            stageTOCheck++;
            switch (stageTOCheck)
            {
                //This is to check if the regestration is valid 
                case (int)ValidInputsAdjustVeh.REGO:
                    bool test3 = generalLength == 6;
                    if (!test3)
                    {
                        return true;
                    }
                    string numbers = input.Substring(0, 3);
                    string letters = input.Substring(3, 3);

                    bool test1 = Int32.TryParse(numbers, out int notNEEDED);
                    //This is to test that all the letters are indeed letters by try parsing them to int                    
                    bool test21 = !(Int32.TryParse(letters.Substring(0, 1), out int noTNEEDED));
                    bool test22 = !(Int32.TryParse(letters.Substring(1, 1), out int nOTNEEDED));
                    bool test23 = !(Int32.TryParse(letters.Substring(2, 1), out int NOTNEEDED));
                    bool test2 = test21 & test22 & test23;


                    if (!test1 | !test2 | !test3)
                    {
                        ErrorPresent = true;
                    }
                    break;
                //Check if the grade change is valid
                case (int)ValidInputsAdjustVeh.GRADE:
                    input = input.ToLower();
                    if (generalLength != 1)
                    {
                        ErrorPresent = true;
                    };
                    switch (input)
                    {
                        case "c":
                        case "l":
                        case "e":
                        case "f":
                            ErrorPresent = false;
                            break;
                        default:
                            ErrorPresent = true;
                            break;
                    };
                    break;
                //Check if year is correct
                case (int)ValidInputsAdjustVeh.YEAR:
                    if (generalLength != 4)
                    {
                        ErrorPresent = true;
                    }
                    ErrorPresent = !(Int32.TryParse(input, out int notNEEEDed));
                    break;
                //Checks if the seat input is an interger
                case (int)ValidInputsAdjustVeh.SEATS:
                    ErrorPresent = !(Int32.TryParse(input, out int notNEEDEd));
                    break;
                case (int)ValidInputsAdjustVeh.TRANSMISSION:
                    input = input.ToLower();
                    if (generalLength != 1)
                    {
                        ErrorPresent = true;
                    };
                    switch (input)
                    {
                        case "m":
                        case "a":
                            ErrorPresent = false;
                            break;
                        default:
                            ErrorPresent = true;
                            break;
                    };
                    break;
                //Checks if single letter input is correct
                case (int)ValidInputsAdjustVeh.FUELTYPE:
                    input = input.ToLower();
                    if (generalLength != 1)
                    {
                        ErrorPresent = true;
                    };
                    switch (input)
                    {
                        case "p":
                        case "d":
                            ErrorPresent = false;
                            break;
                        default:
                            ErrorPresent = true;
                            break;
                    };
                    break;
                //Checks if single input is correct i.e. Yes or no
                case (int)ValidInputsAdjustVeh.GPS:
                case (int)ValidInputsAdjustVeh.SUNROOF:
                    input = input.ToLower();
                    if (generalLength != 1)
                    {
                        ErrorPresent = true;
                    };
                    switch (input)
                    {
                        case "y":
                        case "n":
                            ErrorPresent = false;
                            break;
                        default:
                            ErrorPresent = true;
                            break;
                    };
                    break;
                case (int)ValidInputsAdjustVeh.RATE:
                    ErrorPresent = !(double.TryParse(input, out double notNEed));
                    break;
            };
            return ErrorPresent;
        }

        //This method displays the current vehicle and which aspects to modify
        //Passes the vehicle information and if the vehicle is new and needs to be added to the database
        public static void VehicleModifyScreen(Vehicle vehicleToModify = null, bool newVehicle = false)
        {

            bool saving = false;
            //Checks if there needs to be a record retrived
            bool findVehicle = vehicleToModify == null;
            if (findVehicle)
            {
                ListofInformation(false);
            }
            else
            {
                //This is to keep loop for inputting running
                bool inputting = true;
                bool error = false;
                bool editing = false;
                bool changes = false;
                int option = 0;
                string userInput;
                string[] placeHolder = new string[12] { "Regestration:", "Grade:", "Make:", "Model:", "Year:", "Seats:", "Transmission:", "Fuel:", "GPS:", "Sunroof:", "Rate:", "Colour:" };
                List<string> currentVehicle = vehicleToModify.GetAttributList();
                string stringtoDisplay;
                int numOfIterations = 6;


                //While the user is making edits to current vehicle
                while (inputting)
                {
                    //General Display For the modify screen
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Modify Vehicle");
                    for (int iterator = 1; iterator <= numOfIterations; iterator++)
                    {
                        int dataRetrival = iterator - 1;
                        stringtoDisplay = string.Format("{0,-2}){4,-15}{1,-15}{2,-2}){5,-15}{3,-15}",
                                                       iterator, currentVehicle[dataRetrival], iterator + 6, currentVehicle[dataRetrival + 6], placeHolder[dataRetrival], placeHolder[dataRetrival + 6]);
                        BasicBody();
                        BasicBody(stringtoDisplay);
                    }
                    BasicBody();
                    //When saving this will trigger the confirmation page
                    if (!editing & saving)
                    {
                        if (!error)
                        {
                            BasicBody();
                            BasicBody("Are you sure these changes are correct");
                            BasicBody("(Y) Yes / (N) No");
                            bottomScreenBackfill(16);
                        }
                        else
                        {
                            BasicBody();
                            BasicBody("That is not a valid input", "!");
                            BasicBody("Are you sure these changes are correct");
                            BasicBody("(Y) Yes / (N) No");
                            bottomScreenBackfill(17);
                        }
                        Write(">>>");
                        error = false;
                        userInput = ReadLine();
                        userInput = userInput.ToLower();
                        switch (userInput)
                        {
                            case "y":
                                if (changes)
                                {
                                    if (vehicleToModify != null)
                                    {
                                        vehicleToModify.UpdateExisting(currentVehicle);
                                    }
                                    if (newVehicle)
                                    {
                                        mainProgram.fleetMain.AddVehicle(vehicleToModify);
                                    }
                                }
                                saving = false;
                                inputting = false;
                                break;
                            case "n":
                                saving = false;
                                break;
                            default:
                                error = true;
                                break;
                        };
                    }
                    //When editing a specific field within the vehicle
                    else if (editing & !saving)
                    {
                        //Error messages to appear according to the edit field
                        if (error)
                        {
                            switch (option + 1)
                            {
                                case (int)ValidInputsAdjustVeh.REGO:
                                    BasicBody("Regestration needs to be 3 Numbers then 3 letters", "!");
                                    break;
                                case (int)ValidInputsAdjustVeh.GRADE:
                                case (int)ValidInputsAdjustVeh.TRANSMISSION:
                                case (int)ValidInputsAdjustVeh.FUELTYPE:
                                    BasicBody("Please Enter one of the specified fields", "!");
                                    break;
                                case (int)ValidInputsAdjustVeh.YEAR:
                                    BasicBody("That is not a year", "!");
                                    break;
                                case (int)ValidInputsAdjustVeh.SEATS:
                                    BasicBody("Please enter a number", "!");
                                    break;
                                default:
                                    BasicBody("That is not a valid input", "!");
                                    break;
                            }
                        }
                        else
                        {
                            BasicBody();
                        };
                        //Displays current field that is being editied
                        stringtoDisplay = string.Format("Currently Editing:{0,10}{1,-15}", placeHolder[option], currentVehicle[option]);
                        BasicBody(stringtoDisplay);
                        //Specific options requried for specific fields ideally the Grades and transmission
                        switch (option + 1)
                        {
                            case (int)ValidInputsAdjustVeh.GRADE:
                                BasicBody("Please Enter (E)Economy/(F)Family/(C)Commercial/(L)Luxry");
                                bottomScreenBackfill(15);
                                break;
                            case (int)ValidInputsAdjustVeh.TRANSMISSION:
                                BasicBody("Please Enter (M)Manual/(A)Automatic");
                                bottomScreenBackfill(15);
                                break;
                            case (int)ValidInputsAdjustVeh.FUELTYPE:
                                BasicBody("Please Enter (P)Petrol/(D)Diesel");
                                bottomScreenBackfill(15);
                                break;
                            case (int)ValidInputsAdjustVeh.GPS:
                            case (int)ValidInputsAdjustVeh.SUNROOF:
                                BasicBody("Enter (Y) Yes or (N) No");
                                bottomScreenBackfill(15);
                                break;
                            default:
                                bottomScreenBackfill(14);
                                break;
                        };
                        Write(">>>");
                        error = false;
                        userInput = ReadLine();
                        //Passes through input to check if the field in question is the correct input
                        error = CheckifValidVeh(userInput, option);
                        if (userInput.ToLower() == "b")
                        {
                            editing = false;
                            error = false;
                        }
                        //Makes changes according to the input field if no error are present
                        if (!error)
                        {
                            string toChange = "";
                            switch (option + 1)
                            {
                                case (int)ValidInputsAdjustVeh.GRADE:
                                    switch (userInput.ToLower())
                                    {
                                        case "c":
                                            toChange = "Commercial";
                                            break;
                                        case "l":
                                            toChange = "Luxury";
                                            break;
                                        case "e":
                                            toChange = "Economy";
                                            break;
                                        case "f":
                                            toChange = "Family";
                                            break;
                                    };
                                    currentVehicle[option] = toChange;
                                    break;
                                case (int)ValidInputsAdjustVeh.TRANSMISSION:
                                    switch (userInput.ToLower())
                                    {
                                        case "a":
                                            toChange = "Automatic";
                                            break;
                                        case "m":
                                            toChange = "Manual";
                                            break;
                                    };
                                    currentVehicle[option] = toChange;
                                    break;
                                case (int)ValidInputsAdjustVeh.FUELTYPE:
                                    switch (userInput.ToLower())
                                    {
                                        case "p":
                                            toChange = "Petrol";
                                            break;
                                        case "d":
                                            toChange = "Diesel";
                                            break;
                                    };
                                    currentVehicle[option] = toChange;
                                    break;
                                case (int)ValidInputsAdjustVeh.GPS:
                                case (int)ValidInputsAdjustVeh.SUNROOF:
                                    switch (userInput.ToLower())
                                    {
                                        case "y":
                                            toChange = "Yes";
                                            break;
                                        case "n":
                                            toChange = "No";
                                            break;
                                    };
                                    currentVehicle[option] = toChange;
                                    break;
                                case (int)ValidInputsAdjustVeh.RATE:
                                    double value = double.Parse(userInput);
                                    userInput = string.Format("{0:C2}", value);
                                    currentVehicle[option] = userInput;
                                    break;
                                default:
                                    currentVehicle[option] = userInput;
                                    break;
                            };
                            changes = true;
                            editing = false;
                        };
                    }
                    //Selecting an input field to edit
                    else
                    {
                        if (error)
                        {
                            BasicBody("That is not a valid input", "!");
                        }
                        else
                        {
                            BasicBody();
                        };
                        BasicBody("Enter the number of the field you wish to edit");
                        BasicBody("When ready type S to Save when you wish to go Back type B");
                        BasicBody("(Warning this will delete any changes made)", "!");
                        bottomScreenBackfill(16);
                        Write(">>>");
                        error = false;
                        userInput = ReadLine().ToLower();
                        //checks the input is correct
                        bool interger = Int32.TryParse(userInput, out option);
                        if (interger)
                        {
                            error = !Enum.IsDefined(typeof(ValidInputsAdjustVeh), option);
                            if (!error)
                            {
                                error = false;
                                editing = true;
                                option = option - 1;
                            }
                        }
                        else
                        {
                            switch (userInput)
                            {
                                case "s":
                                    saving = true;
                                    error = false;
                                    break;
                                case "b":
                                    inputting = false;
                                    error = false;
                                    break;
                                default:
                                    error = true;
                                    break;
                            };
                        };
                    };
                };
            };
            //When everything has been modified and has been succesful this displays
            if (saving)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("CUSTOMER SUCCESFULLY SAVED");
                BasicBody();
                BasicBody();
                BasicBody("Vehicle Succesfully Saved");
                bottomScreenBackfill(6);
                Write(">>>");
                ReadLine();
            };
            VehicleManagmentInitialScreen();
        }


        //This will be the search screen of the vehicle database
        //The boolean variable is to 
        public static Vehicle SearchVehicles(bool ifRenting = false)
        {
            //Variables used throughout
            string currentPhraseToCheck;
            bool inputting = true;
            int[] errorPresent = new int[] { 0, 0 };

            while (inputting)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                }
                Heading("Search Engine");
                BasicBody();
                BasicBody();
                BasicBody("Please enter your query, type in B to return to the previous screen");
                BasicBody();
                if (errorPresent[0] == 1)
                {
                    if (errorPresent[1] == 1)
                    {
                        BasicBody("That contains a mismatch of parenthesis", "!");
                    }
                    else
                    {
                        BasicBody("If you are using AND or OR please put something to the left and right", "!");
                    }
                }
                else
                {
                    BasicBody();
                }
                bottomScreenBackfill(8);
                Write(">>>");
                currentPhraseToCheck = Console.ReadLine();
                errorPresent = CheckQuery(currentPhraseToCheck);
                if (errorPresent[0] == 0)
                {
                    if (currentPhraseToCheck == "")
                    {
                        ListofInformation(false);
                    }
                    else
                    {
                        List<Vehicle> gatheredInformation = searchingAlgorithim.PARSERequest(currentPhraseToCheck);
                        if (ifRenting)
                        {
                            return ListOfResults(gatheredInformation, ifRenting);
                        }
                        else
                        {
                            ListOfResults(gatheredInformation);
                            return null;
                        };
                    }
                }
            }
            return null;
        }


        //Most of this code is reused from class ListofInformation
        //Takes the input of the reultant list as the query has been rifined by shuntingyard method
        //The secoundary pass through is for the renting screen
        public static Vehicle ListOfResults(List<Vehicle> informationToDisplay, bool renting = false)
        {
            //Decide weather or not more pages are needed
            int currentLength = informationToDisplay.Count;
            int trueLength = informationToDisplay.Capacity;
            int maximumInfoVeh = screenSize - 5;
            int option;
            int iterator;
            bool morepages = currentLength > maximumInfoVeh;
            bool inputting = true;
            bool error = false;
            bool empty = currentLength == 0;
            //If more pages arn't required
            if (!morepages & !empty)
            {
                while (inputting)
                {
                    iterator = 1;
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    //Prints List of vehicles
                    Heading("Vehicle List");
                    BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);

                    foreach (Vehicle currentVehicle in informationToDisplay)
                    {
                        string Print;
                        List<string> toBePrinted = currentVehicle.GetAttributList();
                        string modelLength = Convert.ToString(30 - toBePrinted[2].Length);
                        Print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                iterator, toBePrinted[0], toBePrinted[1], toBePrinted[2], toBePrinted[3], toBePrinted[5], toBePrinted[6], toBePrinted[7], toBePrinted[8], toBePrinted[9], toBePrinted[11], toBePrinted[10]);
                        iterator++;
                        BasicBody(Print, "*", false);
                    };
                    BasicBody();
                    if (error)
                    {
                        BasicBody("Please enter a valid input", "!");
                    }
                    else
                    {
                        BasicBody();
                    };

                    BasicBody("Select the number of Vehicle to be Modified");
                    BasicBody("Enter (B) Back to return to the previous menu, or (C) to change the query");
                    bottomScreenBackfill((iterator) + 4);
                    Write(">>>");
                    string userInput = ReadLine();
                    error = ListInputChecks(userInput);
                    if (!error)
                    {
                        bool interger = Int32.TryParse(userInput, out option);
                        if (!interger)
                        {
                            inputting = false;
                            if (userInput.ToLower() == "b")
                            {
                                VehicleManagmentInitialScreen();
                            }
                            else if (userInput.ToLower() == "c")
                            {
                                SearchVehicles();
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        else
                        {
                            inputting = false;
                            {
                                Vehicle placeholder = informationToDisplay[option - 1];
                                string rego = placeholder.GetRego();
                                int i = 0;
                                foreach (Vehicle toCheck in mainProgram.fleetMain.GetVehicles())
                                {
                                    List<string> vehicleInfo = toCheck.GetAttributList();
                                    bool match = vehicleInfo.Contains(rego);
                                    if (match)
                                    {

                                        break;
                                    }
                                    else
                                    {
                                        i++;
                                    };
                                }
                                if (renting)
                                {
                                    return mainProgram.fleetMain.GetVehicle(i);
                                }
                                else
                                {
                                    VehicleModifyScreen(mainProgram.fleetMain.GetVehicle(i));
                                    return null;
                                };
                            };
                        };
                    }
                };
            }
            else if (currentLength == 0)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("Vehicle List");
                BasicBody();
                BasicBody();
                BasicBody("No Vehicles to Display please Add or load vehicles to Database", "!");
                bottomScreenBackfill(6);
                Write(">>>");
                ReadLine();
                if (renting)
                {
                    return null;
                }
                else
                {
                    VehicleManagmentInitialScreen();
                    return null;
                };
            }
            else
            {
                //Variables for use throughout the display of this system
                List<Vehicle> vehicleList = mainProgram.fleetMain.GetVehicles();
                Vehicle toBePrinted;
                List<string> vehicleINFO = new List<string> { };
                int currentloop = 0;
                string print;
                int lineNO;
                decimal noPages = decimal.Ceiling(Convert.ToDecimal(trueLength) / Convert.ToDecimal(maximumInfoVeh));

                //While vehicle is getting retrived to be printed in an apporirate format
                while (inputting)
                {
                    lineNO = 5;
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Vehicle Lists");
                    BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);
                    for (int importantIterator = 0; importantIterator < maximumInfoVeh; importantIterator++)
                    {
                        toBePrinted = vehicleList[importantIterator + currentloop * maximumInfoVeh];
                        vehicleINFO = toBePrinted.GetAttributList();
                        string modelLength = Convert.ToString(30 - vehicleINFO[2].Length);
                        print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                    importantIterator + 1, vehicleINFO[0], vehicleINFO[1], vehicleINFO[2], vehicleINFO[3], vehicleINFO[5], vehicleINFO[6], vehicleINFO[7], vehicleINFO[8], vehicleINFO[9], vehicleINFO[11], vehicleINFO[10]);
                        BasicBody(print, "*", false);
                        lineNO++;
                        if ((importantIterator + 1) + currentloop * maximumInfoVeh == trueLength)
                        {
                            importantIterator = maximumInfoVeh;
                        };
                    };
                    string pageINFO = string.Format("Page {0} of {1}", currentloop + 1, noPages);
                    BasicBody(pageINFO);
                    BasicBody("Press N for the next page or P for the previous Page");
                    BasicBody("Enter the number to Modify/Delete or enter B to go back to the previous menu");
                    if (error)
                    {
                        ErrorMessage((int)ErrorType.InvalidInput);
                        error = false;
                    }
                    else
                    {

                        BasicBody();
                    };
                    bottomScreenBackfill(lineNO);
                    Write(">>>");
                    string user = ReadLine();
                    //Checks input is correct
                    error = ListInputChecks(user);
                    //If no error occurs
                    if (!error)
                    {
                        bool interger = Int32.TryParse(user, out int dataPoint);
                        if (!interger)
                        {
                            switch (user.ToLower())
                            {
                                case "n":
                                    currentloop++;
                                    break;
                                case "p":
                                    currentloop--;
                                    break;
                                case "b":
                                    inputting = false;
                                    VehicleManagmentInitialScreen();
                                    break;
                                case "c":
                                    inputting = false;
                                    SearchVehicles();
                                    break;
                            };
                        }
                        else
                        {
                            inputting = false;
                            Vehicle vehicleToPass = mainProgram.fleetMain.GetVehicle((dataPoint - 1) + currentloop * maximumInfoVeh);
                            string vehicleIDToPASS = Convert.ToString((dataPoint - 1) + currentloop * maximumInfoVeh);
                            if (renting)
                            {
                                return vehicleToPass;
                            }
                            else
                            {
                                VehicleModifyScreen(vehicleToPass);
                                return null;
                            }
                        }
                    };
                };
            };
            return null;
        }

        //Private class that checks the search query
        //Returns an array detailing the error type
        private static int[] CheckQuery(string pharse)
        {
            pharse += " ";
            int[] errorDetails = new int[2];
            Regex braketsL = new Regex(@"[(]");
            Regex braketsR = new Regex(@"[)]");
            MatchCollection bracketsLEFT = braketsL.Matches(pharse);
            MatchCollection bracketsRIGHT = braketsR.Matches(pharse);
            bool check1 = bracketsLEFT.Count != bracketsRIGHT.Count;
            bool allreusults = pharse == " ";
            if (!allreusults)
            {
                if (check1)
                {
                    errorDetails = new int[2] { 1, 1 };
                    return errorDetails;
                }
                else
                {
                    List<string> searchTerms = new List<string> { };
                    bool bracketsPOS;
                    bool optionsPOS;
                    string substring = "";

                    int length = pharse.Length - 1;

                    //Regular Expression to find matches of the and, or and also finding brackets
                    Regex optionsSearch = new Regex(@"( OR )|( AND )|( or )|( and )");

                    MatchCollection optionsResults = optionsSearch.Matches(pharse);

                    List<int> optionsposition = new List<int> { };

                    foreach (Match optionsMatch in optionsResults)
                    {
                        optionsposition.Add(optionsMatch.Index);
                    };

                    Regex Brakcets = new Regex(@"[()]");

                    MatchCollection bracketsResult = Brakcets.Matches(pharse);

                    List<int> bracketsINDEX = new List<int> { };

                    foreach (Match bracketMatch in bracketsResult)
                    {
                        bracketsINDEX.Add(bracketMatch.Index);
                    };



                    for (int i = 0; i <= length; i++)
                    {
                        bracketsPOS = bracketsINDEX.Contains(i);
                        optionsPOS = optionsposition.Contains(i);
                        if (bracketsPOS)
                        {
                            if (substring != "")
                            {
                                int len = substring.Length - 1;
                                if (substring[len].ToString() == " ")
                                {
                                    substring = substring.Remove(len, 1);
                                };
                                searchTerms.Add(substring);
                                substring = "";
                            }
                            searchTerms.Add(pharse.Substring(i, 1));
                        }
                        //This is for options
                        else if (optionsPOS)
                        {
                            int keywordLength = 0;
                            if (substring != "")
                            {
                                int len = substring.Length - 1;
                                if (substring[len].ToString() == " ")
                                {
                                    substring = substring.Remove(len, 1);
                                };
                                searchTerms.Add(substring);
                                substring = "";
                            }
                            //Deciding if the keyword is and or or
                            if (pharse.Substring(i + 1, 1) == "O" | pharse.Substring(i + 1, 1) == "o")
                            {
                                keywordLength = 3;
                            }
                            else
                            {
                                keywordLength = 4;
                            };
                            searchTerms.Add(pharse.Substring(i, keywordLength).Replace(" ", ""));
                            i += (keywordLength);

                        }
                        //This is for saving the word
                        else
                        {
                            substring += pharse.Substring(i, 1);
                        };
                    }
                    if (substring != "")
                    {
                        int len = substring.Length - 1;
                        {
                            substring = substring.Remove(len, 1);
                        };
                        searchTerms.Add(substring);
                        substring = "";
                    }
                    //Clean up
                    searchTerms.RemoveAll(searchingAlgorithim.ISEMPTY);

                    bool error = !(searchingAlgorithim.ConvertToTokens(searchTerms, true));
                    if (error)
                    {
                        errorDetails = new int[2] { 1, 2 };
                        return errorDetails;
                    }
                    else
                    {
                        errorDetails = new int[2] { 0, 0 };
                        return errorDetails;
                    }

                };
            }
            else
            {
                errorDetails = new int[2] { 0, 0 };
                return errorDetails;
            }
        }

        //This menu will handle all the renting aspects of the business
        //Making new rentals and deleting them and also searching for apporarte vehicles
        public static void RentingMain()
        {
            //Variables for inputting throughout the function
            bool inputting = true;
            int[] error = { 0, 0 };

            while (inputting)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                }
                Heading("Renting - Mangement");
                BasicBody();
                BasicBody();
                BasicBody("1) Make a new rental");
                BasicBody("2) Delete a rental");
                BasicBody("3) Currently Rented out Vehicles (Report)");
                BasicBody();
                if (error[0] != 0)
                {
                    ErrorMessage((int)ErrorType.IncorrectInput);
                }
                else
                {
                    BasicBody();
                };
                BasicBody();
                BasicBody("B) Previous Menu");
                bottomScreenBackfill(10);
                error = UserPrompt(Location.RentalMangament);
                switch (error[1])
                {
                    case (int)ValidInputsRenting.Back:
                        inputting = false;
                        MainScreen();
                        break;
                    case (int)ValidInputsRenting.New:
                        inputting = false;
                        //Only creating a new Rental in the database the default delete boolean variable is false
                        RentChange();
                        break;
                    case (int)ValidInputsRenting.Delete:
                        inputting = false;
                        ListReport(true);
                        //The rental is being deleted hence why the boolean variable is set to true
                        RentChange(true);
                        break;
                    case (int)ValidInputsRenting.Current:
                        inputting = false;
                        ListReport();
                        break;
                    default:
                        break;
                }
            };
        }

        //This is to create or delete rentals from the database
        //It takes a boolean variables to determine 
        public static void RentChange(bool deleting = false)
        {
            //Variables for use to hold information and confirm status of program
            customer custPlaceholder = null;
            Vehicle vehPlaceholder = null;
            bool informationCollected = false;
            bool finalConfirmation = false;
            bool error = false;
            string userInput;
            //Display Variables
            string[] custInformation = null;
            List<string> vehInformation = null;
            string customerFormate;
            string vehicleFormate;

            if (deleting)
            {
                Heading("Delete page");
                ReadLine();
            }
            else
            {
                //User selecting a piece of data to retrive
                while (!informationCollected)
                {
                    finalConfirmation = custPlaceholder != null & vehPlaceholder != null;
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Search Protocol");
                    BasicBody();
                    BasicBody();
                    BasicBody("1) Customer Details:");
                    if (custPlaceholder != null)
                    {
                        customerFormate = string.Format($"{custInformation[0]} {custInformation[1]} {custInformation[2]} DOB:{custInformation[4]}");
                        BasicBody(customerFormate);
                    }
                    else
                    {
                        BasicBody();
                    };
                    BasicBody();
                    BasicBody("2) Vehicle Details:");
                    if (vehPlaceholder != null)
                    {
                        //Displays rego, Grade, Make, Model, Daily Rate
                        vehicleFormate = string.Format($"{vehInformation[0]}    {vehInformation[1]}      {vehInformation[2]}      {vehInformation[3]}       {vehInformation[10]}");
                        BasicBody(vehicleFormate);
                    }
                    else
                    {
                        BasicBody();
                    };
                    BasicBody();

                    if (finalConfirmation)
                    {
                        BasicBody("Would you like to rent out the Car to the customer (Y) Yes or (N)");
                    }
                    else
                    {
                        BasicBody("Enter the number of the information you would like to retrive");
                    };
                    BasicBody("Enter B when ready to return to the previous screen");
                    BasicBody();
                    if (error)
                    {
                        BasicBody("There was an error with that response", "!");
                    }
                    else
                    {
                        BasicBody();
                    }
                    bottomScreenBackfill(15);
                    Write(">>>");
                    userInput = ReadLine();
                    if (!finalConfirmation)
                    {
                        switch (userInput.ToLower())
                        {
                            case "1":
                                error = false;
                                custPlaceholder = RentalGrabCust();
                                custInformation = custPlaceholder.GetEverything();
                                break;
                            case "2":
                                error = false;
                                vehPlaceholder = RentalGrabVeh();
                                vehInformation = vehPlaceholder.GetAttributList();
                                break;
                            case "b":
                                error = false;
                                RentingMain();
                                break;
                            default:
                                error = true;
                                break;
                        };
                    }
                    else
                    {
                        switch (userInput.ToLower())
                        {
                            case "y":
                                error = false;
                                mainProgram.fleetMain.COMMITRENT(vehPlaceholder, custPlaceholder);
                                informationCollected = true;
                                break;
                            case "n":
                                informationCollected = false;
                                vehPlaceholder = null;
                                custPlaceholder = null;
                                error = false;
                                break;
                            case "b":
                                error = false;
                                RentingMain();
                                break;
                            default:
                                error = true;
                                break;
                        };
                    }
                };
            };
            RentingMain();
        }

        //This is a break down of the ListofInformation method rather then redesinging it
        //Returns the selected customer
        public static customer RentalGrabCust()
        {
            bool ifRenting = false;
            //Initilse the variables for use throughout the method
            int customerDatabaseID = mainProgram.CRMMain.CustDatabaseCap();
            int customerDatabaseLength = mainProgram.CRMMain.numOfEntrys();
            int missingData = customerDatabaseID - customerDatabaseLength;
            int maximuminfoCUST = screenSize - 5;
            int currentLoops = 0;
            decimal numOfPagesCust = decimal.Ceiling(Convert.ToDecimal(customerDatabaseLength) / Convert.ToDecimal(maximuminfoCUST));
            string toBePrinted;
            bool error = false;
            bool inputs = true;
            int option = 0;
            string[] custINFO;
            //If more pages are required then the formating will be different
            bool morepages = customerDatabaseLength > maximuminfoCUST;
            bool Empty = customerDatabaseLength == 0;
            if (!morepages & !Empty)
            {
                while (inputs)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Select Customer");
                    BasicBody("      Title:       First:           Last:             DOB:");
                    int listIterator = 0;
                    foreach (customer indviudalCust in mainProgram.CRMMain.GetCustomers())
                    {
                        listIterator++;
                        //Retrives the current customer information and puts it in array
                        custINFO = indviudalCust.GetEverything();
                        //Fromates the string to align to the screen
                        toBePrinted = String.Format("{0,-3}){1,8}{2,15}{3,15}{4,20}", listIterator, custINFO[0], custINFO[1], custINFO[2], custINFO[4]);
                        BasicBody(toBePrinted);

                    };
                    BasicBody();
                    BasicBody("Input the number to select the customer");
                    BasicBody("Enter B to back out of this screen to the previous screen");
                    if (error)
                    {
                        BasicBody("That is not a valid input", "!");
                        listIterator += 6;
                    }
                    else if (ifRenting)
                    {
                        BasicBody("That customer is already renting a vehicle", "!");
                        ifRenting = false;
                        listIterator += 6;
                    }
                    else
                    {
                        listIterator += 5;
                    };
                    bottomScreenBackfill(listIterator);
                    Write(">>>");
                    string userIn = ReadLine();
                    //Check inputs if it is valid in this area
                    error = ListInputChecks(userIn);
                    if (!error)
                    {
                        bool interger = Int32.TryParse(userIn, out option);
                        if (interger)
                        {
                            //Passes customer to modify screen
                            option--;
                            ifRenting = mainProgram.fleetMain.checkIfRenting(true, option.ToString());
                            if (ifRenting)
                            {
                                inputs = true;
                            }
                            else
                            {
                                inputs = false;
                            };
                        }
                        else
                        {
                            switch (userIn.ToLower())
                            {
                                case "b":
                                    inputs = false;
                                    RentChange();
                                    break;
                            };
                        }
                    };
                };
            }
            else if (customerDatabaseLength == 0)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                };
                Heading("List of Customers");
                BasicBody();
                BasicBody();
                BasicBody("No Customers to Display please Load or Add new customers", "!");
                bottomScreenBackfill(6);
                Write(">>>");
                ReadLine();
                CustomerManagmentInitialScreen();
            }
            else
            {
                while (inputs)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    //Multiple Pages to display
                    Heading("Select Customer");
                    string heading2 = string.Format("      Title:       First:           Last:             DOB:      Page: {0} of {1}", currentLoops + 1, numOfPagesCust);
                    BasicBody(heading2);
                    //Only displays the number of entrys of the maximum entrys specified
                    for (int iterator = 0; iterator < maximuminfoCUST + missingData; iterator++)
                    {
                        int currentRecord = iterator + maximuminfoCUST * currentLoops;
                        customer currentCUST = mainProgram.CRMMain.GetCustomer(currentRecord);
                        if (currentCUST != null)
                        {
                            custINFO = currentCUST.GetEverything();
                            toBePrinted = String.Format("{0,-3}){1,8}{2,15}{3,15}{4,20}", iterator + 1, custINFO[0], custINFO[1], custINFO[2], custINFO[4]);
                            BasicBody(toBePrinted);
                        }
                        if (currentRecord == customerDatabaseLength - 1)
                        {
                            for (int emptyspaces = iterator; emptyspaces < maximuminfoCUST; emptyspaces++)
                            {
                                BasicBody();
                            };
                            iterator = maximuminfoCUST;
                        };
                    }
                    BasicBody();
                    BasicBody("Input the number to modify the customer");
                    BasicBody("Enter B to back out of this screen to the previous screen");
                    BasicBody("Enter N for the Next Page or P for the previous Page");
                    bottomScreenBackfill(20);
                    Write(">>>");
                    string userIn = ReadLine();
                    error = ListInputChecks(userIn);
                    if (!error)
                    {
                        bool interger = Int32.TryParse(userIn, out option);
                        if (interger)
                        {
                            option = (option - 1) + currentLoops * maximuminfoCUST;
                            ifRenting = mainProgram.fleetMain.checkIfRenting(true, option.ToString());
                            if (ifRenting)
                            {
                                inputs = true;
                            }
                            else
                            {
                                inputs = false;
                            };
                        }
                        else
                        {
                            switch (userIn.ToLower())
                            {
                                case "b":
                                    inputs = false;
                                    RentChange();
                                    break;
                                case "n":
                                    currentLoops++;
                                    break;
                                case "p":
                                    currentLoops--;
                                    break;
                            };
                        }
                    };
                }
            };
            return mainProgram.CRMMain.GetCustomer(option);

        }

        //This is a break down of the ListofInformation method rather then redesinging it
        //Returns the selected vehicle
        public static Vehicle RentalGrabVeh()
        {
            //First part is selecting the search method
            //Varibles for use in the first part
            bool selectingSearch = true;
            Vehicle toBeReturned = null;
            bool errorSearch = false;
            bool listMethod = false;
            bool gotVEH = true;
            string userInputSelc;
            //Check if renting
            bool ifRENTING = false;
            string REGO;

            while (gotVEH)
            {
                while (selectingSearch)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Search Method");
                    BasicBody();
                    BasicBody();
                    BasicBody("1) Query");
                    BasicBody("2) List");
                    BasicBody();
                    if (errorSearch)
                    {
                        BasicBody("That is not a valid input", "!");
                    }
                    else if (ifRENTING)
                    {
                        BasicBody("That vehicle is currently being rented", "!");
                    }
                    else
                    {
                        BasicBody();
                    };
                    BasicBody();
                    BasicBody("Enter the number of the way in which you would like to retrive the vehicle or (B) to go back");
                    bottomScreenBackfill(11);
                    Write(">>>");
                    userInputSelc = ReadLine();
                    switch (userInputSelc.ToLower())
                    {
                        case "1":
                            errorSearch = false;
                            selectingSearch = false;
                            listMethod = false;
                            break;
                        case "2":
                            errorSearch = false;
                            selectingSearch = false;
                            listMethod = true;
                            break;
                        case "b":
                            errorSearch = false;
                            RentChange();
                            break;
                        default:
                            errorSearch = true;
                            break;
                    };
                };

                if (!listMethod)
                {
                    //This is looking for a vehicle 
                    toBeReturned = SearchVehicles(true);
                    REGO = toBeReturned.GetRego();
                    ifRENTING = mainProgram.fleetMain.checkIfRenting(true, REGO, false);
                    if (ifRENTING)
                    {
                        selectingSearch = true;
                        errorSearch = false;
                        listMethod = false;
                    }
                }
                else
                {
                    //Decide weather or not more pages are needed
                    int currentLength = mainProgram.fleetMain.VehicleCurrentAmount();
                    int trueLength = mainProgram.fleetMain.VehcileCapacity();
                    int maximumInfoVeh = screenSize - 5;
                    int option;
                    int iterator = 1;
                    bool morepages = currentLength > maximumInfoVeh;
                    bool inputting = true;
                    bool error = false;
                    bool empty = currentLength == 0;
                    //If more pages arn't required
                    if (!morepages & !empty)
                    {
                        while (inputting)
                        {
                            if (mainProgram.NOTRecording)
                            {
                                Clear();
                            };
                            //Prints List of vehicles
                            Heading("Vehicle List");
                            BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);

                            foreach (Vehicle currentVehicle in mainProgram.fleetMain.GetVehicles())
                            {
                                string Print;
                                List<string> toBePrinted = currentVehicle.GetAttributList();
                                string modelLength = Convert.ToString(30 - toBePrinted[2].Length);
                                Print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                        iterator, toBePrinted[0], toBePrinted[1], toBePrinted[2], toBePrinted[3], toBePrinted[5], toBePrinted[6], toBePrinted[7], toBePrinted[8], toBePrinted[9], toBePrinted[11], toBePrinted[10]);
                                iterator++;
                                BasicBody(Print, "*", false);
                            };
                            BasicBody();
                            if (error)
                            {
                                BasicBody("Please enter a valid input", "!");
                            }
                            else
                            {
                                BasicBody();
                            };

                            BasicBody("Select the number of Vehicle to be Modified");
                            BasicBody("Eneter (B) Back to return to the previous menu");
                            bottomScreenBackfill((iterator) + 4);
                            Write(">>>");
                            string userInput = ReadLine();
                            error = ListInputChecks(userInput);
                            if (!error)
                            {
                                bool interger = Int32.TryParse(userInput, out option);
                                if (!interger)
                                {
                                    inputting = false;
                                    RentChange();
                                }
                                else
                                {
                                    inputting = false;
                                    toBeReturned = mainProgram.fleetMain.GetVehicle(option - 1);
                                    REGO = toBeReturned.GetRego();
                                    ifRENTING = mainProgram.fleetMain.checkIfRenting(true, REGO, false);
                                    if (ifRENTING)
                                    {
                                        selectingSearch = true;
                                        errorSearch = false;
                                        listMethod = false;
                                    }
                                    else
                                    {
                                        gotVEH = false;
                                    };
                                };
                            }
                        };
                    }
                    else if (currentLength == 0)
                    {
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Vehicle List");
                        BasicBody();
                        BasicBody();
                        BasicBody("No Vehicles to Display please Add or load vehicles to Database", "!");
                        bottomScreenBackfill(6);
                        Write(">>>");
                        ReadLine();
                        RentChange();
                    }
                    else
                    {
                        //Variables for use throughout the display of this system
                        List<Vehicle> vehicleList = mainProgram.fleetMain.GetVehicles();
                        Vehicle toBePrinted;
                        List<string> vehicleINFO = new List<string> { };
                        int currentloop = 0;
                        string print;
                        int lineNO;
                        decimal noPages = decimal.Ceiling(Convert.ToDecimal(trueLength) / Convert.ToDecimal(maximumInfoVeh));

                        //While vehicle is getting retrived to be printed in an apporirate format
                        while (inputting)
                        {
                            lineNO = 5;
                            if (mainProgram.NOTRecording)
                            {
                                Clear();
                            }
                            Heading("Vehicle Lists");
                            BasicBody("    REGO:    Grade:      Make:            Model:           Seats:  Transmission:  Fuel:  GPS:  SunRoof: Colour: Rate:", "*", false);
                            for (int importantIterator = 0; importantIterator < maximumInfoVeh; importantIterator++)
                            {
                                toBePrinted = vehicleList[importantIterator + currentloop * maximumInfoVeh];
                                vehicleINFO = toBePrinted.GetAttributList();
                                string modelLength = Convert.ToString(30 - vehicleINFO[2].Length);
                                print = string.Format(" {0,-3}){1,-8}{2,-12}{3}{4," + modelLength + "}{5,10}{6,15}{7,8}{8,5}{9,7}{10,9} {11}",
                                                            importantIterator + 1, vehicleINFO[0], vehicleINFO[1], vehicleINFO[2], vehicleINFO[3], vehicleINFO[5], vehicleINFO[6], vehicleINFO[7], vehicleINFO[8], vehicleINFO[9], vehicleINFO[11], vehicleINFO[10]);
                                BasicBody(print, "*", false);
                                lineNO++;
                                if ((importantIterator + 1) + currentloop * maximumInfoVeh == trueLength)
                                {
                                    importantIterator = maximumInfoVeh;
                                };
                            };
                            string pageINFO = string.Format("Page {0} of {1}", currentloop + 1, noPages);
                            BasicBody(pageINFO);
                            BasicBody("Press N for the next page or P for the previous Page");
                            BasicBody("Enter the number to Modify/Delete or enter B to go back to the previous menu");
                            if (error)
                            {
                                ErrorMessage((int)ErrorType.InvalidInput);
                                error = false;
                            }
                            else
                            {

                                BasicBody();
                            };
                            bottomScreenBackfill(lineNO);
                            Write(">>>");
                            string user = ReadLine();
                            //Checks input is correct
                            error = ListInputChecks(user);
                            //If no error occurs
                            if (!error)
                            {
                                bool interger = Int32.TryParse(user, out int dataPoint);
                                if (!interger)
                                {
                                    switch (user.ToLower())
                                    {
                                        case "n":
                                            currentloop++;
                                            break;
                                        case "p":
                                            currentloop--;
                                            break;
                                        case "b":
                                            inputting = false;
                                            RentChange();
                                            break;
                                    };
                                }
                                else
                                {
                                    inputting = false;
                                    Vehicle vehicleToPass = mainProgram.fleetMain.GetVehicle((dataPoint - 1) + currentloop * maximumInfoVeh);
                                    toBeReturned = vehicleToPass;
                                    REGO = toBeReturned.GetRego();
                                    ifRENTING = mainProgram.fleetMain.checkIfRenting(true, REGO, false);
                                    if (ifRENTING)
                                    {
                                        selectingSearch = true;
                                        errorSearch = false;
                                        listMethod = false;
                                    }
                                    else
                                    {
                                        gotVEH = false;
                                    }
                                };
                            };
                        };
                    };
                };
            };
            return toBeReturned;
        }

        //Produces a List of the the currently rented customers with vehicle information
        //The boolean variable that passses through determines if some elements are displayed
        public static void ListReport(bool delete = false)
        {
            //Variables for use throughout
            int currentLength = mainProgram.fleetMain.RentingLength();
            int maximumInfoRenting = screenSize - 4;
            bool needingMorePages = currentLength > maximumInfoRenting;
            bool inputting = true;
            bool error = false;
            int lineNumber = 4;
            bool empty = currentLength == 0;
            string userInput;
            //Display variables
            string headOFTable = "TITLE:  NAME:                    REGSTRATION:        GRADE:    MAKE:         RATE:";
            string informationFormate;
            //Place holder variables
            Dictionary<string, int> placeholder = mainProgram.fleetMain.ReturnRentals();
            int custID;
            string REGO;
            Vehicle vehPlaceholder;
            customer custPlaceholder;
            string[] custInformation;
            List<string> vehInformation;
            int maxValue;

            //When it is a value being deleted
            if(delete)
            {
                headOFTable = "    " + headOFTable;
                //When the list is empty
                if (empty)
                {
                    if (mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Rentals");
                    BasicBody();
                    BasicBody("There are currently no rentals in the database");
                    bottomScreenBackfill(5);
                    Write(">>>");
                    ReadLine();
                    RentChange();
                }
                else
                {
                    while (inputting)
                    {
                        lineNumber = 4;
                        maxValue = 0;
                        int option = 0;
                        if (mainProgram.NOTRecording)
                        {
                            Clear();
                        };
                        Heading("Rental Report");
                        BasicBody(headOFTable);
                        foreach (KeyValuePair<string, int> valuePair in placeholder)
                        {
                            //Grabs detials and fills it in
                            custID = valuePair.Value;
                            REGO = valuePair.Key;
                            custPlaceholder = mainProgram.CRMMain.GetCustomer(custID);
                            custInformation = custPlaceholder.GetEverything();
                            vehPlaceholder = mainProgram.fleetMain.GetVehicleRego(REGO);
                            vehInformation = vehPlaceholder.GetAttributList();
                            informationFormate = string.Format("{0,3}){1,5}{2,25}{3,10}{4,20}{5,15}{6,9}",
                                (option+1), custInformation[0], (custInformation[1] + " " + custInformation[2]), vehInformation[0], vehInformation[1], vehInformation[2], vehInformation[10]);
                            BasicBody(informationFormate);
                            lineNumber++;
                            option++;
                            maxValue++;
                        };
                        BasicBody();
                        BasicBody("Press B key to return or a number to delete the rental");
                        if(error)
                        {
                            BasicBody("That is not a valid input", "!");
                        }
                        else
                        {
                            BasicBody();
                        };
                        bottomScreenBackfill(lineNumber + 3);
                        Write(">>>");
                        userInput = ReadLine();
                        error = !(Int32.TryParse(userInput, out int trueOption));
                        //Checks the input is a correct inpuyt
                        if(error)
                        {
                            switch(userInput.ToLower())
                            {
                                case "b":
                                    error = false;
                                    inputting = false;
                                    break;
                                default:
                                    break;
                            };
                        }
                        else
                        {
                            bool withinRange = (trueOption >= 1) & (trueOption <= maxValue);
                            if (withinRange)
                            {
                                trueOption--;
                                KeyValuePair<string, int> toBeDeleted = placeholder.ElementAt(trueOption);
                                FinalConfirmation(toBeDeleted);
                                inputting = false;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    };
                };
            }
            else
            {
                if (empty)
                { 
                    if(mainProgram.NOTRecording)
                    {
                        Clear();
                    }
                    Heading("Rentals");
                    BasicBody();
                    BasicBody("There are currently no rentals in the database");
                    bottomScreenBackfill(5);
                    Write(">>>");
                    ReadLine();
                    RentChange();
                }
                else
                { 
                    if(mainProgram.NOTRecording)
                    {
                        Clear();
                    };
                    Heading("Rental Report");
                    BasicBody(headOFTable);
                    foreach (KeyValuePair<string, int> valuePair in placeholder)
                    {
                        custID = valuePair.Value;
                        REGO = valuePair.Key;
                        custPlaceholder = mainProgram.CRMMain.GetCustomer(custID);
                        custInformation = custPlaceholder.GetEverything();
                        vehPlaceholder = mainProgram.fleetMain.GetVehicleRego(REGO);
                        vehInformation = vehPlaceholder.GetAttributList();
                        informationFormate = string.Format("{0,5}{1,25}{2,10}{3,20}{4,15}{5,9}",
                            custInformation[0], (custInformation[1] + " " + custInformation[2]),vehInformation[0], vehInformation[1], vehInformation[2], vehInformation[10]);
                        BasicBody(informationFormate);
                        lineNumber++;
                    };
                    BasicBody();
                    BasicBody("Press Enter key to continue");
                    bottomScreenBackfill(lineNumber + 2);
                    Write(">>>");
                    ReadLine();
                };
            }
            RentingMain();
        }

        //This is the final comfirimation before the rental being delted
        public static void FinalConfirmation(KeyValuePair<string, int> RentalToDelete)
        {
            //Variables used throughout the method
            string REGO = RentalToDelete.Key;
            Vehicle TODELETE = mainProgram.fleetMain.GetVehicleRego(REGO);
            string vehicleFormate;
            List<string> vehInformation = TODELETE.GetAttributList();
            customer custTODELETE = mainProgram.CRMMain.GetCustomer(RentalToDelete.Value);
            string[] custInformation = custTODELETE.GetEverything();
            string customerFormate;
            bool error = false;
            bool inputting = true;
            bool deleting = false;
            string userInput;

            while (inputting)
            {
                if (mainProgram.NOTRecording)
                {
                    Clear();
                }
                Heading("Confirmation - Remove Rental");
                BasicBody();
                BasicBody("Customer Details:");
                customerFormate = string.Format($"{custInformation[0]} {custInformation[1]} {custInformation[2]} DOB:{custInformation[4]}");
                BasicBody(customerFormate);
                BasicBody();
                BasicBody("Currently Renting:");
                vehicleFormate = string.Format($"{vehInformation[0]}    {vehInformation[1]}      {vehInformation[2]}      {vehInformation[3]}       {vehInformation[10]}");
                BasicBody(vehicleFormate);
                BasicBody();
                if(error)
                {
                    BasicBody("That is not a valid input", "!");
                    error = false;
                }
                else
                {
                    BasicBody();
                }
                BasicBody("Is the rental you would like to delete? Yes (Y) or No (N)");
                bottomScreenBackfill(11);
                Write(">>>");
                userInput = ReadLine();
                switch (userInput.ToLower())
                {
                    case "y":
                        inputting = false;
                        deleting = true;
                        break;
                    case "n":
                        inputting = false;
                        break;
                    default:
                        error = true;
                        break;
                };
            };
            //Displays a final screen saying everything was succesfull
            if(deleting)
            {
                if(mainProgram.NOTRecording)
                {
                    Clear();
                }
                mainProgram.fleetMain.DELETERental(RentalToDelete);
                Heading("Deleted Record");
                BasicBody();
                BasicBody();
                BasicBody("The rental has been deleted from the database");
                bottomScreenBackfill(6);
                Write(">>>");
                ReadLine();
            };
        }
    }
}
