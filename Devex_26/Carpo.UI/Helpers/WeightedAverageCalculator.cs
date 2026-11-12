using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.UI.Helpers
{
    public class WeightedAverageCalculator
    {
        decimal totalWeight = 0;
        decimal totalWeightedValue = 0;
        public WeightedAverageCalculator() { }

        public void Add(decimal weight, decimal value)
        {
            totalWeightedValue += value * weight;
            totalWeight += weight;
        }
        public decimal Calculate()
        {
            if (totalWeight == 0 || totalWeightedValue == 0) return 0;
            return totalWeightedValue / totalWeight;
        }
    }
}
