using System;
using System.Text;
using System.Threading.Tasks;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Models;
using SudokuCollective.Dev.Classes;

namespace SudokuCollective.Dev.Routines
{
    internal static class SolveSolutions
    {
        internal static void Run()
        {
            Console.WriteLine("\nPlease enter the sudoku puzzle you wish to solve.");
            Console.WriteLine("You will be entering the nine values for each row.");
            Console.WriteLine("Just enter the values with no spaces, for unknown");
            Console.WriteLine("values enter 0.  Once you're done the solver will");
            Console.WriteLine("produce an answer.  The solver will notify you if");
            Console.WriteLine("the sodoku puzzle cannot be solved.\n");
            Console.WriteLine("Press enter to continue!");

            Console.ReadLine();

            var continueLoop = true;

            do
            {
                var response = new StringBuilder();

                Console.Write("Enter the first row:   ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the second row:  ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the third row:   ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the fourth row:  ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the fifth row:   ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the sixth row:   ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the seventh row: ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the eighth row:  ");

                response.Append(Console.ReadLine());

                Console.Write("Enter the ninth row:   ");

                response.Append(Console.ReadLine());

                Console.Write("\nPress enter to continue... ");

                Console.ReadLine();

                Console.WriteLine();

                var matrix = new SudokuMatrix(response.ToString());

                Task solver = matrix.Solve();

                ConsoleSpiner spin = new ConsoleSpiner();

                while (!solver.IsCompleted)
                {
                    spin.Turn();
                }

                Console.WriteLine();

                Console.Beep();

                if (matrix.IsValid())
                {
                    var displayMatrix = new SudokuMatrix(matrix.ToIntList());

                    displayMatrix.SetDifficulty(
                        new Difficulty
                        {
                            Name = "Test",
                            DifficultyLevel = DifficultyLevel.TEST
                        });

                    DisplayScreens.DisplayMatix(displayMatrix);

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        matrix.Stopwatch.Elapsed.Hours,
                        matrix.Stopwatch.Elapsed.Minutes,
                        matrix.Stopwatch.Elapsed.Seconds,
                        matrix.Stopwatch.Elapsed.Milliseconds / 10);

                    Console.Write("\n\nTime to generate solution: " + elapsedTime + "\n\n");

                }
                else
                {
                    Console.WriteLine("\nNeed more values in order to deduce a solution.\n");
                }

                Console.Write("Would you like to solve another solution (yes/no): ");

                var result = Console.ReadLine();

                if (result.ToLower().Equals("no") || result.ToLower().Equals("n"))
                {
                    continueLoop = false;
                }
                else
                {
                    Console.WriteLine();
                }

            } while (continueLoop);
        }
    }
}
