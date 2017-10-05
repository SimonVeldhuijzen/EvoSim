using Evolution.Attributes;

namespace Evolution
{
    public static class Parameters
    {
        [Min(25)]
        public static int Health { get; set; } = 4500;
        [Min(0)]
        public static int FoodValue { get; set; } = 600;
        [Min(0)]
        public static int DangerValue { get; set; } = 25;
        public static int FoodCount { get; set; } = 45;
        public static int DangerCount { get; set; } = 10;
        [Min(1)]
        public static int BlobCount { get; set; } = 15;
        [Min(0)]
        public static int StepDamage { get; set; } = 1;
        [Min(1)]
        public static int BlobSpeed { get; set; } = 2;
        [Min(0), Max(1)]
        public static double MateFrom { get; set; } = 0.7;
        [Min(0), Max(1)]
        public static double MateDamage { get; set; } = 0.36;
        [Min(1)]
        public static int EntityRadius { get; set; } = 5;
        [Min(1)]
        public static int TimerStep { get; set; } = 1;
        [Min(1)]
        public static int BaseEyeSightLength { get; set; } = 50;
        [Min(1)]
        public static int MaxEyeSightLength { get; set; } = 100;
        [Min(1)]
        public static int BaseEyeSightWidth { get; set; } = 16;
        [Min(1)]
        public static int MaxEyeSightWidth { get; set; } = 50;
        [Min(0)]
        public static int MaxTurnPerTick { get; set; } = 1;
        [Min(1)]
        public static int EatenCountForMate { get; set; } = 5;
        [Min(1)]
        public static int EatenCountForSplit { get; set; } = 10;
        [Min(1)]
        public static int EntitiesToPass { get; set; } = 3;
        [Min(1)]
        public static int MaxBlobs { get; set; } = 50;

        public static bool CheckParameters()
        {
            var allOk = true;

            foreach (var property in typeof(Parameters).GetProperties())
            {
                foreach (var att in property.GetCustomAttributes(false))
                {
                    if (att.GetType() == typeof(Min))
                    {
                        var min = ((Min)att).Value;
                        var value = double.Parse(property.GetValue(typeof(Parameters)).ToString());
                        if (value < min)
                        {
                            allOk = false;
                            AutoClosingMessageBox.Show($"Minimum value for parameter \"{property.Name}\" is {min}", "Parameter value out of allowed range", 5000);
                            var typeOfProperty = property.PropertyType;
                            if (typeOfProperty == typeof(double))
                            {
                                property.SetValue(typeof(Parameters), min);
                            }
                            else if (typeOfProperty == typeof(int))
                            {
                                property.SetValue(typeof(Parameters), (int)min);
                            }
                        }
                    }
                    else if (att.GetType() == typeof(Max))
                    {
                        var max = ((Max)att).Value;
                        var value = double.Parse(property.GetValue(typeof(Parameters)).ToString());
                        if (value > max)
                        {
                            allOk = false;
                            AutoClosingMessageBox.Show($"Maximum value for parameter \"{property.Name}\" is {max}", "Parameter value out of allowed range", 5000);
                            var typeOfProperty = property.PropertyType;
                            if (typeOfProperty == typeof(double))
                            {
                                property.SetValue(typeof(Parameters), max);
                            }
                            else if (typeOfProperty == typeof(int))
                            {
                                property.SetValue(typeof(Parameters), (int)max);
                            }
                        }
                    }
                }
            }

            return allOk;
        }
    }
}
