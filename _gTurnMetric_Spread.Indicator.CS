using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Spread : IndicatorObject 
    {
		private _gDeltaFC m_deltaFC;

		private VariableSeries<Double> m_dollarSpreadTheo;
		private VariableSeries<Double> m_dollarSpreadReal;

        public _gTurnMetric_Spread(object _ctx):base(_ctx)
		{
			length = 10;
			vxWeight = 0.6;
			upRatio = 0.5;
			spreadLookBack = 100;
		}
	
		private IPlotObject spreadReal;
		private IPlotObject spreadTheo;

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
			m_deltaFC = new _gDeltaFC(this);

			m_dollarSpreadReal = new VariableSeries<Double>(this);
			m_dollarSpreadTheo = new VariableSeries<Double>(this);

			spreadReal = AddPlot(new PlotAttributes("SpreadReal", EPlotShapes.Histogram, Color.Cyan, Color.Empty, 3, 0, true));
			spreadTheo = AddPlot(new PlotAttributes("SpreadTheo", EPlotShapes.Line, Color.Red, Color.Empty, 3, 0, true));
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

			fvES = BarsOfData(3).Close;
			fvVX = BarsOfData(4).Close;
		}
		protected override void CalcBar()
        {
			var esDollarChange = 50 * (fvES[0] - fvES[spreadLookBack]);
			var vxDollarChangeReal = 1000 * (fvVX[0] - fvVX[spreadLookBack]);
			var vxDollarChangeTheo = -1 * m_deltaFC[spreadLookBack] * esDollarChange;

			m_dollarSpreadReal.Value = esDollarChange + vxDollarChangeReal;
			m_dollarSpreadTheo.Value = esDollarChange + vxDollarChangeTheo;

			spreadTheo.Set(0, m_dollarSpreadTheo.Value);
			spreadReal.Set(0, m_dollarSpreadReal.Value);
		}
	}
}