using System;
using static System.Console;

namespace SOSCalc
{
    class Program
    {
        private static UserInterface UI = new UserInterface();
        private static string version = "1.0";
        static void Main(string[] args)
        {
            bool quitProgram = false;

            Clear();

            while (!quitProgram)
            {
                UI.Header(version);
                switch (UI.Menu())
                {
                    case 1:
                        bool quitCalculator = false;

                        while (!quitCalculator)
                        {
                            Clear();
                            UI.Header(version);
                            switch (UI.Calculator())
                            {
                                case "help":
                                    Clear();
                                    UI.Help();
                                    break;
                                case "func":
                                    Clear();
                                    UI.ViewFunctions();
                                    break;
                                case "vars":
                                    Clear();
                                    UI.ViewVariables();
                                    break;
                                case "back":
                                    Clear();
                                    quitCalculator = true;
                                    break;
                            }
                        }
                        break;
                    case 2:
                        bool quitFunctions = false;

                        while (!quitFunctions)
                        {
                            Clear();
                            UI.Header(version);
                            switch (UI.Functions())
                            {
                                case 1:
                                    Clear();
                                    UI.ViewFunctions();
                                    break;
                                case 2:
                                    Clear();
                                    UI.Header(version);
                                    UI.CreateFunction();
                                    break;
                                case 3:
                                    Clear();
                                    UI.Header(version);
                                    UI.DeleteFunction();
                                    break;
                                case 4:
                                    Clear();
                                    quitFunctions = true;
                                    break;
                            }
                        }
                        Clear();
                        break;
                    case 3:
                        bool quitVariables = false;

                        while (!quitVariables)
                        {
                            Clear();
                            UI.Header(version);
                            switch (UI.Variables())
                            {
                                case 1:
                                    Clear();
                                    UI.ViewVariables();
                                    break;
                                case 2:
                                    Clear();
                                    UI.Header(version);
                                    UI.CreateVariable();
                                    break;
                                case 3:
                                    Clear();
                                    UI.Header(version);
                                    UI.DeleteVariable();
                                    break;
                                case 4:
                                    Clear();
                                    quitVariables = true;
                                    break;
                            }
                        }
                        Clear();
                        Clear();
                        break;
                    case 4:
                        WriteLine("Bye!");
                        Thread.Sleep(1000);
                        Clear();
                        quitProgram = true;
                        break;
                }
            }
        }
    }
}