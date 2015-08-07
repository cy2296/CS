using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Indicator
{
    public class _gOutput_2 : IndicatorObject
    {
        public _gOutput_2(object _ctx) : base(_ctx) { }

        private VariableSeries<Double> m_theoVX;
        private VariableSeries<Double> m_vxSize;
        private VariableSeries<Double> m_esSize;

        // debug 
        private VariableSeries<Double> m_delta;

        private IPlotObject plot1;

        private _gVX_fv m_theoVX_indic;
        private _gHedgeRatio m_hedgeRatio_indic;

        private ISeries<double> vxFv { get; set; }

        private System.Text.StringBuilder csv;
        private static object locker = new object();

        // private FileStream fileStream;
        //   UnicodeEncoding uniEncoding;

        // debug 

        private _gTurnMetric_Delta m_delta_indic;

        //[Input]
        //public int delta_length { get; set; }
        //[Input]
        //public double delta_vxWeight { get; set; }
        //[Input]
        //public double delta_upRatio { get; set; }
        //[Input]
        //public int lookBack { get; set; }

        protected override void Create()
        {
            csv = new StringBuilder();

            //  fileStream = new FileStream(
            //   "C:/Users/gyang/Dropbox/mapping/vx1.csv", FileMode.OpenOrCreate,
            //   FileAccess.ReadWrite, FileShare.ReadWrite);

            //   uniEncoding = new UnicodeEncoding();

            //m_spread = new VariableSeries<double>(this);
            m_theoVX = new VariableSeries<double>(this);
            m_vxSize = new VariableSeries<double>(this);
            m_esSize = new VariableSeries<double>(this);

            m_delta = new VariableSeries<double>(this);

            plot1 = AddPlot(new PlotAttributes("theoVX", EPlotShapes.Line, Color.Yellow));

            m_theoVX_indic = (_gVX_fv)AddIndicator("_gVX_fv");
            m_hedgeRatio_indic = (_gHedgeRatio)AddIndicator("_gHedgeRatio");
            m_delta_indic = (_gTurnMetric_Delta)AddIndicator("_gTurnMetric_Delta");
            m_delta_indic.length = 100;
            m_delta_indic.vxWeight = 0.6;
            m_delta_indic.upRatio = 0.5;

            m_hedgeRatio_indic.sizeFactor = 10;
            //m_theoVX_indic.delta_length = 100;
            //m_theoVX_indic.delta_vxWeight = 0.6;
            //m_theoVX_indic.delta_upRatio = 0.5;
            m_theoVX_indic.spreadLookBack = 10;
        }
        protected override void StartCalc()
        {
            // assign inputs 
            vxFv = BarsOfData(4).Close;
        }
        protected override void CalcBar()
        {
            // indicator logic 
            //m_spread.Value = vxFv[0] - m_theoVX_indic.Plots[0].Values[0];
            m_theoVX.Value = m_theoVX_indic.Plots[0].Values[0];
            m_esSize.Value = m_hedgeRatio_indic.Plots[0].Values[0];
            m_vxSize.Value = m_hedgeRatio_indic.Plots[1].Values[0];

            plot1.Set(0, m_theoVX.Value);
            string date = Bars.Time[0].Year.ToString("0000") + "/"
                + Bars.Time[0].Month.ToString("00") + "/"
                + Bars.Time[0].Day.ToString("00");
            string time = Bars.Time[0].Hour.ToString("00") + ":" +
                Bars.Time[0].Minute.ToString("00") + ":" +
                Bars.Time[0].Second.ToString("00");
            //string date = DateTime.Now.Year.ToString("0000")
            //    + DateTime.Now.Month.ToString("00")
            //    + DateTime.Now.Day.ToString("00");
            //string time = DateTime.Now.Hour.ToString("00")
            //    + ":" + DateTime.Now.Minute.ToString("00")
            //    + ":" + DateTime.Now.Second.ToString("00");
            string theoVX = Math.Round(m_theoVX.Value, 2).ToString();
            string vxPos = Math.Round(m_vxSize.Value, 0).ToString();
            string esPos = Math.Round(m_esSize.Value, 0).ToString();
            string delta = Math.Round(m_delta_indic.Plots[6].Values[1], 2).ToString();

            //string barNum = (Bars.CurrentBar+5).ToString();


            var newLine = string.Format("{0}, {1}, {2}, {3}, {4}{5}",
                date, time, theoVX, delta, esPos, System.Environment.NewLine);

            lock (locker)
            {
                csv.Append(newLine);
                //System.IO.File.WriteAllText("C:/Users/gyang/Google Drive/DeltaAnalysis/historical", csv.ToString());
                System.IO.File.WriteAllText("C:/Users/gyang/Dropbox/turnMetrics/mapping/vx2.csv", csv.ToString());
            }
        }

    }
}