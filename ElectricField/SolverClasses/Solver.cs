﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ElectricField.Classes;
using ElectricField.Controls;

namespace ElectricField.SolverClasses
{
    public class Solver
    {
        private readonly List<Charge> _charges;
        private readonly int _height;
        private readonly List<Surface> _surfaces;
        private readonly int _width;
        private double[] _data;

        public Solver(IEnumerable<Charge> charges, IEnumerable<Surface> surfaces, int height, int width)
        {
            _charges = new List<Charge>(charges);
            _surfaces = new List<Surface>(surfaces);
            _height = height;
            _width = width;
        }

        public void SolveIt()
        {
            _data = new double[_height*_width];
            for (int yPos = 0; yPos < _height; ++yPos)
            {
                int yIndex = yPos*_width;
                for (int xPos = 0; xPos < _width; ++xPos)
                {
                    var totalForceVec = new Vector(0, 0);
                    foreach (Charge charge in _charges)
                    {
                        if (charge.IsActive)
                        {
                            double distanceX = xPos - charge.Location.X;
                            double distanceY = charge.Location.Y - yPos;
                            distanceX -= 16;
                            distanceY += 16;
                            var tempVec = new Vector(distanceX, distanceY);
                            double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;
                            if (charge.Type == Charge.ChargeType.Negative)
                                tempVec = Helper.InverseVector(tempVec);
                            tempVec = Helper.NormalizeVector(tempVec);
                            tempVec = Helper.AmountOForce(charge.ElectricCharge, amountOfDistance)*tempVec;
                            totalForceVec += tempVec;
                        }
                    }

                    if (_surfaces != null)
                    {
                        foreach (Surface surface in _surfaces)
                        {
                            if (surface.MyCharge.IsActive)
                            {
                                double amountOfChargePerPoint = surface.MyCharge.ElectricCharge/10;
                                var surfaceLocation = new Point(surface.RenderTransform.Value.OffsetX,
                                                                surface.RenderTransform.Value.OffsetY);
                                double parresh = surface.Width/10;
                                double taVasat = surface.Height/2;

                                for (int i = 0; i < 10; i++)
                                {
                                    var here = new Point(surfaceLocation.X + (i*parresh), surfaceLocation.Y + taVasat);

                                    double distanceX = xPos - here.X;
                                    double distanceY = here.Y - yPos;

                                    var tempVec = new Vector(distanceX, distanceY);
                                    double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                                    if (surface.MyCharge.Type == Charge.ChargeType.Negative)
                                    {
                                        tempVec = Helper.InverseVector(tempVec);
                                    }
                                    tempVec = Helper.NormalizeVector(tempVec);
                                    tempVec = Helper.AmountOForce(amountOfChargePerPoint, amountOfDistance)*tempVec;

                                    totalForceVec += tempVec;
                                }
                            }
                        }
                    }

                    _data[xPos + yIndex] = Helper.VectorMagnitude(totalForceVec);
                }
            }
        }

