using System;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SOSCalc
{
    class StorageEntry
    {
        private string name = "";
        private string definition = "";

        // Konstruktor
        public StorageEntry(string name, string definition)
        {
            this.name = name;
            this.definition = definition;
        }
        // Egenskaper
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Definition
        {
            get { return definition; }
            set { definition = value; }
        }
    }

    class StorageList : List<StorageEntry>
    {

    }

    class Storage
    {
        // PROPERTIES
        private StorageList createdFunctions = new StorageList();
        private StorageList createdVariables = new StorageList();
        private Dictionary<string, Func<double, double>> nativeFunctions = new Dictionary<string, Func<double, double>>
        {
            {"log", Math.Log},
            {"exp", Math.Exp},
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"sqrt", Math.Sqrt}
        };
        private Dictionary<string, double> nativeVariables = new Dictionary<string, double>
        {
            {"PI", Math.PI},
            {"E", Math.E}
        };
        private Dictionary<string, string> protectedWords = new Dictionary<string, string>
        {
            {"func", "view functions"},
            {"vars", "view variables"},
            {"clear", "clear the current calculations"},
            {"back", "go back to the menu"},
            {"help", "get help"}
        };
        private string functionsFilePath = "functions.json";
        private string variablesFilePath = "variables.json";
        private string? jsonString;

        // METHODS
        public Storage()
        {
            LoadFromFile();
        }
        public (StorageList, StorageList) LoadFromFile()
        {
            // Create an instace of the StorageList for the functions and variables
            createdFunctions = new StorageList();
            createdVariables = new StorageList();

            // Check if the function and variable file exists
            if (File.Exists(functionsFilePath) && File.Exists(variablesFilePath))
            {
                // Create two instances of FileInfo
                FileInfo functionsFileInfo = new FileInfo(functionsFilePath);
                FileInfo variablesFileInfo = new FileInfo(variablesFilePath);

                // Check that the file isn't empty
                if (functionsFileInfo.Length != 0)
                {
                    // Konvert the json file to a string
                    jsonString = File.ReadAllText(functionsFilePath);

                    // Save the string to the createdFunctions List
                    createdFunctions = JsonSerializer.Deserialize<StorageList>(jsonString) ?? new StorageList();
                }

                // Check that the file isn't empty
                if (variablesFileInfo.Length != 0)
                {
                    // Konvert the json file to a string
                    jsonString = File.ReadAllText(variablesFilePath);

                    // Save the string to the createdVariables List
                    createdVariables = JsonSerializer.Deserialize<StorageList>(jsonString) ?? new StorageList();
                }
            }
            return (createdFunctions, createdVariables);
        }
        // Get the list with created functions
        public StorageList GetCreatedFunctions()
        {
            return createdFunctions;
        }
        // Get the list with created variables
        public StorageList GetCreatedVariables()
        {
            return createdVariables;
        }
        // Get the dictionary with native functions
        public Dictionary<string, Func<double, double>> GetNativeFunctions()
        {
            return nativeFunctions;
        }
        // Get the dictionary with native variables
        public Dictionary<string, double> GetNativeVariables()
        {
            return nativeVariables;
        }
        // Get the dictionary with the protected words
        public Dictionary<string, string> GetProtectedWords()
        {
            return protectedWords;
        }
        // Get variable value
        public double variableDefinition(string name)
        {
            // Try to get the entry for the given variable name
            StorageEntry? variable = createdVariables.Find(v => v.Name == name);
            double definition = 0;

            // Check if the wanted variable existsin the createdVariables list
            if (variable != null)
            {
                double.TryParse(variable.Definition, out definition);
            }
            // Check if the wanted variable exists in the nativeVariables dictionary
            else if (nativeVariables.ContainsKey(name))
            {
                definition = nativeVariables[name];
            }
            return definition;
        }
        // Add function or variable
        public void AddEntry(string type, string name, string definition)
        {
            // Load the function and variable lists from the json files
            (StorageList createdFunctions, StorageList createdVariables) = LoadFromFile();

            // Check type of entry being saved
            switch (type)
            {
                case "function":
                    // Create and add the new instance of StorageEntry
                    StorageEntry function = new StorageEntry(name, definition);
                    createdFunctions.Add(function);

                    // Convert the list to a json string and save it to the json file
                    jsonString = JsonSerializer.Serialize(createdFunctions);
                    UpdateJSON(jsonString, functionsFilePath);
                    break;
                case "variable":
                    // Check if the variable name exists and change definition or create a new StorageEntry
                    StorageEntry variable = createdVariables.Find(v => v.Name == name) ?? new StorageEntry(name, definition);
                    createdVariables.Remove(variable);
                    variable.Definition = definition;

                    // Add a new variable to the list
                    createdVariables.Add(variable);

                    // Convert the list to a json string and save it to the json file
                    jsonString = JsonSerializer.Serialize(createdVariables);
                    UpdateJSON(jsonString, variablesFilePath);
                    break;
            }
        }
        // Delete function or variable
        public void DeleteEntry(string type, string name)
        {
            // Load the function and variable lists from the json files
            (StorageList createdFunctions, StorageList createdVariables) = LoadFromFile();

            // Check the type of entry being deleted
            switch (type)
            {
                case "function":
                    // Try to get the function from createdFunction list
                    StorageEntry? function = createdFunctions.Find(f => f.Name == name);
                    if (function != null)
                    {
                        // Remove the function from the list
                        createdFunctions.Remove(function);
                    }
                    // Convert the list to a json string and save it to the json file
                    jsonString = JsonSerializer.Serialize(createdFunctions);
                    UpdateJSON(jsonString, functionsFilePath);
                    break;
                case "variable":
                    // Try to get the function from the createdVariable list
                    StorageEntry? variable = createdVariables.Find(v => v.Name == name);
                    if (variable != null)
                    {
                        // Remove the variable from the list
                        createdVariables.Remove(variable);
                    }
                    // Convert the list to a json string and save it to the json file
                    jsonString = JsonSerializer.Serialize(createdVariables);
                    UpdateJSON(jsonString, variablesFilePath);
                    break;
            }
        }
        // Update the json file
        private void UpdateJSON(string jsonString, string filePath)
        {
            // Update the data in the json file
            File.WriteAllText(filePath, jsonString);

            // Reload the file
            LoadFromFile();
        }
        // Search for the occurance of a function name
        public bool FunctionExists(string name)
        {
            StorageEntry? createdFunction = createdFunctions.Find(f => f.Name == name);
            return createdFunction != null || nativeFunctions.ContainsKey(name);
        }
        // Search for the occurance of a variable name
        public bool VariableExists(string name)
        {
            StorageEntry? variable = createdVariables.Find(v => v.Name == name);
            return variable != null || nativeVariables.ContainsKey(name);
        }
    }
}