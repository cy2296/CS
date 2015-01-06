using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gES_theoES : IndicatorObject 
    {
		public _gES_theoES(object _ctx):base(_ctx)
        {
            spreadLookBack = 10;               
        }
        private VariableSeries<Double> m_theoES;
        private VariableSeries<Double> m_spread;

        private IPlotObject plot1;

        private _gTurnMetric_Delta m_delta_indic;
        //private _gVX_fv m_theoVX_indic;
        private ISeries<double> esFv { get; set; }
        private ISeries<double> vxFv { get; set; }
        // private variables for delta indicator
        private int delta_length = 100;
        private double delta_vxWeight = 0.6;
        private double delta_upRatio = 0.5;
        /////////////////////////////////////////
        [Input]
        public int spreadLookBack { get; set; }
		protected override void Create()
        {
            m_theoES = new VariableSeries<double>(this);
            m_spread = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("theoES_es_spread", EPlotShapes.Histogram, Color.Cyan));
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
            if (Bars.CurrentBar >= delta_length)
            {
                var dollarChangeVX = 1000 * (vxFv[0] - vxFv[spreadLookBack]);
                var theoDollarChangeES = (m_delta_indic.Plots[6].Values[spreadLookBack] != 0) ?
                    -1 * m_delta_indic.Plots[6].Values[spreadLookBack] * dollarChangeVX : 0;
                m_theoES.Value = esFv[spreadLookBack] + (theoDollarChangeES / 50);
            }
            else
            {
                m_theoES.Value = esFv[0];
            }
            m_spread.Value = esFv[0] - m_theoES.Value;
            plot1.Set(0, m_spread.Value);
		}
	}
}