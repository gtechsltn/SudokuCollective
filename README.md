# Sudoku Collective

Sudoku Collective is an open source Web API that you can use to learn client side technologies such as React or Vue.js. With this API you will create an app that allows users to play sudoku puzzles and compare their performance against other players. The benefit of using this tool is that once you create your first app and obtain an understanding of how the API works you will be in a better position to compare and understand various frontend technologies like React and Vue.js. The API is fully documented so you can create your own client app which can fully integrate the API. Thank you for checking out this tool and if you decide to proceed I sincerely hope you have fun.

## Requirements

- [.Net 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [PostgreSQL 11](https://www.postgresql.org/download/)
- [Vue/CLI - version 4.5.7](https://cli.vuejs.org/)
- [Redis Server - version 5.0.7](https://redis.io/download)

For the Redis Cache Server on Windows 10 it is recommended you use [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) in order to install and run Redis through Ubuntu 20.04.  The instruction for downloading from the official Ubuntu PPA are contained in the above Redis link.

## Installation

In the API project you will find a *dummysettings.json* file that you can use to populate the *appsettings.json* file that is required to run the app.  Simply rename the file to *appsettings.json* and place your value where it states [Your value here].

For the *License* field in *DefaultAdminApp* and *DefaultClientApp* you can enter a hexadecimal value, random values can be generated [here](https://www.guidgenerator.com/online-guid-generator.aspx), braces aren't necessary but you should include hyphens.

In the API project you will also find the Vue.js client app.  You will need to add a *.env* file to the client app.  In the app you will see a *.env.dummy* file that documents the required values just as the *dummysettings.json* file does in the API project.  Just replace the values where it states 'your value here' and rename the file to *.env*.

Once the above is done run the following command to instantiate the database:

`dotnet ef database update`

Once done the project should start.