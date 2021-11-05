using System.Text;

namespace KnapsackProblem
{
    public class Chromosome
    {
        public bool[] Genes;

        public Chromosome(bool[] genes)
        {
            Genes = genes;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (bool gen in Genes)
            {
                result.Append(" " + gen);
            }

            return result.ToString();
        }
    }
}