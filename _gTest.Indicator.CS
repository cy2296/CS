using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gTest : IndicatorObject 
    {
		public _gTest(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            atrlen = 15;
        }

        private VariableSeries<Double> m_spread;
        private _gSpreadFC m_spreadFC;

        private _gTrueRange m_myTR;
        private _gATR m_myATR;

        private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;     //  built in TrueRangeCustom
        private IPlotObject plot4;     // my true range
        private IPlotObject plot5;     // my average true range

        public ISeries<Double> instSprd { get; set; }
        [Input]
        public double leg1 { get; set; }

        [Input]
        public double leg2 { get; set; }

        [Input]
        public double leg3 { get; set; }

        [Input]
        public int atrlen { get; set; }

        void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1,
            ISeries<Double> inst2, ISeries<Double> inst3)
        {
            sprdFunc.inst1 = inst1;
            sprdFunc.inst2 = inst2;
            sprdFunc.inst3 = inst3;
            sprdFunc.leg1 = this.leg1;
            sprdFunc.leg2 = this.leg2;
            sprdFunc.leg3 = this.leg3;
        }		

        protected override void Create() 
        {
            m_spread = new VariableSeries<double>(this);
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
            plot2 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Yellow));
            plot3 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Aqua));
            plot4 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Yellow));
            plot5 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Aqua));
            m_spreadFC = new _gSpreadFC(this);
            m_myTR = new _gTrueRange(this);

            m_myATR = new _gATR(this);
		}
		protected override void StartCalc() 
        {
            assign(m_spreadFC, Bars.Open, BarsOfData(2).Open,
                (leg3 == 0) ? Bars.Open : BarsOfData(3).Open);

            instSprd = m_spreadFC;

            m_myTR.leg1 = leg1;
            m_myTR.leg2 = leg2;
            m_myTR.leg3 = leg3;

            m_myATR.leg1 = leg1;
            m_myATR.leg2 = leg2;
            m_myATR.leg3 = leg3;
            m_myATR.atrlen = atrlen;
		}
		protected override void CalcBar()
        {
            m_spread.Value = m_spreadFC[0];
            //plot1.Set(this.TrueRangeSeries(0).Value);
            //plot2.Set(this.AverageTrueRange(15));

            plot3.Set(this.instSprd.TrueRangeCustom(m_spreadFC, m_spreadFC, 0));


            plot4.Set(m_myTR[0]);
            plot5.Set(m_myATR[0]);
		}
	}
}