using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSpreadATR : IndicatorObject 
    {
        private _gSpreadFC m_spreadOpenFC;
        private _gSpreadFC m_spreadClosFC;

        private _gATR m_atr;

        private IPlotObject plot0;      // close    
        private IPlotObject plot1;      // close[1] + num * atr
        private IPlotObject plot2;      // close[1] - num * atr
        //private IPlotObject plot3;      
        //private IPlotObject plot4;


		public _gSpreadATR(object _ctx):base(_ctx)
        {
            leg1 = 1.0;
            leg2 = -0.333;
            leg3 = -0.5;
            numAtr = 2;
            atrLen = 15;
        }
        [Input]
        public double leg1 { get; set; }
        [Input]
        public double leg2 { get; set; }
        [Input]
        public double leg3 { get; set; }
        [Input]
        public int numAtr { get; set; }
        [Input]
        public int atrLen { get; set; }


        void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1,
            ISeries<Double> inst2, ISeries<Double> inst3)
        {
            sprdFunc.inst1 = inst1;
            sprdFunc.inst2 = inst2;
            sprdFunc.inst3 = inst3;
            sprdFunc.leg1 = leg1;
            sprdFunc.leg2 = leg2;
            sprdFunc.leg3 = leg3;
        }		

		protected override void Create() 
        {
            m_spreadOpenFC = new _gSpreadFC(this);
            m_spreadClosFC = new _gSpreadFC(this);
            m_atr = new _gATR(this);
            plot0 = AddPlot(new PlotAttributes("close", EPlotShapes.Line, Color.White));
            plot1 = AddPlot(new PlotAttributes("c1_atr_up", EPlotShapes.Line, Color.Yellow));
            plot2 = AddPlot(new PlotAttributes("c1_atr_dn", EPlotShapes.Line, Color.Blue));
		}
		protected override void StartCalc() 
        {
            assign(m_spreadOpenFC, Bars.Open, BarsOfData(2).Open, 
                (leg3 == 0) ? Bars.Open : BarsOfData(3).Open);
            assign(m_spreadClosFC, Bars.Close, BarsOfData(2).Close, 
                (leg3 == 0) ? Bars.Close : BarsOfData(3).Close);

            m_atr.leg1 = leg1;
            m_atr.leg2 = leg2;
            m_atr.leg3 = leg3;
            m_atr.atrlen = atrLen;
        }
		protected override void CalcBar()
        {
            plot0.Set(m_spreadClosFC[0]);
            plot1.Set((m_spreadOpenFC[1] + m_spreadClosFC[1]) * 0.5 + numAtr * m_atr[1]);
            plot2.Set((m_spreadOpenFC[1] + m_spreadClosFC[1]) * 0.5 - numAtr * m_atr[1]);
		}
	}
}