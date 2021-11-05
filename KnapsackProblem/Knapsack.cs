using System.Collections.Generic;
using System.Linq;

namespace KnapsackProblem
{
    public class Knapsack
    {
        private List<Item> _items;
        public int Weight => _items.Sum(item => item.Weight);
        public int Value => _items.Sum(item => item.Value);

        public Knapsack(List<Item> items)
        {
            _items = items;
        }
    }
}