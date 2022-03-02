using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Decision;

namespace exmo_trader_bot_console.Services.ChartDrawer
{
    class ChartDrawerService: IChartDrawerService
    {
        public void DrawTrendLinesAndSave(Candle[] candles, FlowLine flowLine, string pair, int resolution)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + pair + "-" + resolution + "-" + DateTime.Now.Ticks + ".png";

            var size = new Size(1000, 1000);
            using var chartPic = new Bitmap(size.Width, size.Height);
            var graphics = Graphics.FromImage(chartPic);

            var xValue = (float)size.Width / (float)candles.Length;
            var yValue = size.Height / ((float)candles.Max(c => c.High) - (float)candles.Min(c => c.Low)) / 1.5f;

            var greenPen = new Pen(Color.Green, 2);
            var redPen = new Pen(Color.Red, 2);
            var greenBrush = new SolidBrush(Color.Green);
            var redBrush = new SolidBrush(Color.Red);

            var yOffset = (float) candles.Min(c => c.Low);

            //draw candles

            for (int i = 0; i < candles.Length; i++)
            {
                var pen = candles[i].Open <= candles[i].Close ? greenPen : redPen;
                var brush = candles[i].Open <= candles[i].Close ? greenBrush : redBrush;

                graphics.DrawLine(pen, i * xValue + xValue / 2, size.Height - ((float) candles[i].Low - yOffset) * yValue,
                    i * xValue + xValue / 2, size.Height - ((float) candles[i].High - yOffset) * yValue);

                var highY = candles[i].Open >= candles[i].Close ? (float)candles[i].Open : (float)candles[i].Close;
                var lowY = candles[i].Open < candles[i].Close ? (float)candles[i].Open : (float)candles[i].Close;

                graphics.DrawRectangle(pen, i * xValue, size.Height - (highY - yOffset) * yValue, xValue,
                    (highY - lowY) * yValue);
                graphics.FillRectangle(brush, i * xValue, size.Height - (highY - yOffset) * yValue, xValue,
                    (highY - lowY) * yValue);
            }

            //draw trend lines

            var trendLinePen = new Pen(Color.DarkMagenta, 4);
            graphics.DrawLine(trendLinePen, (float)flowLine.UpperLine.X1 * xValue, size.Height - ((float)flowLine.UpperLine.Y1 - yOffset) * yValue,
                (float)flowLine.UpperLine.X2 * xValue, size.Height - ((float)flowLine.UpperLine.Y2 - yOffset) * yValue);

            graphics.DrawLine(trendLinePen, (float)flowLine.LowerLine.X1 * xValue, size.Height - ((float)flowLine.LowerLine.Y1 - yOffset) * yValue,
                (float)flowLine.LowerLine.X2 * xValue, size.Height - ((float)flowLine.LowerLine.Y2 - yOffset) * yValue);

            chartPic.Save(path, ImageFormat.Png);
        }
    }
}
