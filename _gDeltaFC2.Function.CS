using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gDeltaFC2 : FunctionSeries<System.Double>
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

            private _gConSumFC m_vxUpCountFC;
            private _gConSumFC m_vxDnCountFC;
            private _gConSumFC m_esUpCountFC;
            private VariableSeries<Double> m_vxWeight;
            private VariableSeries<Double> m_upWeight;

			public _gDeltaFC2(CStudyControl _master) : base(_master) { }
			public _gDeltaFC2(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> esDelta { get; set; }
            public ISeries<Double> esEvent { get; set; }
            public ISeries<Double> vxDelta { get; set; }
            public ISeries<Double> vxEvent { get; set; }

            public int lenDelta { get; set; }
            public int lenLookBack { get; set; } 
            // lenLookBack might equal to lenDelta to have better accuracy, check later

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

                m_vxUpCountFC = new _gConSumFC(this);
                m_vxDnCountFC = new _gConSumFC(this);
                m_esUpCountFC = new _gConSumFC(this);
                m_vxWeight = new VariableSeries<Double>(this);
                m_upWeight = new VariableSeries<Double>(this);
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
                m_xAvgUpEsFC.length = lenDelta;
                m_xAvgDnEsFC.length = lenDelta;
                m_xAvgUpVxFC.length = lenDelta;
                m_xAvgDnVxFC.length = lenDelta;
                m_xAvgUpEsFC.eventId = 2.0;
                m_xAvgDnEsFC.eventId = 1.0;
                m_xAvgUpVxFC.eventId = 2.0;
                m_xAvgDnVxFC.eventId = 1.0;

                m_vxUpCountFC.price = vxEvent;
                m_vxUpCountFC.volume = vxEvent;
                m_vxUpCountFC.eventId = 2.0;
                m_vxUpCountFC.length = lenLookBack;

                m_vxDnCountFC.price = vxEvent;
                m_vxDnCountFC.volume = vxEvent;
                m_vxDnCountFC.eventId = 1.0;
                m_vxDnCountFC.length = lenLookBack;

                m_esUpCountFC.price = esEvent;
                m_esUpCountFC.volume = esEvent;
                m_esUpCountFC.eventId = 2.0;
                m_esUpCountFC.length = lenLookBack;
			}

			protected override System.Double CalcBar()
			{
                m_avgUpES.Value = (0 == m_xAvgUpEsFC[0]) ? 0 : 1 / m_xAvgUpEsFC[0];
                m_avgDnES.Value = (0 == m_xAvgDnEsFC[0]) ? 0 : 1 / m_xAvgDnEsFC[0];
                m_avgUpVX.Value = m_xAvgUpVxFC[0];
                m_avgDnVX.Value = m_xAvgDnVxFC[0];

                var vxCount = ((int)m_vxUpCountFC[0] / 2) + m_vxDnCountFC[0];
                m_vxWeight.Value = 1 - (vxCount / lenLookBack);

                var upCount = ((int)m_vxUpCountFC[0] / 2) + ((int)m_esUpCountFC[0] / 2);
                m_upWeight.Value = 1 - (upCount / lenLookBack);

                m_avgUp.Value = (1 - m_vxWeight.Value) * m_avgUpES.Value + m_vxWeight.Value * m_avgUpVX.Value;
                m_avgDn.Value = (1 - m_vxWeight.Value) * m_avgDnES.Value + m_vxWeight.Value * m_avgDnVX.Value;
                m_avgTot.Value = m_upWeight.Value * m_avgUp.Value + (1 - m_upWeight.Value) * m_avgDn.Value;

                return m_avgTot.Value;
			}
		}
	}
}