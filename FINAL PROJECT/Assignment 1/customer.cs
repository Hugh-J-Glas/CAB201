using System;
using System.Collections.Generic;
using System.Text;

namespace MRRC
{
    public class customer
    {
        //Initilising Variables
        private int customerID { get; set; }
        private int title { get; set; }
        private string firstName { get; set; }
        private string lastName { get; set; }
        private int gender { get; set; }
        private DateTime DOB { get; set; }

        public customer(int customerID,
                    string titleRAW,
                    string First,
                    string Last,
                    string genderRAW,
                    string DOBINput)
        {
            this.customerID = customerID;
            this.title = (int)Enum.Parse(typeof(Title), titleRAW);
            this.firstName = First;
            this.lastName = Last;
            this.gender = (int)Enum.Parse(typeof(Gender), genderRAW);
            this.DOB = DateTime.Parse(DOBINput);
        }

        //Retrives both the first name and last name of a particlar customer
        public string[] GetName()
        {
            string[] toReturn = new string[2];
            toReturn[0] = firstName;
            toReturn[1] = lastName;
            return toReturn;
        }

        //Sets the Id of the customer to the new ID
        public void SetID(int newID)
        {
            customerID = newID;
        }

        //Returns the Date of bith of the customer
        public DateTime GetDOB()
        {
            return this.DOB;
        }

        //Reuturns the ID of the customer
        public int GetID()
        {
            return this.customerID;
        }

        //This method returns all the information in a string array
        public string[] GetEverything()
        {
            string[] toReturn = new string[5];
            toReturn[0] = Enum.GetName(typeof(Title), this.title);
            toReturn[1] = GetName()[0];
            toReturn[2] = GetName()[1];
            toReturn[4] = (GetDOB().ToString()).Substring(0,10);
            toReturn[3] = Enum.GetName(typeof(Gender), this.gender);
            return toReturn;
        }

        //Updates the customer with the new information
        public bool UpdateCustomer(string[] newInformation)
        {
            this.title = (int)Enum.Parse(typeof(Title),newInformation[0]);
            this.firstName = newInformation[1];
            this.lastName = newInformation[2];
            this.DOB = DateTime.Parse(newInformation[4]);
            this.gender = (int)Enum.Parse(typeof(Gender), newInformation[3]);
            return true;
        }

        //This is to return a string in the proper CSV string for saving
        public string ToCSVString()
        {
            const string DELIM = ",";
            string toRETURN;
            toRETURN = customerID+DELIM+Enum.GetName(typeof(Title), title) + DELIM + firstName + DELIM + lastName + DELIM + Enum.GetName(typeof(Gender), gender) + DELIM + (GetDOB().ToString()).Substring(0, 10);
            return toRETURN;
        }

        //Enumartions for the Title and gender of the customer
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
    }
}
