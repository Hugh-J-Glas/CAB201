using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.Data;
using static MRRC.mainProgram;

namespace MRRC
{
    public class searchingAlgorithim
    {
        //Place holder lists that contain all the information
        private static List<SearchParameters> postFix = new List<SearchParameters> { };
        private static List<SearchParameters> prefix = new List<SearchParameters> { };
        private static List<SearchParameters> operationStack = new List<SearchParameters> { };

        //This method takes string inputs and converts them to tokens to be
        //Sorted according to the shunting yard method
        public static List<Vehicle> PARSERequest(string RAWinput)
        {
            RAWinput += " ";
            //Clears everything out
            operationStack.Clear();
            prefix.Clear();
            postFix.Clear();
            //Variables used throughout the method
            List<string> searchTerms = new List<string> { };
            bool bracketsPOS;
            bool optionsPOS;
            string substring = "";

            int length = RAWinput.Length - 1;

            //Regular Expression to find matches of the and, or and also finding brackets
            //This is to help converting the strings into tokens
            Regex optionsSearch = new Regex(@"( OR )|( AND )|( or )|( and )");

            MatchCollection optionsResults = optionsSearch.Matches(RAWinput);

            List<int> optionsposition = new List<int> { };

            foreach (Match optionsMatch in optionsResults)
            {
                optionsposition.Add(optionsMatch.Index);
            };

            Regex Brakcets = new Regex(@"[()]");

            MatchCollection bracketsResult = Brakcets.Matches(RAWinput);

            List<int> bracketsINDEX = new List<int> { };

            foreach (Match bracketMatch in bracketsResult)
            {
                bracketsINDEX.Add(bracketMatch.Index);
            };

            //Using the regular expression it subdivides everything into strings for tokens
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
                    searchTerms.Add(RAWinput.Substring(i, 1));
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
                    if (RAWinput.Substring(i+1,1) == "O" | RAWinput.Substring(i+1,1) == "o")
                    {
                        keywordLength = 3;
                    }
                    else
                    {
                        keywordLength = 4;
                    };
                    searchTerms.Add(RAWinput.Substring(i, keywordLength).Replace(" ",""));
                    i += (keywordLength);

                }
                //This is for saving the word
                else
                {
                    substring += RAWinput.Substring(i, 1);
                };
            }
            //Finally in casse there is an empty
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
            //Clean up
            searchTerms.RemoveAll(ISEMPTY);

            //Goes through process of using shunting yard
            ConvertToTokens(searchTerms);
            ShuntingYard(prefix);

            List<Vehicle> toBeReturned = new List<Vehicle> { };
            toBeReturned = GetResults(postFix);

