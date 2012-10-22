using System.Windows;

namespace ElectricField.Classes
{
    public class PointStatistics
    {
        public PointStatistics()
        {
            Time = 0;
            Position = new Point(0, 0);
        }

        public PointStatistics(Point location, int time, double acceleration, double velocity, double force)
        {
            Position = location;
            Time = time;
            Acceleration = acceleration;
            Velocity = velocity;
            Force = force;
        }

        public int Time { set; get; }
        public Point Position { get; set; }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public double Force { get; set; }
    }
}