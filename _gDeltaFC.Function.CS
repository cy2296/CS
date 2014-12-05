using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gDeltaFC : FunctionSeries<System.Double>
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

			public _gDeltaFC(CStudyControl _master) : base(_master) { }
			public _gDeltaFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

			public ISeries<Double> esDelta { get; set; }
			public ISeries<Double> esEvent { get; set; }
			public ISeries<Double> vxDelta { get; set; }
			public ISeries<Double> vxEvent { get; set; }

			public int length { get; set; }
			public double vxWeight { get; set; }
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
				m_avgUp = new VariableSeries<Double>(this);
				m_avgDn = new VariableSeries<Double>(this);
				m_avgTot = new VariableSeries<Double>(this);
			}

			protected override void StartCalc()
			{
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

			protected override System.Double CalcBar()
			{
				m_avgUpES.Value = (0 == m_xAvgUpEsFC[0]) ? 0 : 1 / m_xAvgUpEsFC[0];
				m_avgDnES.Value = (0 == m_xAvgDnEsFC[0]) ? 0 : 1 / m_xAvgDnEsFC[0];
				m_avgUpVX.Value = m_xAvgUpVxFC[0];
				m_avgDnVX.Value = m_xAvgDnVxFC[0];

				m_avgUp.Value = (1 - vxWeight) * m_avgUpES.Value + vxWeight * m_avgUpVX.Value;
				m_avgDn.Value = (1 - vxWeight) * m_avgDnES.Value + vxWeight * m_avgDnVX.Value;
				m_avgTot.Value = upRatio * m_avgUp.Value + (1 - upRatio) * m_avgDn.Value;

				return m_avgTot.Value;
			}
		}
	}
}