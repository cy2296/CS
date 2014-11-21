using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _GaryRevAverageFC : FunctionSimple<System.Double>
		{
			private SummationFC m_summationfc1;
			
			public _GaryRevAverageFC(CStudyControl _master) : base(_master) { }
			public _GaryRevAverageFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

			public ISeries<Double> price { get; set; }
			public Int32 length { get; set; }
			
			protected override void Create()
			{
				// create variable objects and function objects
				m_summationfc1 = new SummationFC(this);
			}

			protected override void StartCalc()
			{
				// assign inputs
				m_summationfc1.price = price;
				m_summationfc1.length = length;		
			}

			protected override System.Double CalcBar()
			{
				// function logic
				return 1 / (m_summationfc1[0] / length);
			}
		}
	}
}