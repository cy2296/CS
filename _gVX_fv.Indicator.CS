using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gVX_fv : IndicatorObject {
		public _gVX_fv(object _ctx):base(_ctx)
        {
            //delta_length = 100;
            //delta_vxWeight = 0.6;
            //delta_upRatio = 0.5;
            spreadLookBack = 10;        
        }
        private VariableSeries<Double> m_theoVX;
        private IPlotObject plot1;
        private _gTurnMetric_Delta m_delta_indic;

        private ISeries<double> esFv { get; set; }
        private ISeries<double> vxFv { get; set; }

        // private variables for delta indicator
        private int delta_length = 100;
        private double delta_vxWeight = 0.6;
        private double delta_upRatio = 0.5;
        /////////////////////////////////////////

        //[Input]
        //public int delta_length { get; set; }
        //[Input]
        //public double delta_vxWeight { get; set; }
        //[Input]
        //public double delta_upRatio { get; set; }
        [Input]
        public int spreadLookBack { get; set; }

        protected override void Create() {
            m_theoVX = new VariableSeries<double>(this);
			plot1 = AddPlot(new PlotAttributes("theoVX", EPlotShapes.Line, Color.Red));
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
                var dollarChangeES = 50 * (esFv[0] - esFv[spreadLookBack]);
                var theoDollarChangeVX = (m_delta_indic.Plots[6].Values[spreadLookBack] != 0) ?
                    -1 / m_delta_indic.Plots[6].Values[spreadLookBack] * dollarChangeES : 0;
                m_theoVX.Value = vxFv[spreadLookBack] + (theoDollarChangeVX / 1000);
            }
            else
            {
                m_theoVX.Value = vxFv[0];
            }
            plot1.Set(0, m_theoVX.Value);
		}
	}
}