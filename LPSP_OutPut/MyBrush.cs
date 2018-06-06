using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HUST_OutPut
{
    class MyBrush : IComparable<MyBrush>
    {
        public Int32 priority { get; set; }
        public Brush myBrush { get; set; }
        public String describe;
        public MyBrush(Brush myBrush, Int32 priority, String describe)
        {
            this.myBrush = myBrush;
            this.priority = priority;
            this.describe = describe;
        }

        public int CompareTo(MyBrush other)
        {
            return this.priority - other.priority;
        }
    }
}
