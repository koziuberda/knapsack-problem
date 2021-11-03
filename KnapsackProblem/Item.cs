namespace KnapsackProblem
{
    public class Item
    {
        public int Weight { get; private set; }
        public int Value { get; private set; }

        public Item(int weight, int value)
        {
            Weight = weight;
            Value = value;
        }
    }
}