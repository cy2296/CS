using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gVX_fv : IndicatorObject {
		public _gVX_fv(object _ctx):base(_ctx){}
		
        private IPlotObject plot1;
        //private _gTurnMetric_Delta m_delta_indic;
        private _gTurnMetric_Fv m_dChange_indic;

        [Input]
        public int spreadLookBack { get; set; }

        protected override void Create() {
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
            m_dChange_indic = (_gTurnMetric_Fv)AddIndicator("dChange");

        }
		protected override void StartCalc() {
            m_dChange_indic.length = 100;
            m_dChange_indic.vxWeight = 0.6;
            m_dChange_indic.upRatio = 0.5;
            m_dChange_indic.spreadLookBack = spreadLookBack;
		}
		protected override void CalcBar(){
			// indicator logic 
            var theoVX = m_dChange_indic.Plots[2].Values[0] + BarsOfData(4).Close[0];
            plot1.Set(theoVX);
		}
	}
}