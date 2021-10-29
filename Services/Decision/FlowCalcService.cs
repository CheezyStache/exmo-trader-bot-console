using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Decision;

namespace exmo_trader_bot_console.Services.Decision
{
    class FlowCalcService: IFlowCalcService
    {
        public FlowLine CalcFlowLine(Candle[] candles)
        {
            FlowLine flowLine = new FlowLine();

            FindTrendLine(candles, GetHighPrice, FlowLinePos.Higher, flowLine.UpperLine);

            if (flowLine.UpperLine == null)
                throw new Exception("Can't find upper line of trend");

            FindTrendLine(candles, GetLowPrice, FlowLinePos.Lower, flowLine.LowerLine);

            if (flowLine.LowerLine == null)
                throw new Exception("Can't find lower line of trend");

            return flowLine;
        }

        public FlowLinePos GetPricePosition(FlowLine flowLine, double price, int candleIndex)
        {
            var upperLine = CheckSingleLine(flowLine.UpperLine, price, candleIndex);
            if (upperLine == FlowLinePos.Higher || upperLine == FlowLinePos.InArea) return FlowLinePos.Higher;

            var lowerLine = CheckSingleLine(flowLine.LowerLine, price, candleIndex);
            if (lowerLine == FlowLinePos.Lower || lowerLine == FlowLinePos.InArea) return FlowLinePos.Lower;

            return FlowLinePos.InArea;
        }

        public double FlowPercentRange(FlowLine flowLine, int index)
        {
            var points = new double[2];
            points[0] = GetValueOnLine(flowLine.UpperLine, index);
            points[1] = GetValueOnLine(flowLine.LowerLine, index);

            if (points[1] == 0) return 0;

            return points[0] / points[1] * 100 - 100;
        }

        private double GetValueOnLine(LineVector line, int index)
        {
            return line.Y2 - (line.Y2 - line.Y1) * (line.X2 - index) / (line.X2 - line.X1);
        }


        private FlowLinePos CheckSingleLine(LineVector line, double price, int candleIndex)
        {
            var v1 = new double[] {line.X2 - line.X1, line.Y2 - line.Y1};
            var v2 = new double[] {line.X2 - candleIndex, line.Y2 - price};

            var xp = v1[0] * v2[1] - v1[1] * v2[0]; // cross product

            if (xp > 0) return FlowLinePos.Higher;
            if (xp < 0) return FlowLinePos.Lower;

            return FlowLinePos.InArea;
        }

        private double GetHighPrice(Candle candle)
        {
            return candle.Close > candle.Open ? candle.Close : candle.Open;
        }

        private double GetLowPrice(Candle candle)
        {
            return candle.Close > candle.Open ? candle.Open : candle.Close;
        }

        private void FindTrendLine(Candle[] candles, Func<Candle, double> getPrice, FlowLinePos flowLinePos, LineVector lineVectorRef)
        {
            for (int i = 0; i < candles.Length - 1; i++)
            {
                LineVector lineVector = new LineVector
                {
                    X1 = i,
                    Y1 = getPrice(candles[i]),
                };

                for (int j = candles.Length - 1; j > i; j--)
                {
                    lineVector.X2 = j;
                    lineVector.Y2 = getPrice(candles[j]);

                    var isMostLowLine = true;
                    for (int k = 0; k < candles.Length; k++)
                    {
                        if (CheckSingleLine(lineVector, getPrice(candles[k]), k) == flowLinePos)
                        {
                            isMostLowLine = false;
                            break;
                        }
                    }

                    if (isMostLowLine)
                    {
                        lineVectorRef = lineVector;
                        break;
                    }
                }

                if (lineVectorRef != null)
                    break;
            }
        }
    }
}
