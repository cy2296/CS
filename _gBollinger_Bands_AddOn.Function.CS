using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gBollinger_Bands_AddOn : FunctionSeries<System.Double>
		{
			private AverageFC m_averagefc1;

			private VariableSeries<Double> m_lowerband;
			private VariableSeries<Double> m_middleband;
			private VariableSeries<Double> m_upperband;

			//public _gBollinger_Bands_AddOn(CStudyControl _master) : base(_master) { }
			//public _gBollinger_Bands_AddOn(CStudyControl _master, int _ds) : base(_master, _ds) { }
			public _gBollinger_Bands_AddOn(CStudyControl _master, int len, double devUp, double devDn, int type) : base(_master) 
			{
				length = len;
				numdevsup = devUp;
				numdevsdn = devDn;
				returnType = type;				
			}

			public ISeries<double> bollingerprice { get; set; }

			public int length { get; set; }
			public double numdevsup { get; set; }
			public double numdevsdn { get; set; }
			public int returnType { get; set; }

			protected override void Create()
			{
				m_averagefc1 = new AverageFC(this);
				m_lowerband = new VariableSeries<Double>(this);
				m_middleband = new VariableSeries<Double>(this);
				m_upperband = new VariableSeries<Double>(this);
			}

			protected override void StartCalc()
			{
				m_averagefc1.price = bollingerprice;
				m_averagefc1.length = length;
			}

			protected override System.Double CalcBar()
			{
				if (length + 1 == Bars.CurrentBar)
				{
					var m_avg = m_averagefc1[0];
					var m_sdev = bollingerprice.StandardDeviationCustom(length, 1);
					m_middleband.Value = m_avg;
					m_upperband.Value = (m_avg + (numdevsup * m_sdev));
					m_lowerband.Value = (m_avg - (numdevsdn * m_sdev));
					if (0 == returnType)
						return m_lowerband.Value;
					else if (1 == returnType)
						return m_middleband.Value;
					else
						//else if (2 == returnType)
						return m_upperband.Value;
				}
				else
					return bollingerprice[0];
			}
		}
	}
}