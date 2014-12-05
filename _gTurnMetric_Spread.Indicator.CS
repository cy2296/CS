using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Spread : IndicatorObject 
    {
		/// 
		private _gDeltaFC m_deltaFC;
		private VariableSeries<Double> m_dollarSpreadTheo;
		private VariableSeries<Double> m_dollarSpreadReal;

		private _gBollinger_Bands_AddOn m_lowBandFC;
		private _gBollinger_Bands_AddOn m_midBandFC;
		private _gBollinger_Bands_AddOn m_upBandFC;
		private VariableSeries<Double> m_lowBand;
		private VariableSeries<Double> m_midBand;
		private VariableSeries<Double> m_upBand;

        public _gTurnMetric_Spread(object _ctx):base(_ctx)
		{
			deltaLength = 100;
			vxWeight = 0.6;
			upRatio = 0.5;
			spreadLookBack = 5;
		}
	
		private IPlotObject spreadReal;
		private IPlotObject spreadTheo;

		private IPlotObject Plot1;
		private IPlotObject Plot2;
		private IPlotObject Plot3;

		private ISeries<double> fvES { get; set; }
		private ISeries<double> fvVX { get; set; }

		/// inputs for delta 
		[Input]
		public int deltaLength { get; set; }
		[Input]
		public double vxWeight { get; set; }
		[Input]
		public double upRatio { get; set; }
		
		[Input]
		public int spreadLookBack { get; set; }
		/// Inputs for BBand
		[Input]
		public int bbLength { get; set; }
		[Input]
		public int stdevUp { get; set; }
		[Input]
		public int stdevDn { get; set; }


		protected override void Create()
        {
			m_deltaFC = new _gDeltaFC(this);

			m_dollarSpreadReal = new VariableSeries<Double>(this);
			m_dollarSpreadTheo = new VariableSeries<Double>(this);
			spreadReal = AddPlot(new PlotAttributes("SpreadReal", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, true));
			spreadTheo = AddPlot(new PlotAttributes("SpreadTheo", EPlotShapes.Line, Color.Red, Color.Empty, 0, 0, true));

			m_lowBandFC = new _gBollinger_Bands_AddOn(this, bbLength, stdevUp, stdevDn, 0);
			m_midBandFC = new _gBollinger_Bands_AddOn(this, bbLength, stdevUp, stdevDn, 1);
			m_upBandFC = new _gBollinger_Bands_AddOn(this, bbLength, stdevUp, stdevDn, 2);
			m_lowBand = new VariableSeries<Double>(this);
			m_midBand = new VariableSeries<Double>(this);
			m_upBand = new VariableSeries<Double>(this);

			Plot1 = AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow, Color.Empty, 0, 0, true));
			Plot2 = AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue, Color.Empty, 0, 0, true));
			Plot3 = AddPlot(new PlotAttributes("MidLine", 0, Color.Gray, Color.Empty, 0, 0, true));
		
		}
		protected override void StartCalc()
        {
			//m_midBandFC.bollingerprice = m_dollarSpreadTheo;

			m_deltaFC.esDelta = Bars.Close;
			m_deltaFC.esEvent = Bars.TrueVolume();
			m_deltaFC.vxDelta = BarsOfData(2).Close;
			m_deltaFC.vxEvent = BarsOfData(2).TrueVolume();
			m_deltaFC.length = deltaLength;
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

			m_lowBand.Value = m_lowBandFC[0];
			m_midBand.Value = m_midBandFC[0];
			m_upBand.Value = m_upBandFC[0];
 
			spreadTheo.Set(0, m_dollarSpreadTheo.Value);
			Plot1.Set(0, m_lowBand.Value);
			Plot2.Set(0, m_midBand.Value);
			Plot3.Set(0, m_upBand.Value);
			//spreadReal.Set(0, m_dollarSpreadReal.Value);
		}
	}
}