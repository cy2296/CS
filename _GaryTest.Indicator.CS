using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _GaryTest : IndicatorObject 
    {
        private AverageFC m_averagefc1;
        private AverageFC m_averagefc2;
	//	private AverageFC m_averagefc3;

        private VariableSeries<Double> m_avgBar1;
		private VariableSeries<Double> m_avgBar2;
	//	private VariableSeries<Double> m_avgBar3;
        //private VariableSeries<Double> m_avgBar12;

        public _GaryTest(object _ctx) : base(_ctx) 
        {
           // ratioBar1 = 0.5;
            length = 9;
        }
		//private IPlotObject plot1;
        private IPlotObject plot2;
        private IPlotObject plot3;
		//private IPlotObject plot4;
		protected override void Create() 
        {
           // m_avgBar12 = new VariableSeries<Double>(this);
            m_avgBar1 = new VariableSeries<Double>(this);
            m_avgBar2 = new VariableSeries<Double>(this);
//			m_avgBar3 = new VariableSeries<Double>(this);

            m_averagefc1 = new AverageFC(this);
            m_averagefc2 = new AverageFC(this);
			//m_averagefc3 = new AverageFC(this);  
			// create variable objects, function objects, plot objects etc.
			//plot1 = AddPlot(new PlotAttributes("Avg", 0, Color.Yellow, Color.Empty, 
             //               0, 0, true));
            plot2 = AddPlot(new PlotAttributes("Ma1", 0, Color.Red, Color.Empty,
                0, 0, true));
            plot3 = AddPlot(new PlotAttributes("Ma2", 0, Color.Green, Color.Empty,
                0, 0, true));
			//plot4 = AddPlot(new PlotAttributes("Ma3", 0, Color.Aqua, Color.Empty,
			//	0, 0, true));
		}

        private ISeries<double> price1 { get; set; }
        private ISeries<double> price2 { get; set; }
		private ISeries<double> price3 { get; set; }
        [Input]
        public double ratioBar1 { get; set; }

        [Input]
        public int length { get; set; }


		protected override void StartCalc() 
        {
			// assign inputs 
            price1 = Bars.Close;
            price2 = BarsOfData(2).Close;
			//price3 = BarsOfData(2).Close;

            m_averagefc1.price = price1;
            m_averagefc2.price = price2;
			//m_averagefc3.price = price3;

            m_averagefc1.length = length;
            m_averagefc2.length = length;
			//m_averagefc3.length = length;
		}
		protected override void CalcBar()
        {
			// indicator logic 
            m_avgBar1.Value = m_averagefc1[0];
			m_avgBar2.Value = m_averagefc2[0];
			//m_avgBar3.Value = m_averagefc3[0];

		//	m_avgBar12.Value = ratioBar1 * m_averagefc1[0]
          //      + (1 - ratioBar1) * m_averagefc2[0];
          //  plot1.Set(0, m_avgBar12.Value);
            plot2.Set(0, m_avgBar1.Value);
            plot3.Set(0, m_avgBar2.Value);
			//plot4.Set(0, m_avgBar3.Value);
		}
	}
}