using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ElectricField.SolverClasses;

namespace ElectricField.Classes
{
    public class Helper
    {
        public static Vector NormalizeVector(Vector myVec)
        {
            return new Vector(myVec.X/(VectorMagnitude(myVec)), myVec.Y/(VectorMagnitude(myVec)));
        }

        public static double VectorMagnitude(Vector myvec)
        {
            return Math.Sqrt((myvec.X*myvec.X) + (myvec.Y*myvec.Y));
        }

        public static Vector ReverseVector(Vector myvec)
        {
            return new Vector(myvec.Y, myvec.X);
        }

        public static Vector InverseVector(Vector myvec)
        {
            return new Vector(-myvec.X, -myvec.Y);
        }

        public static double Distance(Point point1, Point point2)
        {
            double DisX = Math.Abs(point1.X - point2.X);
            double DisY = Math.Abs(point1.Y - point2.Y);

            return Math.Sqrt((DisX*DisX) + (DisY*DisY));
        }

        public static double RadianToDegree(double angle)
        {
            return angle*(180.0/Math.PI);
        }

        public static double Clamp(double val, int min, int max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;
            else
            {
                return val;
            }
        }

        public static double InvertClamp(double val, int min, int max)
        {
            if (val < min)
                return max;
            else if (val > max)
                return min;
            else
            {
                return val;
            }
        }

        public static bool IsInBetween(double number, int min, int max)
        {
            if ((int) number >= min && (int) number <= max)
                return true;
            return false;
        }

        public static int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        public static double AmountOForce(int electricCharge, double distance)
        {
            return ((9*electricCharge)/(distance*distance));
        }

        public static double AmountOForce(double electricCharge, double distance)
        {
            return ((9*electricCharge)/(distance*distance));
        }

        public static int GetMin(double[] data)
        {
            double minimume = data[0];
            foreach (double t in data)
            {
                if (t < minimume && t > 0)
                    minimume = t;
            }
            return (int) minimume;
        }

        public static int GetMax(double[] data)
        {
            double maximume = data[0];
            maximume = data.Concat(new[] {maximume}).Max();
            return (int) maximume;
        }

        public static int[] Integerizer(double[] data)
        {
            var newdata = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                newdata[i] = (int) data[i];
            }

            return newdata;
        }

        public static int[] Sort(int[] data)
        {
            Array.Sort(data);
            return data;
        }

        public static int Mode(double[] data)
        {
            int[] newdata = Sort(Integerizer(data));

            int mode = 0;
            int modeRepeats = 0;

            for (int i = 0; i < newdata.Length; i++)
            {
                int currentmode = newdata[i];
                int currentmodeRepeats = 1;
                for (int j = i + 1; j < newdata.Length; j++)
                {
                    if (currentmode == newdata[j])
                        currentmodeRepeats++;
                    else
                        break;
                }

                if (currentmodeRepeats > modeRepeats)
                {
                    mode = currentmode;
                    modeRepeats = currentmodeRepeats;
                }
                i += currentmodeRepeats - 1;
            }

            return mode;
        }

        public static bool Thresholder(int fvalue, double svalue, int threshold)
        {
            return Math.Abs(((int) svalue) - fvalue) <= threshold;
        }

        public static Color GetCurrentColor(double percent, Color aColor, Color bColor)
        {
            if (percent > 1)
            {
                throw new InvalidDataException("Holy Shit!");
            }

            var cr = (byte) ((percent*aColor.R) + ((1 - percent)*bColor.R));
            var cg = (byte) ((percent*aColor.G) + ((1 - percent)*bColor.G));
            var cb = (byte) ((percent*aColor.B) + ((1 - percent)*bColor.B));
            return Color.FromRgb(cr, cg, cb);
        }

        public static IEnumerable<Point> CalculateVelocity(IEnumerable<Point> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            var velocityList = new List<Point>();

            for (int i = 0; i < data.Count() - 1; i++)
            {
                double currentValue = (data.ToArray()[i].Y);
                double nextValue = (data.ToArray()[i + 1].Y);

                double deltaTime = (data.ToArray()[i + 1].X - data.ToArray()[i].X);


                double velocity = (nextValue - currentValue)/deltaTime;
                double time = data.ToArray()[i].X;

                velocityList.Add(new Point(time, velocity));
            }

            return velocityList;
        }

        public static LinearGradientBrush ConvertToBrush(List<Solver.Colorizer> data)
        {
            var returnBrush = new LinearGradientBrush();
            float counter = 1f;
            foreach (Solver.Colorizer colorizer in data)
            {
                returnBrush.GradientStops.Add(new GradientStop(colorizer.Color, (counter/data.Count)));

                counter++;
            }

            return returnBrush;
        }

        public static Solver.Colorizer GetNextColor(List<Solver.Colorizer> listOfColors, Solver.Colorizer afterThis)
        {
            Solver.Colorizer returnValue = afterThis;

            foreach (Solver.Colorizer colorizer in listOfColors)
            {
                if (IsInBetween(afterThis.Max + 1, colorizer.Min, colorizer.Max))
                {
                    returnValue = colorizer;
                }
            }
            return returnValue;
        }

        public static Solver.Colorizer GetPrevColor(List<Solver.Colorizer> listOfColors, Solver.Colorizer afterThis)
        {
            Solver.Colorizer returnValue = afterThis;

            foreach (Solver.Colorizer colorizer in listOfColors)
            {
                if (IsInBetween(afterThis.Min - 1, colorizer.Min, colorizer.Max))
                {
                    returnValue = colorizer;
                }
            }
            return returnValue;
        }

        public static Solver.Colorizer ConvertToColorizer(Solver.ColorRange data)
        {
            var returnItem = new Solver.Colorizer {Min = (int) data.Min, Max = (int) data.Max, Color = data.Color};

            return returnItem;
        }

        public static IEquatable<Point> CalculateAcceleration(IEquatable<Point> data)
        {
            return null;
        }

        #region Nested type: Area

        public class Area
        {
            public Area()
            {
                Height = 1;
                Width = 1;
            }

            public Area(int height, int width)
            {
                Height = height;
                Width = width;
            }

            public int Height { get; set; }
            public int Width { get; set; }

            public int GetArea()
            {
                return Height*Width;
            }
        }

        #endregion
    }
}