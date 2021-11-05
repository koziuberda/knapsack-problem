using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KnapsackProblem
{
    public class GeneticAlgorithm
    {
        private readonly List<Item> _items;
        private readonly int _capacity;
        private List<Chromosome> _population;
        private int _bestValue;
        private List<Tuple<int, int>> _statistics;

        public GeneticAlgorithm(List<Item> items, int capacity)
        {
            _items = items;
            _capacity = capacity;
            _population = new List<Chromosome>();
            _bestValue = 0;
            _statistics = new List<Tuple<int, int>>();
        }

        public Knapsack Solve()
        {

            // Initializing a population
            for (int i = 0; i < _items.Count; i++)
            {
                bool[] currentChromosome = new bool[_items.Count];
                currentChromosome[i] = true;
                _population.Add(new Chromosome(currentChromosome));
            }

            // It's nice to work with sorted list
            _population = _population.OrderBy(chromosome => ToKnapsack(chromosome).Value).ToList();
            
            for (int generationCounter = 0; generationCounter < 1000; generationCounter++)
            {
                _bestValue = ToKnapsack(_population.Last()).Value;

                if (generationCounter % 20 == 0)
                {
                    _statistics.Add(Tuple.Create<int, int>(generationCounter, _bestValue));
                }
                
                Tuple<int, int> parentsPair = ChooseParents();
                var firstParent = _population[parentsPair.Item1];
                var secondParent = _population[parentsPair.Item2];
                
                // Make children
                Chromosome firstChild = Crossover(firstParent,secondParent);
                Chromosome secondChild = Crossover(secondParent, firstParent);

                var random = new Random();

                if (random.Next(20) == 0)
                {
                    var firstMutant = Mutation(firstChild);
                    if (ToKnapsack(firstMutant).Weight <= _capacity)
                    {
                        firstChild = firstMutant;
                    }
                }

                if (random.Next(20) == 0)
                {
                    var secondMutant = Mutation(secondChild);
                    if (ToKnapsack(secondMutant).Weight <= _capacity)
                    {
                        secondChild = secondMutant;
                    }
                }

                // Add children to population
                int aliveChildren = 0;
                if (ToKnapsack(firstChild).Weight <= _capacity)
                {
                    _population.Add(Improvement(firstChild));
                    aliveChildren++;
                }

                if (ToKnapsack(secondChild).Weight <= _capacity)
                {
                    _population.Add(Improvement(secondChild));
                    aliveChildren++;
                }
                
                _population = _population.OrderBy(chromosome => ToKnapsack(chromosome).Value).ToList();
                
                // Remove exactly as much as we added 
                for (int i = 0; i < aliveChildren; i++)
                {
                    _population.RemoveAt(0);
                }
            }

            return ToKnapsack(_population.Last());
        }

        public void GetStatisticsToFile(string path)
        {
            var writer = new StreamWriter(path);
            foreach (Tuple<int, int> pair in _statistics)
            {
                string line = $"{pair.Item1},{pair.Item2}";
                writer.WriteLine(line);
            }
            writer.Close();
        }

        private Knapsack ToKnapsack(Chromosome chromosome)
        {
            var itemsToAdd = new List<Item>();
            for (int i = 0; i < chromosome.Genes.Length; i++)
            {
                if (chromosome.Genes[i])
                {
                    itemsToAdd.Add(_items[i]);
                }
            }

            return new Knapsack(itemsToAdd);
        }

        private Chromosome Crossover(Chromosome first, Chromosome second)
        {
            const double fraction = 0.3;
            int point = (int)(_items.Count() * fraction);
            bool[] child = new bool[_items.Count];
            
            for (int i = 0; i < point; i++)
            {
                child[i] = first.Genes[i];
            }

            for (int i = point; i < _items.Count; i++)
            {
                child[i] = second.Genes[i];
            }

            return new Chromosome(child);
        }

        private Tuple<int, int> ChooseParents()
        {
            Chromosome bestElement = null;
            
            foreach (var current in _population)
            {
                if (ToKnapsack(current).Value == _bestValue)
                {
                    bestElement = current;
                    break;
                }
            }
            
            var indexOfBestElement = _population.IndexOf(bestElement);
            int randomIndex;
            do
            {
                randomIndex = new Random().Next(0, _population.Count);
            } while (randomIndex == indexOfBestElement);
            
            return Tuple.Create(indexOfBestElement, randomIndex);
        }

        private Chromosome Mutation(Chromosome individual)
        {
            bool[] genes = individual.Genes.ToArray();

            var random = new Random();
            int first = random.Next(0, genes.Length);
            int second = random.Next(0, genes.Length);

            while (first == second)
            {
                second = random.Next(0, genes.Length);
            }

            (genes[first], genes[second]) = (genes[second], genes[first]);

            return new Chromosome(genes);
        }

        private Chromosome Improvement(Chromosome individual)
        {
            List<Item> itemsToAdd = new List<Item>();

            // Take those items who aren't included
            for (int i = 0; i < individual.Genes.Length; i++)
            {
                if (!individual.Genes[i])
                {
                    itemsToAdd.Add(_items[i]);
                }
            }

            // Trying to find the lightest possible item to add
            var minWeight = itemsToAdd.Min(item => item.Weight);
            Item lightest = itemsToAdd.Find(item => item.Weight == minWeight);

            int index = _items.IndexOf(lightest);

            bool[] newGenes = individual.Genes.ToArray();
            newGenes[index] = true;

            Chromosome newIndividual = new Chromosome(newGenes);

            // Actually, we could overweight the knapsack by adding an item
            if (ToKnapsack(newIndividual).Weight <= _capacity)
            {
                return newIndividual;
            }
            else
            {
                return individual;
            }
        }
    }
}