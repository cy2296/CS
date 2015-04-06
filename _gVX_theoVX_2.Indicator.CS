using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gVX_theoVX_2 : IndicatorObject {
		public _gVX_theoVX_2(object _ctx):base(_ctx){}
        private VariableSeries<Double> m_closesES;
        private VariableSeries<Double> m_closesVX;
        private VariableSeries<Double> m_theoVX;
        private VariableSeries<Double> m_spread;
        private IPlotObject plot1;

        private _gTurnMetric_Delta m_delta_indic;

        private ISeries<double> esFv { get; set; }
        private ISeries<double> vxFv { get; set; }
        // private variables for delta indicator
        private int delta_length = 100;
        private double delta_vxWeight = 0.6;
        private double delta_upRatio = 0.5;
        /////////////////////////////////////////

		protected override void Create()
        {
            m_closesES = new VariableSeries<double>(this);
            m_closesVX = new VariableSeries<double>(this);
            m_theoVX = new VariableSeries<double>(this);
            m_spread = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("theoVX_vx_spread", EPlotShapes.Histogram, Color.Cyan));
            //plot1 = AddPlot(new PlotAttributes("theoVX_vx_spread", EPlotShapes.Line, Color.Yellow));
            m_delta_indic = (_gTurnMetric_Delta)AddIndicator("_gTurnMetric_Delta");
            m_delta_indic.length = delta_length;
            m_delta_indic.vxWeight = delta_vxWeight;
            m_delta_indic.upRatio = delta_upRatio;
        }
		protected override void StartCalc()
        {
            esFv = BarsOfData(3).Close;
            vxFv = BarsOfData(4).Close;
        }
		protected override void CalcBar()
        {
            TimeSpan timeDiff = Bars.Time[0].Subtract(Bars.Time[1]);
            if (timeDiff.TotalHours >= 10)
            {
                m_closesES.Value = esFv[1];
                m_closesVX.Value = vxFv[1];
            }
            else
            {
                m_closesES.Value = m_closesES[1];
                m_closesVX.Value = m_closesVX[1];
            }

            var dollarChangeES = 50 * (esFv[0] - m_closesES.Value);
            var theoDollarChangeVX = (m_delta_indic.Plots[6].Values[0] != 0) ?
                    -1 / m_delta_indic.Plots[6].Values[0] * dollarChangeES : 0;
            m_theoVX.Value = m_closesVX.Value + (theoDollarChangeVX / 1000);
            m_spread.Value = vxFv[0] - m_theoVX.Value;
            plot1.Set(0, m_spread.Value);
            //plot1.Set(0, m_theoVX.Value);
        }
	}
}