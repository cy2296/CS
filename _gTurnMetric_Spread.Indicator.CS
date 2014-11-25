using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Spread : IndicatorObject 
    {
        private _gConXAvgFC m_xAvgUpEsFC;
        private _gConXAvgFC m_xAvgDnEsFC;
        private _gConXAvgFC m_xAvgUpVxFC;
        private _gConXAvgFC m_xAvgDnVxFC;

        private VariableSeries<Double> m_avgUpES;
        private VariableSeries<Double> m_avgDnES;
        private VariableSeries<Double> m_avgUpVX;
        private VariableSeries<Double> m_avgDnVX;
        private VariableSeries<Double> m_avgUp;
        private VariableSeries<Double> m_avgDn;
        private VariableSeries<Double> m_avgTot;

		private VariableSeries<Double> m_dollarSpread;

        public _gTurnMetric_Spread(object _ctx):base(_ctx)
		{
			length = 10;
			vxWeight = 0.6;
			upRatio = 0.5;
			spreadLookBack = 100;
		}
		
		//private IPlotObject upES;
		//private IPlotObject dnES;
		//private IPlotObject upVX;
		//private IPlotObject dnVX;
		//private IPlotObject upDel;
		//private IPlotObject dnDel;
		//private IPlotObject totDel;

		private IPlotObject spread;

		private ISeries<double> esDelta { get; set; }
		private ISeries<double> esEvent { get; set; }
		private ISeries<double> vxDelta { get; set; }
		private ISeries<double> vxEvent { get; set; }

		private ISeries<double> fvES { get; set; }
		private ISeries<double> fvVX { get; set; }


		[Input]
		public int length { get; set; }

		[Input]
		public double vxWeight { get; set; }

		[Input]
		public double upRatio { get; set; }

		[Input]
		public int spreadLookBack { get; set; }

		protected override void Create()
        {
            m_xAvgUpEsFC = new _gConXAvgFC(this);
            m_xAvgDnEsFC = new _gConXAvgFC(this);
            m_xAvgUpVxFC = new _gConXAvgFC(this);
            m_xAvgDnVxFC = new _gConXAvgFC(this);

            m_avgUpES = new VariableSeries<Double>(this);
            m_avgDnES = new VariableSeries<Double>(this);
            m_avgUpVX = new VariableSeries<Double>(this);
            m_avgDnVX = new VariableSeries<Double>(this);
            m_avgUp   = new VariableSeries<Double>(this);
            m_avgDn   = new VariableSeries<Double>(this);
            m_avgTot  = new VariableSeries<Double>(this);

			m_dollarSpread = new VariableSeries<Double>(this);

			// create variable objects, function objects, plot objects etc.
			//upES   = AddPlot(new PlotAttributes("upES", 0, Color.MediumTurquoise, Color.Empty, 1, 0, true));
			//dnES = AddPlot(new PlotAttributes("dnES", 0, Color.LightPink, Color.Empty, 1, 0, true));
			//upVX = AddPlot(new PlotAttributes("upVX", 0, Color.LightCyan, Color.Empty, 1, 0, true));
			//dnVX = AddPlot(new PlotAttributes("dnVX", 0, Color.MistyRose, Color.Empty, 1, 0, true));
			//upDel = AddPlot(new PlotAttributes("Up", 0, Color.Aqua, Color.Empty, 2, 0, true));
			//dnDel = AddPlot(new PlotAttributes("Down", 0, Color.Red, Color.Empty, 2, 0, true));
			//totDel = AddPlot(new PlotAttributes("Total", 0, Color.Yellow, Color.Empty, 3, 0, true));
			spread = AddPlot(new PlotAttributes("Spread", EPlotShapes.Histogram, Color.Cyan, Color.Empty, 3, 0, true));
		}
		protected override void StartCalc()
        {
			esDelta = Bars.Close;
			esEvent = Bars.TrueVolume();
			
			vxDelta = BarsOfData(2).Close;
			vxEvent = BarsOfData(2).TrueVolume();

			fvES = BarsOfData(3).Close;
			fvVX = BarsOfData(4).Close;

            m_xAvgUpEsFC.price = esDelta;
            m_xAvgDnEsFC.price = esDelta;
            m_xAvgUpVxFC.price = vxDelta;
            m_xAvgDnVxFC.price = vxDelta;

			m_xAvgUpEsFC.volume = esEvent;
			m_xAvgDnEsFC.volume = esEvent;
			m_xAvgUpVxFC.volume = vxEvent;
			m_xAvgDnVxFC.volume = vxEvent;

            m_xAvgUpEsFC.length = length;
            m_xAvgDnEsFC.length = length;
            m_xAvgUpVxFC.length = length;
            m_xAvgDnVxFC.length = length;

            m_xAvgUpEsFC.eventId = 2.0;
            m_xAvgDnEsFC.eventId = 1.0;
            m_xAvgUpVxFC.eventId = 2.0;
            m_xAvgDnVxFC.eventId = 1.0;

		}
		protected override void CalcBar()
        {
			m_avgUpES.Value = (0 == m_xAvgUpEsFC[0]) ? 0 : 1 / m_xAvgUpEsFC[0];
            m_avgDnES.Value = (0 == m_xAvgDnEsFC[0]) ? 0 : 1 / m_xAvgDnEsFC[0];
            m_avgUpVX.Value = m_xAvgUpVxFC[0];
            m_avgDnVX.Value = m_xAvgDnVxFC[0];

            m_avgUp.Value = (1 - vxWeight) * m_avgUpES.Value + vxWeight * m_avgUpVX.Value;
            m_avgDn.Value = (1 - vxWeight) * m_avgDnES.Value + vxWeight * m_avgDnVX.Value;
            m_avgTot.Value = upRatio * m_avgUp.Value + (1 - upRatio) * m_avgDn.Value;

			var esChange = fvES[0] - fvES[spreadLookBack];
			var vxChange = fvVX[0] - fvVX[spreadLookBack];
			m_dollarSpread.Value = esChange * 50 + vxChange * 1000;

			//upES.Set(0, m_avgUpES.Value);
			//dnES.Set(0, m_avgDnES.Value);
			//upVX.Set(0, m_avgUpVX.Value);
			//dnVX.Set(0, m_avgDnVX.Value);
			//upDel.Set(0, m_avgUp.Value);
			//dnDel.Set(0, m_avgDn.Value);
			//totDel.Set(0, m_avgTot.Value);

			spread.Set(0, m_dollarSpread.Value);
		}
	}
}