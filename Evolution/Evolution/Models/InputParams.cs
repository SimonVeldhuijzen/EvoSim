using System.Collections.Generic;

namespace Evolution.Models
{
    public class InputParams
    {
        public double Distance { get; }
        public double XDist { get; }
        public double YDist { get; }

        public InputParams(double distance, double xDist, double yDist)
        {
            this.Distance = distance;
            this.XDist = xDist;
            this.YDist = yDist;
        }

        public static List<double> ConvertToDoubles(List<InputParams> inputs)
        {
            var result = new List<double>();

            for (int i = 0; i < Parameters.EntitiesToPass; i++)
            {
                if (inputs.Count > i)
                {
                    result.Add(inputs[i].XDist);
                    result.Add(inputs[i].YDist);
                }
                else
                {
                    result.Add(0);
                    result.Add(0);
                }
            }

            return result;
        }
    }
}
