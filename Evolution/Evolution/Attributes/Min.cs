using System;

namespace Evolution.Attributes
{
    public class Min : Attribute
    {
        public Min(double min)
        {
            this.Value = min;
        }

        public double Value { get; private set; }
    }
}
