using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Fv : IndicatorObject 
    {
		private _gDeltaFC m_deltaFC;
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
			m_deltaFC = new _gDeltaFC(this);

			m_dChangeES = new VariableSeries<Double>(this);
			m_dChangeVX = new VariableSeries<Double>(this);
			m_dChangeVxTheo = new VariableSeries<Double>(this);

			dChageES = AddPlot(new PlotAttributes("dollarES", EPlotShapes.Line, Color.Red, Color.Empty, 3, 0, true));
			dChangeVX = AddPlot(new PlotAttributes("dollarVX", EPlotShapes.Line, Color.Green, Color.Empty, 3, 0, true));
			dChangeVxTheo = AddPlot(new PlotAttributes("dollarVxTheo", EPlotShapes.Line, Color.Cyan, Color.Empty, 3, 0, true));

		}
		protected override void StartCalc()
        {
			m_deltaFC.esDelta = Bars.Close;
			m_deltaFC.esEvent = Bars.TrueVolume();
			m_deltaFC.vxDelta = BarsOfData(2).Close;
			m_deltaFC.vxEvent = BarsOfData(2).TrueVolume();
			m_deltaFC.length = length;
			m_deltaFC.vxWeight = vxWeight;
			m_deltaFC.upRatio = upRatio;

			esFv = BarsOfData(3).Close;
			vxFv = BarsOfData(4).Close;
		}
		protected override void CalcBar()
        {
			m_dChangeES.Value = 50 * (esFv[0] - esFv[spreadLookBack]);
			m_dChangeVX.Value = 1000 * (vxFv[0] - vxFv[spreadLookBack]);
			m_dChangeVxTheo.Value = -1 * m_deltaFC[spreadLookBack] * m_dChangeES.Value;
	
			dChageES.Set(0, -1 * m_dChangeES.Value);
			dChangeVX.Set(0, m_dChangeVX.Value);
			dChangeVxTheo.Set(0, m_dChangeVxTheo.Value);
		}
	}
}