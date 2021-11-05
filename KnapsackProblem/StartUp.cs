using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KnapsackProblem
{
    class StartUp
    {
        static void Main(string[] args)
        {
            const int numberOfItems = 100;
            const int capacity = 250;

            var items = new List<Item>();

            Console.WriteLine("This is program that solves knapsack problem.");
            Console.WriteLine("Choose how to get items: ");
            Console.WriteLine("0 - generate items randomly");
            Console.WriteLine("1 - input from file");
            char ch = Convert.ToChar(Console.ReadLine());

            if (ch == '0')
            {
                for (int i = 0; i < numberOfItems; i++)
                {
                    items.Add(GetRandomItem());
                }
            }
            else if (ch == '1')
            {
                Console.WriteLine("Please, input path to file: ");
                var pathToFile = Console.ReadLine();
                var text = File.ReadAllText(pathToFile);
                items = JsonSerializer.Deserialize<List<Item>>(text);
            }
            else
            {
                Console.WriteLine("Bad input!");
                return;
            }
            
            var solver = new GeneticAlgorithm(items, capacity);
            Knapsack solution = solver.Solve();
            
            Console.WriteLine("The problem is solved. The final knapsack: ");
            Console.WriteLine($"Weight: {solution.Weight}");
            Console.WriteLine($"Value: {solution.Value}");

            List<Item> chosenItems = solution.Items;
            
            var chosenItemsJson = JsonSerializer.Serialize(chosenItems);

            Directory.CreateDirectory("Results");
            File.WriteAllText(@"Results\knapsack.json",chosenItemsJson);
            
            var allItemsSerialized = JsonSerializer.Serialize(items);
            File.WriteAllText(@"Results\items.json",allItemsSerialized);
            
            solver.GetStatisticsToFile(@"Results\statistics.csv");
            
            Console.WriteLine("You can see the detailed results in Results folder.");
        }

        private static Item GetRandomItem()
        {
            var random = new Random();

            return new Item(random.Next(1, 26), random.Next(2, 31));
        }
    }
}
