using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Fv : IndicatorObject 
    {
        private _gConXAvgFC m_xAvgUpEsFC;
        private _gConXAvgFC m_xAvgDnEsFC;
        private _gConXAvgFC m_xAvgUpVxFC;
        private _gConXAvgFC m_xAvgDnVxFC;
		/// Variable series for computing deltas
        private VariableSeries<Double> m_avgUpES;
        private VariableSeries<Double> m_avgDnES;
        private VariableSeries<Double> m_avgUpVX;
        private VariableSeries<Double> m_avgDnVX;
        private VariableSeries<Double> m_avgUp;
        private VariableSeries<Double> m_avgDn;
        private VariableSeries<Double> m_avgTot;
		/// Variable series for plotting dollar change value 
		private VariableSeries<Double> m_dChangeES;
		private VariableSeries<Double> m_dChangeVX;
		private VariableSeries<Double> m_dChangeVxTheo;

		public _gTurnMetric_Fv(object _ctx)
			: base(_ctx)
		{
			length = 10;
			vxWeight = 0.6;
			upRatio = 0.5;
			spreadLookBack = 100;
		}
		private IPlotObject dChageES;
		private IPlotObject dChangeVX;
		private IPlotObject dChangeVxTheo;

		private ISeries<double> esDelta { get; set; }
		private ISeries<double> esEvent { get; set; }
		private ISeries<double> vxDelta { get; set; }
		private ISeries<double> vxEvent { get; set; }

		private ISeries<double> esFv { get; set; }
		private ISeries<double> vxFv { get; set; }

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

			m_dChangeES = new VariableSeries<Double>(this);
			m_dChangeVX = new VariableSeries<Double>(this);
			m_dChangeVxTheo = new VariableSeries<Double>(this);

			dChageES = AddPlot(new PlotAttributes("dollarES", EPlotShapes.Line, Color.Red, Color.Empty, 3, 0, true));
			dChangeVX = AddPlot(new PlotAttributes("dollarVX", EPlotShapes.Line, Color.Green, Color.Empty, 3, 0, true));
			dChangeVxTheo = AddPlot(new PlotAttributes("dollarVxTheo", EPlotShapes.Line, Color.Cyan, Color.Empty, 3, 0, true));

		}
		protected override void StartCalc()
        {
			esDelta = Bars.Close;
			esEvent = Bars.TrueVolume();
			
			vxDelta = BarsOfData(2).Close;
			vxEvent = BarsOfData(2).TrueVolume();

			esFv = BarsOfData(3).Close;
			vxFv = BarsOfData(4).Close;

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

			m_dChangeES.Value = 50 * (esFv[0] - esFv[spreadLookBack]);
			m_dChangeVX.Value = 1000 * (vxFv[0] - vxFv[spreadLookBack]);
			m_dChangeVxTheo.Value = -1 * m_avgTot[spreadLookBack] * m_dChangeES.Value;
	
			dChageES.Set(0, m_dChangeES.Value);
			dChangeVX.Set(0, -1 * m_dChangeVX.Value);
			dChangeVxTheo.Set(0, -1 * m_dChangeVxTheo.Value);
		}
	}
}