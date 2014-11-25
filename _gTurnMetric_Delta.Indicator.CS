using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
	public class _gTurnMetric_Delta : IndicatorObject 
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

        public _gTurnMetric_Delta(object _ctx):base(_ctx)
		{
			length = 10;
			vxWeight = 0.6;
			upRatio = 0.5;
		}
		
		private IPlotObject upES;
		private IPlotObject dnES;
		private IPlotObject upVX;
		private IPlotObject dnVX;
		private IPlotObject upDel;
		private IPlotObject dnDel;
		private IPlotObject totDel;

		private ISeries<double> esDelta { get; set; }
		private ISeries<double> vxDelta { get; set; }

		[Input]
		public int length { get; set; }

		[Input]
		public double vxWeight { get; set; }

		[Input]
		public double upRatio { get; set; }

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
			// create variable objects, function objects, plot objects etc.
			upES   = AddPlot(new PlotAttributes("upES", 0, Color.Aqua));
			dnES   = AddPlot(new PlotAttributes("dnES", 0, Color.Red));
			upVX   = AddPlot(new PlotAttributes("upVX", 0, Color.Red));
			dnVX   = AddPlot(new PlotAttributes("dnVX", 0, Color.Red));
			upDel  = AddPlot(new PlotAttributes("Up", 0, Color.Red));
			dnDel  = AddPlot(new PlotAttributes("Down", 0, Color.Red));
			totDel = AddPlot(new PlotAttributes("Total", 0, Color.Red));
		}
		protected override void StartCalc()
        {
			esDelta = Bars.Close;
			vxDelta = BarsOfData(2).Close;

            m_xAvgUpEsFC.price = esDelta;
            m_xAvgDnEsFC.price = esDelta;
            m_xAvgUpVxFC.price = vxDelta;
            m_xAvgDnVxFC.price = vxDelta;

            m_xAvgUpEsFC.length = length;
            m_xAvgDnEsFC.length = length;
            m_xAvgUpVxFC.length = length;
            m_xAvgDnVxFC.length = length;

            m_xAvgUpEsFC.eventId = 2;
            m_xAvgDnEsFC.eventId = 1;
            m_xAvgUpVxFC.eventId = 2;
            m_xAvgDnVxFC.eventId = 1;

		}
		protected override void CalcBar()
        { 
            m_avgUpES.Value = m_xAvgUpEsFC[0];
            m_avgDnES.Value = m_xAvgDnEsFC[0];
            m_avgUpVX.Value = m_xAvgUpVxFC[0];
            m_avgDnVX.Value = m_xAvgDnVxFC[0];

            m_avgUp.Value = (1 - vxWeight) * m_avgUpES.Value + vxWeight * m_avgUpVX.Value;
            m_avgDn.Value = (1 - vxWeight) * m_avgDnES.Value + vxWeight * m_avgDnVX.Value;
            m_avgTot.Value = upRatio * m_avgUp.Value + (1 - upRatio) * m_avgDn.Value;

            upES.Set(0, m_avgUpES.Value);
            dnES.Set(0, m_avgDnES.Value);
            upVX.Set(0, m_avgUpVX.Value);
            dnVX.Set(0, m_avgDnVX.Value);
            upDel.Set(0, m_avgUp.Value);
            dnDel.Set(0, m_avgDn.Value);
            totDel.Set(0, m_avgTot.Value);
		}
	}
}