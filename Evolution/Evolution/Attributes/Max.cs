using System;

namespace Evolution.Attributes
{
    public class Max : Attribute
    {
        public Max(double max)
        {
            this.Value = max;
        }

        public double Value { get; private set; }
    }
}