            return toBeReturned;
        }

        //Simply converts everthing into a friendly list to be passed throuhg to shunting yard
        //The other checking variable is for the screens class
        public static bool ConvertToTokens(List<string> passthrough, bool checking = false)
        {
            if (checking)
            {
                List<SearchParameters> placeHolder = new List<SearchParameters> { };
                foreach (string convertingString in passthrough)
                {
                    switch (convertingString)
                    {
                        case "(":
                            placeHolder.Add(new LeftParenthesis());
                            break;
                        case ")":
                            placeHolder.Add(new RightParenthesis());
                            break;
                        case "or":
                        case "and":
                        case "OR":
                        case "AND":
                            placeHolder.Add(new BinarayTokens(convertingString));
                            break;
                        default:
                            placeHolder.Add(new VALUEToken(convertingString));
                            break;
                    };
                }
                //Final confirmation that the query is correct
                bool correct = true;
                bool soleSearch = placeHolder.Count == 1;
                //If it is a single word check it is a value token i.e. red blue etc
                if(soleSearch)
                {
                    correct = placeHolder[0].GetType() == typeof(VALUEToken); 
                }
                //Multiple words in query
                else
                {
                    bool nextValueBeingOperator = false;

                    //Get rid of all brackets
                    placeHolder.RemoveAll(ISBRACKET);

                    int iterator = placeHolder.Count;
                    //The idea for this loop is that if all brackets of the query are removed 
                    //it would be value operator value operator value opertar
                    for (int i = 0; i <= iterator - 1; i++)
                    {
                        if (placeHolder[i].GetType() == typeof(VALUEToken) & !nextValueBeingOperator)
                        {
                            nextValueBeingOperator = true;
                        }
                        else if (placeHolder[i].GetType() == typeof(BinarayTokens) & nextValueBeingOperator)
                        {
                            nextValueBeingOperator = false;
                        }
                        else
                        {
                            correct = false;
                        }
                    }
                    if (!nextValueBeingOperator)
                    {
                        correct = false;
                    }
                }

                return correct;
            }
            else
            {
                foreach (string convertingString in passthrough)
                {
                    switch (convertingString)
                    {
                        case "(":
                            prefix.Add(new LeftParenthesis());
                            break;
                        case ")":
                            prefix.Add(new RightParenthesis());
                            break;
                        case "or":
                        case "and":
                        case "OR":
                        case "AND":
                            prefix.Add(new BinarayTokens(convertingString));
                            break;
                        default:
                            prefix.Add(new VALUEToken(convertingString));
                            break;
                    };
                };
                return true;
            };

        }

        //Converts the array using the shunting yard method in order to make searching for relvant results easier
        private static void ShuntingYard(List<SearchParameters> tobeOrganised)
        {
            //Variables checking through each cirteria of the shunting yard method
            bool ifValue;
            bool ifOperator;
            bool ifLParenthisis;
            bool ifRParenthisis;

            bool ifPopping;
            //Used for operators cireteria
            int Length = operationStack.Count;
            bool FirstCriteria;
            int lastIndex;
            bool secoundCriteria;
            bool precendenceCriteria;

            foreach (SearchParameters toBeShunted in tobeOrganised)
            {
                //Set all the Boolean variables for different funcitons
                ifValue = toBeShunted.GetType() == typeof(VALUEToken);
                ifOperator = toBeShunted.GetType() == typeof(BinarayTokens);
                ifLParenthisis = toBeShunted.GetType() == typeof(LeftParenthesis);
                ifRParenthisis = toBeShunted.GetType() == typeof(RightParenthesis);

                if (ifValue)
                {
                    push(toBeShunted);
                }
                else if (ifOperator)
                {
                    
                    ifPopping = true;
                    //Loop to push through operation stack if it was true
                    while (ifPopping)
                    {
                        Length = operationStack.Count;
                        lastIndex = operationStack.Count - 1;
                        FirstCriteria = Length == 0;
                        if (FirstCriteria)
                        {
                            ifPopping = false;
                        }
                        else
                        {
                            secoundCriteria = operationStack[lastIndex].GetType() != typeof(LeftParenthesis);

                            if (secoundCriteria)
                            {
                                BinarayTokens tempToken = (BinarayTokens)toBeShunted;
                                BinarayTokens toBeCompared = (BinarayTokens)operationStack[lastIndex];
                                precendenceCriteria = tempToken.Precdence > toBeCompared.Precdence;
                                if (precendenceCriteria)
                                {
                                    ifPopping = false;
                                }
                                else
                                {
                                    push(operationStack[lastIndex]);
                                    operationStack.RemoveAt(lastIndex);
                                };

                            }
                            else
                            {
                                ifPopping = false;
                            };

                        };
                    };

                    pull(toBeShunted);
                }
                else if (ifLParenthisis)
                {
                    pull(toBeShunted);
                }
                else if (ifRParenthisis)
                {
                    ifPopping = true;
                    //This is to organise 
                    while (ifPopping)
                    {
                        lastIndex = operationStack.Count - 1;
                        if (operationStack[lastIndex].GetType() == typeof(LeftParenthesis))
                        {
                            ifPopping = false;
                        };
                        if ((operationStack[lastIndex].GetType() != typeof(LeftParenthesis))
                         & (operationStack[lastIndex].GetType() != typeof(RightParenthesis)))
                        {
                            push(operationStack[lastIndex]);
                        };
                        operationStack.RemoveAt(lastIndex);
                        if(operationStack.Count == 0)
                        {
                            ifPopping = false;
                        }
                    };
                };
            };

            for (int iterator = operationStack.Count - 1; iterator >= 0; iterator--)
            {
                push(operationStack[iterator]);
            }
        }

        //This Retrives all the results according to the search pattern orgainsed in the search patter
        private static List<Vehicle> GetResults(List<SearchParameters> searchPattern)
        {
            List<Vehicle> toBeReturned = new List<Vehicle> { };
            List<Vehicle> SecondaryHolding = new List<Vehicle> { };
            List<SearchParameters> holdingPlace = new List<SearchParameters> { };

            for (int iterator = 0; iterator < postFix.Count; iterator++)
            {
                SearchParameters placeHolder = postFix[iterator];
                if (placeHolder.GetType() == typeof(VALUEToken))
                {
                    holdingPlace.Add(placeHolder);
                }
                else
                {
                    int holdingLength = holdingPlace.Count;
                    BinarayTokens toBeCompared;
                    if (holdingLength == 2)
                    {
                        if(toBeReturned.Count != 0)
                        {
                            SecondaryHolding = toBeReturned;
                        }
                        toBeCompared = (BinarayTokens)postFix[iterator];
                        toBeReturned = fleetMain.SearchThrough(Operator: toBeCompared, firstValue: (VALUEToken)holdingPlace[0], secoundValue: (VALUEToken)holdingPlace[1]);
                        holdingPlace.Clear();
                    }
                    else if (holdingLength == 0)
                    {
                        toBeCompared = (BinarayTokens)postFix[iterator];
                        toBeReturned = fleetMain.SearchThrough(Operator: toBeCompared, exisitngResults: toBeReturned, secoundResults: SecondaryHolding);
                    }
                    else
                    {
                        toBeCompared = (BinarayTokens)postFix[iterator];
                        toBeReturned = fleetMain.SearchThrough(Operator: toBeCompared, firstValue: (VALUEToken)holdingPlace[0], exisitngResults: toBeReturned);
                        holdingPlace.Clear();
                    }

                }
            }
            if(holdingPlace.Count != 0)
            {
                toBeReturned = fleetMain.SearchThrough(firstValue: (VALUEToken)holdingPlace[0]);
            }
            return toBeReturned;
        }

        //This is method used in methods to help clean up
        public static bool ISBRACKET(SearchParameters passThrough)
        {
            if(passThrough.GetType() == typeof(LeftParenthesis) | passThrough.GetType() == typeof(RightParenthesis))
            {
                return true;
            }
            else
            {
                return false;
            };
        }

        //For use in a remove all, clean up and spaces
        public static bool ISEMPTY(string pasthrough)
        {
            if (pasthrough == "" | pasthrough == " ")
            {
                return true;
            }
            else
            {
                return false;
            };
        }

        private static void pull(SearchParameters pulling)
        {
            operationStack.Add(pulling);
        }

        private static void push(SearchParameters pushed)
        {
            postFix.Add(pushed);
        }
    }

    //Create Sudo Parent to make Lists organistions easier
    public class SearchParameters
    {
    
    }

    //This class is for the Binaray operators 'AND' 'OR'
    public class BinarayTokens : SearchParameters
    {
        private string STRoperator;
        public int Precdence;
        private static int LOWER = 1;
        private static int HIGHER = 2;

        public BinarayTokens(string RAWoperator)
        {
            this.STRoperator = RAWoperator.ToUpper();
            if (RAWoperator == "AND")
            {
                this.Precdence = HIGHER;
            }
            else
            {
                this.Precdence = LOWER;
            };
        }

        public string OperatorName()
        {
            return STRoperator;
        }
    }


        //These classes is for parenthesis they don't need anything because
        //It will be dropped for postfix
    public class LeftParenthesis : SearchParameters
    {

    }

    public class RightParenthesis : SearchParameters
    {

    }


        //This is a class for the words USED
    public class VALUEToken : SearchParameters
    {
        private string RAWrequest;

        public VALUEToken(string request)
        {
            this.RAWrequest = request;
        }

        public string TERM()
        {
            return RAWrequest.ToLower();
        }
    }
}
