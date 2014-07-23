using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WindowsFormsApplication1
{
    static class GestureRecognizer
    {
        static int?   lastAngle = null;
        static Point? lastPoint = null;
        
        static private int GetDistance(Point pointFrom, Point pointTo)
        {
            return (int)Math.Sqrt(Math.Pow(pointTo.X - pointFrom.X, 2.0f) + Math.Pow(pointTo.Y - pointFrom.Y, 2.0f));
        }
        
        static private int GetAngleInDegree(Point pointFrom, Point pointTo)
        {
            float radial = (float)Math.Atan2
            (
                -1.0f * (pointTo.Y - pointFrom.Y),
                1.0f * (pointTo.X - pointFrom.X)
            );
            
            if (radial < 0.0f)
            {
                radial = (float)(2.0f * Math.PI) + radial;
            }
            
            return (int)(180.0f * radial / Math.PI);
        }
    
        static public void Reset()
        {
            lastAngle = null;
            lastPoint = null;
        }
    
        static public bool Recognize(Point point)
        {
            if (lastPoint.HasValue == false)
            {
                lastPoint = point;
                return false;
            }
            else if (GetDistance(lastPoint.Value, point) < 5.0f)
            {
                return false;
            }
            
            if (lastAngle.HasValue == false)
            {
                lastAngle = GetAngleInDegree(lastPoint.Value, point);
                return false;
            }
            
            int angle = GetAngleInDegree(lastPoint.Value, point);
            int diffAngle = angle - lastAngle.Value;
            
            if (diffAngle > 180)
            {
                diffAngle -= 360;
            }
            
            if (CircleRecognizer.Recognize(diffAngle) == true)
            {
                Console.WriteLine(DateTime.Now + " : Circle");
                
                CircleRecognizer.Reset();
                GestureRecognizer.Reset();
                
                return true;
            }
            else
            {
                lastAngle = angle;
                lastPoint = point;    

                return false;
            }    
        }
    }
    
    static class CircleRecognizer
    {
        static int? sumAngle = null;
        
        static public void Reset()
        {
            sumAngle = null;
        }
        
        static public bool Recognize(int angle)
        {
            if (sumAngle.HasValue == false)
            {
                sumAngle = angle;
            }
            
            sumAngle += angle;

            if (Math.Abs(sumAngle.Value) > 300)
            {
                return true;
            }
            else
            {
                if (sumAngle > 360)
                {
                    sumAngle -= 360;
                }
                else if (sumAngle < -360)
                {
                    sumAngle += 360;
                }

                return false;
            }
        }
    }
}
