using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public class Car
    {
        public int Speed { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int MaxSpeed { get; set; } = 250;

        public static int StandardMaxSpeed { get; set; } = 250;

        public void Accelerate(long km)
        {
            Speed += Convert.ToInt32(km);
        }

        public void Accelerate(int miles)
        {
            Speed += Convert.ToInt32(miles * 1.6);
        }
    }

    public class MyError : Exception
    {
        public string Reason { get; set; }
        public MyError(string message, string reason) : base(message)
        {
            Reason = reason;
        }
    }

}
