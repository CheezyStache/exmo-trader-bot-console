using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Decision
{
    class FlowLine
    {
        public LineVector UpperLine { get; set; }
        public LineVector LowerLine { get; set; }
    }

    class LineVector
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
    }

    enum FlowLinePos
    {
        Higher,
        Lower,
        InArea
    }
}
