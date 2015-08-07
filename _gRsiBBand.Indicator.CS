using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _gRsiBBand : IndicatorObject 
    {
        private Function.RSI m_rsi1;        
        //private AverageFC m_averagefc1;   // for BBand
        private XAverage m_averagefc1;
        private VariableSeries<Double> m_myrsi;
        private VariableSeries<Double> m_lowerband;
        private VariableSeries<Double> m_upperband;

		public _gRsiBBand(object _ctx) : base(_ctx)
        {
            numdevsdn = 2;
            numdevsup = 2;
            len_BBand = 20;
            len_RSI = 14;
        }
		
        private IPlotObject plot1;          // Plot RSI 
        private IPlotObject plot2;          // uppper bband    
        private IPlotObject plot3;          // lower bband    

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
            m_rsi1 = new Function.RSI(this);

            m_myrsi = new VariableSeries<Double>(this);
            m_averagefc1 = new XAverage(this);
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);
            // RSI
            plot1 = AddPlot(new PlotAttributes("RSI", 0, Color.Silver, 
                                           Color.Empty, 0, 0, true));
            // upper bband
            plot2 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            // lower bband
            plot3 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }
		protected override void StartCalc() 
        {
            m_rsi1.price = Bars.Close;
            m_rsi1.length = len_RSI;

            m_averagefc1.Price = m_myrsi;
            m_averagefc1.Length = len_BBand;
        }
		protected override void CalcBar()
        {
            m_myrsi.Value = m_rsi1[0];
            var m_avg = m_averagefc1[0];
            var m_sdev = m_myrsi.StandardDeviationCustom(len_BBand, 1);
            m_upperband.Value = (m_avg + (numdevsup * m_sdev));
            m_lowerband.Value = (m_avg - (numdevsdn * m_sdev));

            plot1.Set(0, m_myrsi.Value);
            plot2.Set(0, m_upperband.Value);
            plot3.Set(0, m_lowerband.Value);
		}
	}
}