        public List<Point> GetFieldLines()
        {
            int thrsldX = 16;
            int thrsldY = 16;
            var fieldLines = new List<Point>();

            if (_charges.Count != 0)
            {
                foreach (Charge charge in _charges)
                {
                    if (charge.IsActive)
                    {
                        var data = new List<Point>();
                        bool isNegative = charge.Type == Charge.ChargeType.Negative;
                        var pointlist = new[]
                                            {
                                                new Point(charge.Location.X, charge.Location.Y),
                                                new Point(charge.Location.X + thrsldX, charge.Location.Y),
                                                new Point(charge.Location.X + (2*thrsldX), charge.Location.Y),
                                                new Point(charge.Location.X, charge.Location.Y + thrsldY),
                                                new Point(charge.Location.X + (2*thrsldX), charge.Location.Y + thrsldY),
                                                new Point(charge.Location.X, charge.Location.Y + (2*thrsldY)),
                                                new Point(charge.Location.X + thrsldX, charge.Location.Y + (2*thrsldY)),
                                                new Point(charge.Location.X + (2*thrsldX),
                                                          charge.Location.Y + (2*thrsldY))
                                            };
                        foreach (Point cpoint in pointlist)
                        {
                            PointTracker(cpoint, new Point(_width, _height), ref data, isNegative);
                            fieldLines.AddRange(data);
                        }
                    }
                }
            }

            if (_surfaces.Count != 0)
            {
                thrsldX = 50;
                thrsldY = 16;
                foreach (Surface sfce in _surfaces)
                {
                    if (sfce.MyCharge.IsActive)
                    {
                        var data = new List<Point>();
                        bool isNegative = sfce.MyCharge.Type == Charge.ChargeType.Negative;
                        double locationX = sfce.RenderTransform.Value.OffsetX;
                        double locationY = sfce.RenderTransform.Value.OffsetY;
                        var pointlist = new[]
                                            {
                                                new Point(locationX, locationY),
                                                new Point(locationX + thrsldX, locationY),
                                                new Point(locationX + (2*thrsldX), locationY),
                                                new Point(locationX + (3*thrsldX), locationY),
                                                new Point(locationX + (4*thrsldX), locationY),

                                                //new Point(locationX, locationY + thrsldY),
                                                //new Point(locationX + (2*thrsldX), locationY + thrsldY),

                                                new Point(locationX, locationY + (2*thrsldY)),
                                                new Point(locationX + (thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (2*thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (3*thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (4*thrsldX), locationY + (2*thrsldY))
                                            };
                        foreach (Point cpoint in pointlist)
                        {
                            PointTracker(cpoint, new Point(_width, _height), ref data, isNegative);
                            fieldLines.AddRange(data);
                        }
                    }
                }
            }

            return fieldLines;
        }

        public List<Point> GetFieldLinesParallel()
        {
            const int thrsldX = 16;
            const int thrsldY = 16;
            var fieldLines = new List<Point>();


            Parallel.ForEach(_charges, charge =>
                                           {
                                               if (charge.IsActive)
                                               {
                                                   var data = new List<Point>();
                                                   bool isNegative = charge.Type == Charge.ChargeType.Negative;
                                                   var pointlist = new[]
                                                                       {
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X + thrsldX,
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y + thrsldY),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y + thrsldY),
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y + (2*thrsldY)),
                                                                           new Point(charge.Location.X + thrsldX,
                                                                                     charge.Location.Y + (2*thrsldY)),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y + (2*thrsldY))
                                                                       };
                                                   foreach (Point cpoint in pointlist)
                                                   {
                                                       PointTracker(cpoint, new Point(_width, _height), ref data,
                                                                    isNegative);
                                                       fieldLines.AddRange(data);
                                                   }
                                               }
                                           });


            return fieldLines;
        }

