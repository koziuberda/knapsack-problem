using System.Collections.Generic;
using System.Linq;

namespace KnapsackProblem
{
    public class Knapsack
    {
        private List<Item> Items;
        public int Weight => Items.Sum(item => item.Weight);

        public Knapsack(List<Item> items)
        {
            Items = items;
        }
    }
}