# Sort Of Smart Calculator

## Introduction

This repository contains a console application based on C# built to funciton as a calculator that can make simple calculations and handle functions and variables. The program is able to understand the order of which a expression should be calculated based on operators and parentheses.

The program consists of following files:
* Program.cs - Runs the application
* UserInterface.cs - Generates the UI for the application.
* Calculator.cs - Does all of the calculations aswell as validates the name and defintion for functions created by users.
* Tokenizer.cs - Takes the input given by user for the calculator and seprates its parts to tokens, this makes it possible for the calculator make the calculation for diffrent parts of the expression in the correct order. 
* Storage.cs - Handles of the functions and variables being stored to the program.
* Exceptions.cs - Contains subclasses of the class Ezxception for more distinct error handling.
* functions.json - File where created functions are stored.
* variables.json - File where created variables are stored.

## Documention

The application consist of three parts. The calculator, the handling of functions and the handling of variables.

### Calculator

The calculator are used to make calculations and assign values to variables. By writing "help" in the input field you can obtain the possible commands in the calculator.

To make a calculation you write the expression you want to calculate and press "enter". If done correctly the calculator will provide a answer otherwise a error message containing a simple explantion will appear instead. You can use predetermined functions and variables as a part of your expression which can be viewed by typing "func" or "vars".

To clear the console you type "clear" and to go back to the main menu you type "back.

### Functions and variables

Both functions and variables works in similar ways by providing same functionalities for different parts of your program. You can view, create and delete functions and variables. Further explanation of these processes are stated in the application.