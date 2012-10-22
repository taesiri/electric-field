using System.Windows;

namespace ElectricField.Classes
{
    public class Charge
    {
        #region ChargeType enum

        public enum ChargeType
        {
            Positive = 0,
            Negative = 1
        }

        #endregion

        public string Name;
        public ChargeType Type;

        public Charge()
        {
            Name = "New Charge";
            Type = ChargeType.Positive;
            ElectricCharge = 1;
            Mass = 1;
            Location = new Point(0, 0);
            IsActive = true;
        }

        public Charge(string name, ChargeType type)
        {
            Name = name;
            Type = type;
            ElectricCharge = 1;
            Mass = 1;
            Location = new Point(0, 0);
            IsActive = true;
        }

        public int ElectricCharge { get; set; }
        public int Mass { get; set; }
        public Point Location { get; set; } // (0,0) will be the Current FieldPoint
        public object ChargeTag { get; set; }
        public bool IsActive { get; set; }

        public double GetForce
        {
            get { return 0; }
        }
    }
}