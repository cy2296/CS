using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gSpreadRSI : IndicatorObject 
    {
        private Function._gSpreadRSI m_rsi1;
        private Function._gSpreadRsiUpBB m_spreadRsiUpBB;
        private Function._gSpreadRsiDnBB m_spreadRsiDnBB;

		public _gSpreadRSI(object _ctx):base(_ctx)
        {
            spread_leg1 = -1;
            spread_leg2 = 1;
            spread_leg3 = 0;
            numdevsdn = 2;
            numdevsup = 2;
            len_BBand = 20;
            len_RSI = 14;        
        }
        private IPlotObject plot1;          // Plot RSI 
        private IPlotObject plot2;          // uppper bband    
        private IPlotObject plot3;          // lower bband    

        [Input]
        public double spread_leg1 { get; set; }
        [Input]
        public double spread_leg2 { get; set; }
        [Input]
        public double spread_leg3 { get; set; }
        [Input]
        public int len_RSI { get; set; }
        [Input]
        public int len_BBand { get; set; }
        [Input]
        public int numdevsup { get; set; }
        [Input]
        public int numdevsdn { get; set; }

		protected override void Create() 
        {
            m_rsi1 = new Function._gSpreadRSI(this);
            m_spreadRsiUpBB = new Function._gSpreadRsiUpBB(this);
            m_spreadRsiDnBB = new Function._gSpreadRsiDnBB(this);

            // RSI
            plot1 = AddPlot(new PlotAttributes("RSI", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            plot2 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            plot3 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));

        }
		protected override void StartCalc() 
        {
            m_rsi1.length = len_RSI;
            m_rsi1.price1 = Bars.Close;
            m_rsi1.price2 = BarsOfData(2).Close;
            if (spread_leg3 != 0)
                m_rsi1.price3 = BarsOfData(3).Close;
            else
                m_rsi1.price3 = Bars.Close;
            m_rsi1.leg1 = spread_leg1;
            m_rsi1.leg2 = spread_leg2;
            m_rsi1.leg3 = spread_leg3;

            m_spreadRsiUpBB.len_rsi = len_RSI;
            m_spreadRsiUpBB.price1 = Bars.Close;
            m_spreadRsiUpBB.price2 = BarsOfData(2).Close;
            if (spread_leg3 != 0)
                m_spreadRsiUpBB.price3 = BarsOfData(3).Close;
            else
                m_spreadRsiUpBB.price3 = Bars.Close;
            m_spreadRsiUpBB.leg1 = spread_leg1;
            m_spreadRsiUpBB.leg2 = spread_leg2;
            m_spreadRsiUpBB.leg3 = spread_leg3;
            m_spreadRsiUpBB.numdevsup = numdevsup;
            m_spreadRsiUpBB.numdevsdn = numdevsdn;
            m_spreadRsiUpBB.len_bb = len_BBand;

            m_spreadRsiDnBB.len_rsi = len_RSI;
            m_spreadRsiDnBB.price1 = Bars.Close;
            m_spreadRsiDnBB.price2 = BarsOfData(2).Close;
            if (spread_leg3 != 0)
                m_spreadRsiDnBB.price3 = BarsOfData(3).Close;
            else
                m_spreadRsiDnBB.price3 = Bars.Close;
            m_spreadRsiDnBB.leg1 = spread_leg1;
            m_spreadRsiDnBB.leg2 = spread_leg2;
            m_spreadRsiDnBB.leg3 = spread_leg3;
            m_spreadRsiDnBB.numdevsup = numdevsup;
            m_spreadRsiDnBB.numdevsdn = numdevsdn;
            m_spreadRsiDnBB.len_bb = len_BBand;
		}
		protected override void CalcBar()
        {
            plot1.Set(0, m_rsi1[0]);
            plot2.Set(0, m_spreadRsiUpBB[0]);
            plot3.Set(0, m_spreadRsiDnBB[0]);
		}
	}
}