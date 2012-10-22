namespace ElectricField.Classes
{
    public class ChargeDensity
    {
        public string Name;
        public Charge.ChargeType Type;

        private Helper.Area _bodySurface;
        private double _density;
        private double _electricCharge;

        public ChargeDensity()
        {
            Name = "New Surface";
            Type = Charge.ChargeType.Positive;
            _density = 1;
            _electricCharge = 1;
            _bodySurface = new Helper.Area();
            IsActive = true;
        }

        public ChargeDensity(ChargeDensity chargeDensity)
        {
            Name = chargeDensity.Name;
            Type = chargeDensity.Type;
            _density = chargeDensity.Density;
            _electricCharge = chargeDensity.ElectricCharge;

            _bodySurface = new Helper.Area(chargeDensity.BodySurface.Height, chargeDensity.BodySurface.Width);
            IsActive = chargeDensity.IsActive;
        }

        public double ElectricCharge
        {
            get { return _electricCharge; }
            set
            {
                _electricCharge = value;
                _density = _electricCharge/BodySurface.GetArea();
            }
        }

        public double Density
        {
            get { return _density; }
            set
            {
                _density = value;
                _electricCharge = _density*BodySurface.GetArea();
            }
        }

        public Helper.Area BodySurface
        {
            get { return _bodySurface; }
            set
            {
                _bodySurface = value;
                _density = _electricCharge/BodySurface.GetArea();
            }
        }

        public bool IsActive { get; set; }
    }
}