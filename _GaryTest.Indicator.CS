using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class _GaryTest : IndicatorObject 
    {
        private VariableSeries<Double> m_dChangeES;
        private VariableSeries<Double> m_dChangeVX;
        private VariableSeries<Double> m_dChangeVxTheo;

        public _GaryTest(object _ctx) : base(_ctx) 
        {
            length = 100;
            vxWeight = 0.6;
            upRatio = 0.5;
            spreadLookBack = 10;
        }

        private IPlotObject dChageES;
        private IPlotObject dChangeVX;
        private IPlotObject dChangeVxTheo;

        private ISeries<double> esFv { get; set; }
        private ISeries<double> vxFv { get; set; }

        private _gTurnMetric_Delta m_delta_indic;

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
            //plot0 = AddPlot(new PlotAttributes("Total", 0, Color.Yellow, Color.Empty, 3, 0, true));
            m_dChangeES = new VariableSeries<Double>(this);
            m_dChangeVX = new VariableSeries<Double>(this);
            m_dChangeVxTheo = new VariableSeries<Double>(this);

            dChageES = AddPlot(new PlotAttributes("dollarES", EPlotShapes.Line, Color.Red, Color.Empty, 3, 0, true));
            dChangeVX = AddPlot(new PlotAttributes("dollarVX", EPlotShapes.Line, Color.Green, Color.Empty, 3, 0, true));
            dChangeVxTheo = AddPlot(new PlotAttributes("dollarVxTheo", EPlotShapes.Line, Color.Cyan, Color.Empty, 3, 0, true));

            m_delta_indic = (_gTurnMetric_Delta)AddIndicator("_gTurnMetric_Delta");
            m_delta_indic.length = length;
            m_delta_indic.vxWeight = vxWeight;
            m_delta_indic.upRatio = upRatio;
        }
        protected override void StartCalc()
        {
            esFv = BarsOfData(3).Close;
            vxFv = BarsOfData(4).Close;
        }
        protected override void CalcBar()
        {
            m_dChangeES.Value = 50 * (esFv[0] - esFv[spreadLookBack]);
            m_dChangeVX.Value = 1000 * (vxFv[0] - vxFv[spreadLookBack]);
            m_dChangeVxTheo.Value = -1 * m_delta_indic.Plots[6].Values[spreadLookBack] * m_dChangeES.Value;
           
            dChageES.Set(0, -1 * m_dChangeES.Value);
            dChangeVX.Set(0, m_dChangeVX.Value);
            dChangeVxTheo.Set(0, m_dChangeVxTheo.Value);
        }
	}
}