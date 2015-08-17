using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;


namespace PowerLanguage.Indicator{
	public class _gSpreadRsiBB : IndicatorObject 
    {
        private Function._gSpreadRSI m_rsi1;

        private AverageFC m_averagefc1;   // for BBand
        //private XAverage m_averagefc1;
        private VariableSeries<Double> m_spread_rsi;
        private VariableSeries<Double> m_lowerband;
        private VariableSeries<Double> m_upperband;

        public _gSpreadRsiBB(object _ctx)
            : base(_ctx)
        {
            spread_leg1 = 1;
            spread_leg2 = -0.333;
            spread_leg3 = -0.5;
            numdevsdn = 1.95;
            numdevsup = 1.95;
            len_BBand = 15;
            len_RSI = 10;        
        }
        private IPlotObject plot1;          // Plot RSI 
        private IPlotObject plot2;          // uppper bband    
        private IPlotObject plot3;          // lower bband    
       // private IPlotObject plot4;
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
        public double numdevsup { get; set; }

        [Input]
        public double numdevsdn { get; set; }

		protected override void Create() 
        {
            m_rsi1 = new Function._gSpreadRSI(this);

            m_spread_rsi = new VariableSeries<Double>(this);
            //m_averagefc1 = new XAverage(this);
            m_averagefc1 = new AverageFC(this);
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);

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

            m_averagefc1.price = m_spread_rsi;
            m_averagefc1.length = len_BBand;
		}
		protected override void CalcBar()
        {
            m_spread_rsi.Value = m_rsi1[0];

            var m_avg = m_averagefc1[0];
            var m_sdev = m_spread_rsi.StandardDeviationCustom(len_BBand, 1);
            m_upperband.Value = (m_avg + (numdevsup * m_sdev));
            m_lowerband.Value = (m_avg - (numdevsdn * m_sdev));

            plot1.Set(0, m_spread_rsi.Value);
            plot2.Set(0, m_upperband.Value);
            plot3.Set(0, m_lowerband.Value);



		}
	}
}