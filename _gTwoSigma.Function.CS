using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gTwoSigma : FunctionSeries<System.Double>
		{
            private AverageFC m_averagefc1;

			public _gTwoSigma(CStudyControl _master) : base(_master) { }
			public _gTwoSigma(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }
            public int length { get; set; }
            public double sigma { get; set; }
                        
			protected override void Create()
			{
                m_averagefc1 = new AverageFC(this);
			}

			protected override void StartCalc()
			{
                m_averagefc1.price = price;
                m_averagefc1.length = length;
			}

			protected override System.Double CalcBar()
			{
                var m_sdev = price.StandardDeviationCustom(length, 1);
                return m_averagefc1[0] + (sigma * m_sdev);
			}
		}
	}
}