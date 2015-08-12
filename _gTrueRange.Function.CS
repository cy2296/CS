using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gTrueRange : FunctionSeries<System.Double>
		{
            private _gSpreadFC m_spreadOpenFC;
            private _gSpreadFC m_spreadClosFC;

			public _gTrueRange(CStudyControl _master) : base(_master) { }
			public _gTrueRange(CStudyControl _master, int _ds) : base(_master, _ds) { }

            //public int bb { get; set; }

            //public ISeries<Double> inst1 { get; set; }
            //public ISeries<Double> inst2 { get; set; }
            //public ISeries<Double> inst3 { get; set; }

            public double leg1 { get; set; }
            public double leg2 { get; set; }
            public double leg3 { get; set; }

            void assign(_gSpreadFC sprdFunc, ISeries<Double> inst1,
                ISeries<Double> inst2, ISeries<Double> inst3)
            {
                sprdFunc.inst1 = inst1;
                sprdFunc.inst2 = inst2;
                sprdFunc.inst3 = inst3;
                sprdFunc.leg1 = this.leg1;
                sprdFunc.leg2 = this.leg2;
                sprdFunc.leg3 = this.leg3;
            }	

			protected override void Create()
			{
                m_spreadOpenFC = new _gSpreadFC(this);
                m_spreadClosFC = new _gSpreadFC(this);
			}

			protected override void StartCalc()
			{
                assign(m_spreadOpenFC, Bars.Open, BarsOfData(2).Open,
                    (leg3 == 0) ? Bars.Open : BarsOfData(3).Open);
                assign(m_spreadClosFC, Bars.Close, BarsOfData(2).Close,
                    (leg3 == 0) ? Bars.Close : BarsOfData(3).Close);
			}

			protected override System.Double CalcBar()
			{
                var a = Math.Abs(m_spreadOpenFC[0] - m_spreadClosFC[1]);
                var b = Math.Abs(m_spreadClosFC[0] - m_spreadClosFC[1]);
                var c = Math.Abs(m_spreadClosFC[0] - m_spreadOpenFC[0]);

                return Math.Max(Math.Max(a, b), c);
			}
		}
	}
}