using System;
using System.IO;
using System.Linq;

namespace TillManagementCSharp
{
    class Program
    {
        // Function to calculate the change breakdown
        static string CalculateChange(int change)
        {
            int[] denominations = { 50, 20, 10, 5, 2, 1 };
            var changeBreakdown = Enumerable.Empty<int>();

            foreach (int denom in denominations)
            {
                int count = change / denom;
                if (count > 0)
                {
                    changeBreakdown = changeBreakdown.Concat(Enumerable.Repeat(denom, count));
                    change -= count * denom;
                }
            }

            return string.Join('-', changeBreakdown.Select(denom => $"R{denom}"));
        }

        // Function to process transactions
        static void ProcessTransactions(string inputFile, string outputFile)
        {
            int till = 500;
            string output = "Till Start, Transaction Total, Paid, Change Total, Change Breakdown\n";

            string[] lines = File.ReadAllLines(inputFile);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length != 2)
                {
                    Console.WriteLine($"Invalid input format: {line}");
                    continue;
                }

                string items = parts[0];
                string payment = parts[1];

                if (string.IsNullOrEmpty(items) || string.IsNullOrEmpty(payment))
                {
                    Console.WriteLine($"Invalid input format: {line}");
                    continue;
                }

                // Calculate total cost of items
                int totalCost = items.Split(';').Sum(item => int.Parse(item.Trim().Split('R').Last()));

                // Calculate total paid
                int totalPaid = payment.Split('-').Sum(amount => int.Parse(amount.Trim('R')));

                // Calculate change
                int change = totalPaid - totalCost;

                // Format change breakdown
                string changeBreakdown = CalculateChange(change);

                // Write transaction details to output string
                output += $"R{till}, R{totalCost}, R{totalPaid}, R{change}, {changeBreakdown}\n";

                // Update till amount
                till += totalCost;
            }

            // Write amount left in till to output string
            output += $"R{till}";

            // Write output string to file
            File.WriteAllText(outputFile, output);
        }

        static void Main(string[] args)
        {
            ProcessTransactions("input.txt", "output.txt");
        }
    }
}