        public BitmapSource GenerateFieldLines(bool isQuick)
        {
            //Doesn't need to Solve before!
            int thrsldX = 16;
            int thrsldY = 16;
            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            Filler(_height, ref pixelData, rawStride, Color.FromRgb(0, 0, 0));
            if (_charges.Count != 0)
            {
                foreach (Charge charge in _charges)
                {
                    if (charge.IsActive)
                    {
                        var data = new List<Point>();
                        bool isNegative = charge.Type == Charge.ChargeType.Negative;
                        var pointlist = new[]
                                            {
                                                new Point(charge.Location.X, charge.Location.Y),
                                                new Point(charge.Location.X + thrsldX, charge.Location.Y),
                                                new Point(charge.Location.X + (2*thrsldX), charge.Location.Y),
                                                new Point(charge.Location.X, charge.Location.Y + thrsldY),
                                                new Point(charge.Location.X + (2*thrsldX), charge.Location.Y + thrsldY),
                                                new Point(charge.Location.X, charge.Location.Y + (2*thrsldY)),
                                                new Point(charge.Location.X + thrsldX, charge.Location.Y + (2*thrsldY)),
                                                new Point(charge.Location.X + (2*thrsldX),
                                                          charge.Location.Y + (2*thrsldY))
                                            };
                        foreach (Point cpoint in pointlist)
                        {
                            if (isQuick)
                            {
                                PointTrackerQuick(cpoint, new Point(_width, _height), ref data, isNegative);
                            }
                            else
                            {
                                PointTracker(cpoint, new Point(_width, _height), ref data, isNegative);
                            }


                            foreach (Point point in data)
                            {
                                Color tempcolor = Colors.Gold;
                                if (charge.Type == Charge.ChargeType.Negative)
                                    tempcolor = Colors.MediumVioletRed;

                                if ((int) point.X != _width && (int) point.Y != _height)
                                {
                                    SetPixel((int) point.X, (int) point.Y, tempcolor, ref pixelData, rawStride);
                                    //if ((int)point.Y < (_height - 2))
                                    //{
                                    //    SetPixel((int) point.X, (int) point.Y + 1, tempcolor, ref pixelData, rawStride);
                                    //    //SetPixel((int)point.X, (int)point.Y + 2, tempcolor, ref pixelData, rawStride);
                                    //}
                                    //if ((int)point.Y > 2)
                                    //{
                                    //    SetPixel((int)point.X, (int)point.Y - 1, tempcolor, ref pixelData, rawStride);
                                    //    //SetPixel((int) point.X, (int) point.Y - 2, tempcolor, ref pixelData, rawStride);

                                    //}
                                }
                            }
                        }
                    }
                }
            }
            if (_surfaces.Count != 0)
            {
                thrsldX = 50;
                thrsldY = 16;
                foreach (Surface sfce in _surfaces)
                {
                    if (sfce.MyCharge.IsActive)
                    {
                        var data = new List<Point>();
                        bool isNegative = sfce.MyCharge.Type == Charge.ChargeType.Negative;
                        double locationX = sfce.RenderTransform.Value.OffsetX;
                        double locationY = sfce.RenderTransform.Value.OffsetY;
                        var pointlist = new[]
                                            {
                                                new Point(locationX, locationY),
                                                new Point(locationX + thrsldX, locationY),
                                                new Point(locationX + (2*thrsldX), locationY),
                                                new Point(locationX + (3*thrsldX), locationY),
                                                new Point(locationX + (4*thrsldX), locationY),

                                                //new Point(locationX, locationY + thrsldY),
                                                //new Point(locationX + (2*thrsldX), locationY + thrsldY),

                                                new Point(locationX, locationY + (2*thrsldY)),
                                                new Point(locationX + (thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (2*thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (3*thrsldX), locationY + (2*thrsldY)),
                                                new Point(locationX + (4*thrsldX), locationY + (2*thrsldY))
                                            };
                        foreach (Point cpoint in pointlist)
                        {
                            if (isQuick)
                            {
                                PointTrackerQuick(cpoint, new Point(_width, _height), ref data, isNegative);
                            }
                            else
                            {
                                PointTracker(cpoint, new Point(_width, _height), ref data, isNegative);
                            }


                            foreach (Point point in data)
                            {
                                Color tempcolor = Colors.Red;
                                if (sfce.MyCharge.Type == Charge.ChargeType.Negative)
                                    tempcolor = Colors.DodgerBlue;

                                if ((int) point.X != _width && (int) point.Y != _height)
                                {
                                    SetPixel((int) point.X, (int) point.Y, tempcolor, ref pixelData, rawStride);
                                    //if ((int)point.Y < (_height - 2))
                                    //{
                                    //    SetPixel((int) point.X, (int) point.Y + 1, tempcolor, ref pixelData, rawStride);
                                    //    //SetPixel((int)point.X, (int)point.Y + 2, tempcolor, ref pixelData, rawStride);
                                    //}
                                    //if ((int)point.Y > 2)
                                    //{
                                    //    SetPixel((int)point.X, (int)point.Y - 1, tempcolor, ref pixelData, rawStride);
                                    //    //SetPixel((int) point.X, (int) point.Y - 2, tempcolor, ref pixelData, rawStride);

                                    //}
                                }
                            }
                        }
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource GenerateFieldLinesParallel()
        {
            //Doesn't need to Solve before!
            const int thrsldX = 16;
            const int thrsldY = 16;
            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            Filler(_height, ref pixelData, rawStride, Color.FromRgb(0, 0, 0));

            Parallel.ForEach(_charges, charge =>
                                           {
                                               if (charge.IsActive)
                                               {
                                                   var data = new List<Point>();
                                                   bool isNegative = charge.Type == Charge.ChargeType.Negative;
                                                   var pointlist = new[]
                                                                       {
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X + thrsldX,
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y),
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y + thrsldY),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y + thrsldY),
                                                                           new Point(charge.Location.X,
                                                                                     charge.Location.Y + (2*thrsldY)),
                                                                           new Point(charge.Location.X + thrsldX,
                                                                                     charge.Location.Y + (2*thrsldY)),
                                                                           new Point(charge.Location.X + (2*thrsldX),
                                                                                     charge.Location.Y + (2*thrsldY))
                                                                       };
                                                   foreach (Point cpoint in pointlist)
                                                   {
                                                       PointTracker(cpoint, new Point(_width, _height), ref data,
                                                                    isNegative);

                                                       foreach (Point point in data)
                                                       {
                                                           Color tempcolor = Colors.Gold;
                                                           if (charge.Type == Charge.ChargeType.Negative)
                                                               tempcolor = Colors.MediumVioletRed;

                                                           if ((int) point.X != _width && (int) point.Y != _height)
                                                           {
                                                               SetPixel((int) point.X, (int) point.Y, tempcolor,
                                                                        ref pixelData, rawStride);
                                                               //if ((int)point.Y < (_height - 2))
                                                               //{
                                                               //    SetPixel((int) point.X, (int) point.Y + 1, tempcolor, ref pixelData, rawStride);
                                                               //    //SetPixel((int)point.X, (int)point.Y + 2, tempcolor, ref pixelData, rawStride);
                                                               //}
                                                               //if ((int)point.Y > 2)
                                                               //{
                                                               //    SetPixel((int)point.X, (int)point.Y - 1, tempcolor, ref pixelData, rawStride);
                                                               //    //SetPixel((int) point.X, (int) point.Y - 2, tempcolor, ref pixelData, rawStride);

                                                               //}
                                                           }
                                                       }
                                                   }
                                               }
                                           });


            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        private void PointTracker(Point startpoint, Point boundaries, ref List<Point> data, bool isNegative)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            data = new List<Point> {startpoint};
            Point currentPoint = startpoint;
            var lastVector = new Vector();
            while (true)
            {
                Vector amountToMove = Helper.NormalizeVector(GetForceAtDesirePosition(currentPoint.X, currentPoint.Y));


                //var amountToMove = (GetForceAtDesirePosition(currentPoint.X, currentPoint.Y)) * 0.5;


                if (isNegative)
                    amountToMove = Helper.InverseVector(amountToMove);

                currentPoint = new Point(currentPoint.X + amountToMove.X, currentPoint.Y - amountToMove.Y);


                //if (Math.Abs(( amountToMove.X)) > 100000 &&
                //    Math.Abs((amountToMove.Y)) > 100000)
                //{
                //    break;
                //}

                if (Math.Abs((lastVector.X + amountToMove.X)) <= 0.000001 &&
                    Math.Abs((lastVector.Y + amountToMove.Y)) <= 0.000001)
                {
                    break;
                }
                if (currentPoint.X < 0 || currentPoint.Y < 0 || currentPoint.X > boundaries.X ||
                    currentPoint.Y > boundaries.Y)
                {
                    break;
                }

                data.Add(currentPoint);
                lastVector = amountToMove;
            }
        }

        private void PointTrackerQuick(Point startpoint, Point boundaries, ref List<Point> data, bool isNegative)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            data = new List<Point> {startpoint};
            Point currentPoint = startpoint;
            var lastVector = new Vector();
            while (true)
            {
                //var amountToMove = Helper.NormalizeVector(GetForceAtDesirePosition(currentPoint.X, currentPoint.Y));


                Vector amountToMove = (GetForceAtDesirePosition(currentPoint.X, currentPoint.Y))*0.2;


                if (isNegative)
                    amountToMove = Helper.InverseVector(amountToMove);

                currentPoint = new Point(currentPoint.X + amountToMove.X, currentPoint.Y - amountToMove.Y);


                //if (Math.Abs(( amountToMove.X)) > 100000 &&
                //    Math.Abs((amountToMove.Y)) > 100000)
                //{
                //    break;
                //}

                if (Math.Abs((lastVector.X + amountToMove.X)) <= 0.000001 &&
                    Math.Abs((lastVector.Y + amountToMove.Y)) <= 0.000001)
                {
                    break;
                }
                if (currentPoint.X < 0 || currentPoint.Y < 0 || currentPoint.X > boundaries.X ||
                    currentPoint.Y > boundaries.Y)
                {
                    break;
                }

                data.Add(currentPoint);
                lastVector = amountToMove;
            }
        }

        public Vector GetForceAtDesirePosition(double xPos, double yPos, int fault = 16)
        {
            if (_charges == null && _surfaces == null)
            {
                return new Vector(0, 0);
            }
            if (_charges != null && (_charges.Count == 0 && _surfaces.Count == 0))
            {
                return new Vector(0, 0);
            }

            var totalForceVec = new Vector(0, 0);
            if (_charges != null && _charges.Count != 0)
            {
                foreach (Charge charge in _charges)
                {
                    if (charge.IsActive)
                    {
                        double distanceX = xPos - charge.Location.X;
                        double distanceY = charge.Location.Y - yPos;

                        // Calculating Height and Width of Displayed charge Object itself!
                        distanceX -= fault;
                        distanceY += fault;

                        var tempVec = new Vector(distanceX, distanceY);
                        double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                        //Consideration!
                        if (charge.Type == Charge.ChargeType.Negative)
                        {
                            tempVec = Helper.InverseVector(tempVec);
                        }

                        tempVec = Helper.NormalizeVector(tempVec);
                        tempVec = Helper.AmountOForce(charge.ElectricCharge, amountOfDistance)*tempVec;

                        totalForceVec += tempVec;
                    }
                }
            }
            if (_surfaces != null)
            {
                foreach (Surface surface in _surfaces)
                {
                    if (surface.MyCharge.IsActive)
                    {
                        double amountOfChargePerPoint = surface.MyCharge.ElectricCharge/10;
                        var surfaceLocation = new Point(surface.RenderTransform.Value.OffsetX,
                                                        surface.RenderTransform.Value.OffsetY);
                        double parresh = surface.Width/10;
                        double taVasat = surface.Height/2;

                        for (int i = 0; i < 10; i++)
                        {
                            var here = new Point(surfaceLocation.X + (i*parresh), surfaceLocation.Y + taVasat);

                            double distanceX = xPos - here.X;
                            double distanceY = here.Y - yPos;

                            var tempVec = new Vector(distanceX, distanceY);
                            double amountOfDistance = Helper.VectorMagnitude(tempVec)/100;

                            if (surface.MyCharge.Type == Charge.ChargeType.Negative)
                            {
                                tempVec = Helper.InverseVector(tempVec);
                            }
                            tempVec = Helper.NormalizeVector(tempVec);
                            tempVec = Helper.AmountOForce(amountOfChargePerPoint, amountOfDistance)*tempVec;

                            totalForceVec += tempVec;
                        }
                    }
                }
            }

            return totalForceVec;
        }

        public void DrawGray8Image(string saveLocation)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            const double dpi = 96;
            var pixelData = new byte[_width*_height];

            for (int i = 0; i < _data.Length; i++)
            {
                pixelData[i] = (byte) (Helper.Clamp(_data[i]*4, 0, 255));
            }

            BitmapSource bmpSource = BitmapSource.Create(_width, _height, dpi, dpi, PixelFormats.Gray8, null, pixelData,
                                                         _width);

            var stream = new FileStream(saveLocation, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(bmpSource));
            encoder.Save(stream);
            stream.Close();
        }

        public void DrawRgb24Image(string saveLocation)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            Filler(_height, ref pixelData, rawStride, Color.FromRgb(255, 212, 10));

            for (int i = 0; i < _data.Length; i++)
            {
                {
                    SetPixel((i%_width), (i/_width),
                             Color.FromRgb((byte) (Helper.Clamp(_data[i]*4, 0, 255)), 10, 10), ref pixelData, rawStride);
                }
            }


            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            var stream = new FileStream(saveLocation, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            stream.Close();
        }

        public void DrawRgb24Image(string saveLocation, Color firstColor, Color secondColor, int maxValue)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            //var max= Helper.GetMax(_data);
            //var mode = Helper.Mode(_data);
            for (int i = 0; i < _data.Length; i++)
            {
                double percentage = (_data[i])/maxValue;

                if (percentage > 1) percentage = 1;

                SetPixel((i%_width), (i/_width), Helper.GetCurrentColor(percentage, firstColor, secondColor),
                         ref pixelData, rawStride);
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            var stream = new FileStream(saveLocation, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            stream.Close();
        }

        public void DrawRgb24Image(string saveLocation, Color bgColor)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            for (int i = 0; i < _data.Length; i++)
            {
                {
                    SetPixel((i%_width), (i/_width),
                             Color.FromRgb(bgColor.R, (byte) (255 - Helper.Clamp(_data[i]*4, 0, 255)),
                                           (byte) (255 - Helper.Clamp(_data[i]*4, 0, 225))), ref pixelData, rawStride);
                }
            }


            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            var stream = new FileStream(saveLocation, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            stream.Close();
        }

        public void CustomeColorizer(string saveLocation, List<Colorizer> colorpalette)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            if (colorpalette.Count <= 1)
                throw new InvalidDataException("You must choose at least 2 Colors");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            foreach (Colorizer colorizer in colorpalette)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    if (Helper.IsInBetween(_data[i], colorizer.Min, colorizer.Max))
                    {
                        SetPixel((i%_width), (i/_width), colorizer.Color, ref pixelData, rawStride);
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            var stream = new FileStream(saveLocation, FileMode.Create);
            var encoder = new JpegBitmapEncoder {QualityLevel = 100};
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            stream.Close();
        }

        public BitmapSource CustomeColorizer(List<Colorizer> colorpalette)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            if (colorpalette.Count <= 1)
                throw new InvalidDataException("You must choose at least 2 Colors");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            foreach (Colorizer colorizer in colorpalette)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    if (Helper.IsInBetween(_data[i], colorizer.Min, colorizer.Max))
                    {
                        SetPixel((i%_width), (i/_width), colorizer.Color, ref pixelData, rawStride);
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource CustomeColorizerSolver(List<Colorizer> colorpalette)
        {
            //Sort List!
            //Note:: Sort list here
            //
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            if (colorpalette == null)
                throw new InvalidDataException("Error!");

            if (colorpalette.Count <= 1)
                throw new InvalidDataException("You must choose at least 2 Colors");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            foreach (Colorizer colorizer in colorpalette)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    if (Helper.IsInBetween(_data[i], colorizer.Min, colorizer.Max))
                    {
                        //SetPixel((i % _width), (i / _width), colorizer.Color, ref pixelData, rawStride);


                        double percentage = (_data[i] - colorizer.Min)/(colorizer.Max - colorizer.Min);

                        if (percentage > 1) percentage = 1;

                        SetPixel((i%_width), (i/_width),
                                 Helper.GetCurrentColor(percentage, Helper.GetNextColor(colorpalette, colorizer).Color,
                                                        colorizer.Color),
                                 ref pixelData, rawStride);
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource CustomeColorizerSolverWithLines(List<Colorizer> colorpalette, Color lineColors)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            if (colorpalette == null)
                throw new InvalidDataException("Error!");

            if (colorpalette.Count <= 1)
                throw new InvalidDataException("You must choose at least 2 Colors");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            foreach (Colorizer colorizer in colorpalette)
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    if (Helper.IsInBetween(_data[i], colorizer.Min, colorizer.Max))
                    {
                        //SetPixel((i % _width), (i / _width), colorizer.Color, ref pixelData, rawStride);


                        double percentage = (_data[i] - colorizer.Min)/(colorizer.Max - colorizer.Min);

                        if (percentage > 1) percentage = 1;

                        SetPixel((i%_width), (i/_width),
                                 Helper.GetCurrentColor(percentage, Helper.GetNextColor(colorpalette, colorizer).Color,
                                                        colorizer.Color),
                                 ref pixelData, rawStride);
                    }
                }
            }

            List<Point> linedata = GetFieldLines();
            foreach (Point point in linedata)
            {
                if ((int) point.X != _width && (int) point.Y != _height)
                    SetPixel((int) point.X, (int) point.Y, lineColors, ref pixelData, rawStride);
            }


            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource GetImageRgb24WithLines(Color firstColor, Color secondColor, int maxValue, bool allowMark,
                                                   int specificValue, Color specificColor)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            for (int i = 0; i < _data.Length; i++)
            {
                if (Helper.Thresholder(specificValue, _data[i], 5) && allowMark)
                {
                    SetPixel((i%_width), (i/_width), specificColor, ref pixelData, rawStride);
                }
                else
                {
                    double percentage = (_data[i])/maxValue;

                    if (percentage > 1) percentage = 1;

                    SetPixel((i%_width), (i/_width), Helper.GetCurrentColor(percentage, firstColor, secondColor),
                             ref pixelData, rawStride);
                }
            }

            List<Point> linedata = GetFieldLines();
            foreach (Point point in linedata)
            {
                if ((int) point.X != _width && (int) point.Y != _height)
                    SetPixel((int) point.X, (int) point.Y, Colors.Black, ref pixelData, rawStride);
            }
            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource GetImageRgb24(Color firstColor, Color secondColor, int maxValue, bool allowMark,
                                          int specificValue, Color specificColor)
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            for (int i = 0; i < _data.Length; i++)
            {
                if (Helper.Thresholder(specificValue, _data[i], 5) && allowMark)
                {
                    SetPixel((i%_width), (i/_width), specificColor, ref pixelData, rawStride);
                }
                else
                {
                    double percentage = (_data[i])/maxValue;

                    if (percentage > 1) percentage = 1;

                    SetPixel((i%_width), (i/_width), Helper.GetCurrentColor(percentage, firstColor, secondColor),
                             ref pixelData, rawStride);
                }
            }

            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        public BitmapSource GetImageRgb24()
        {
            if (_data == null)
                throw new InvalidDataException("You must Solve the case First!");

            PixelFormat pf = PixelFormats.Rgb24;
            const double dpi = 96;
            int rawStride = (_width*pf.BitsPerPixel + 7)/8;
            var pixelData = new byte[rawStride*_height];

            Filler(_height, ref pixelData, rawStride, Color.FromRgb(255, 212, 10));

            for (int i = 0; i < _data.Length; i++)
            {
                SetPixel((i%_width), (i/_width),
                         Color.FromRgb((byte) (Helper.Clamp(_data[i]*4, 0, 255)), 10, 10), ref pixelData, rawStride);
            }


            BitmapSource bitmap = BitmapSource.Create(_width, _height, dpi, dpi, pf, null, pixelData, rawStride);

            return bitmap;
        }

        //public IEnumerable<Point> ChargeChargeDistance(FreeCharge freeCharge, PositiveCharge positiveCharge,
        //                                                         int duration, Point startLocation)
        //{
        //    var returnList = new List<Point>();
        //    var data = FreeChargeLocationTracker(freeCharge, duration, startLocation);
        //    foreach (var points in data)
        //    {
        //        var temp = new Point(positiveCharge.RenderTransform.Value.OffsetX,
        //                             positiveCharge.RenderTransform.Value.OffsetY);

        //        var newpoint = new Point(points.Time, Helper.Distance(points.Position, temp));

        //        returnList.Add(newpoint);
        //    }


        //    return returnList;
        //}

        public IEnumerable<PointStatistics> FreeChargeLocationTracker(FreeCharge freeCharge, int duration,
                                                                      Point startLocation)
        {
            var returnList = new List<PointStatistics>();
            int counter = 1;
            bool forceEnd = false;
            while (counter <= duration && !forceEnd)
            {
                //Calculation
                //freeCharge.RenderTransform.Value.OffsetX = startLocation.X;
                //freeCharge.RenderTransform.Value.OffsetX = startLocation.Y;

                double fcX = freeCharge.RenderTransform.Value.OffsetX;
                double fcY = freeCharge.RenderTransform.Value.OffsetY;
                Vector moveVector = GetForceAtDesirePosition(fcX, fcY, 0);

                MainWindow.Instance.MoveIt(moveVector, freeCharge.LastVector, freeCharge);

                int nDist = MainWindow.Instance.NearestDistancetoNegative(freeCharge);
                double mag = Helper.VectorMagnitude(moveVector);
                if (Math.Abs(nDist - mag) < 40)
                {
                    forceEnd = true;
                }
                else if (Math.Abs((int) freeCharge.RenderTransform.Value.OffsetX) >= 900 ||
                         Math.Abs((int) freeCharge.RenderTransform.Value.OffsetY) >= 900)
                {
                    forceEnd = true;
                }
                freeCharge.LastVector += moveVector;


                var newPoint = new PointStatistics(new Point(fcX, fcY), counter, 0, 0,
                                                   Helper.VectorMagnitude(GetForceAtDesirePosition(fcX, fcY, 1)));
                returnList.Add(newPoint);

                counter++;
            }

            return returnList;
        }

        public IEnumerable<Point> ChargeChargeDistance(FreeCharge freeCharge, Point orgin,
                                                       int duration, Point startLocation)
        {
            var returnList = new List<Point>();
            IEnumerable<PointStatistics> data = FreeChargeLocationTracker(freeCharge, duration, startLocation);
            foreach (PointStatistics points in data)
            {
                var newpoint = new Point(points.Time, Helper.Distance(points.Position, orgin));

                returnList.Add(newpoint);
            }


            return returnList;
        }

        private void SetPixel(int x, int y, Color c, ref byte[] buffer, int rawStride)
        {
            int xIndex = x*3;
            int yIndex = y*rawStride;
            buffer[xIndex + yIndex] = c.R;
            buffer[xIndex + yIndex + 1] = c.G;
            buffer[xIndex + yIndex + 2] = c.B;
        }

        private void Filler(int height, ref byte[] pixelData, int rawStride, Color color)
        {
            for (int y = 0; y < height; y++)
            {
                int yIndex = y*rawStride;
                for (int x = 0; x < rawStride; x += 3)
                {
                    pixelData[x + yIndex] = color.R;
                    pixelData[x + yIndex + 1] = color.G;
                    pixelData[x + yIndex + 2] = color.B;
                }
            }
        }

        #region Nested type: ColorRange

        public class ColorRange
        {
            public ColorRange()
            {
                Min = 0;
                Max = 1000;
                Color = Colors.Red;
            }

            public ColorRange(uint min, uint max, Color color)
            {
                Min = min;
                Max = max;
                Color = color;
            }

            public uint Min { get; set; }
            public uint Max { get; set; }
            public Color Color { get; set; }
        }

        #endregion

        #region Nested type: Colorizer

        public struct Colorizer
        {
            public Color Color;
            public int Max;
            public int Min;
        }

        #endregion
    }
}