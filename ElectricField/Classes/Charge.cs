using System.Windows;

namespace ElectricField.Classes
{
    public class Charge
    {
        public string Name;
        public ChargeType Type;
        public int ElectricCharge { get; set; }
        public int Mass { get; set; }
        public Point Location { get; set; } // (0,0) will be the Current FieldPoint
        public object ChargeTag { get; set; }
        public bool IsActive { get; set; }
        public Charge()
        {
            Name = "New Charge";
            Type = ChargeType.Positive;
            ElectricCharge = 1;
            Mass = 1;
            Location = new Point(0,0);
            IsActive = true;
        }
        public Charge(string name,ChargeType type)
        {
            Name = name;
            Type = type;
            ElectricCharge = 1;
            Mass = 1;
            Location = new Point(0, 0);
            IsActive = true;
        }

        public double GetForce
        {
            get { return 0; }
        }

        public enum ChargeType
        {
            Positive = 0,
            Negative = 1
        }
    }
}