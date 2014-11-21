using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gConditionSumFC : FunctionSeries<System.Double>
		{
			private VariableSeries<Double> m_conSum;
			private Queue<double> m_priceQueue;  
		
			public _gConditionSumFC(CStudyControl _master) : base(_master) { }
			public _gConditionSumFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

			public ISeries<Double> price { get; set; }
			//public ISeries<Int32> isCount { get; set; }   // indicates whether price should be counted
			public Int32 length { get; set; }			
			
			
			protected override void Create()
			{
				m_conSum = new VariableSeries<Double>(this);
				m_priceQueue = new Queue<double>();
			}

			protected override void StartCalc()
			{
				// assign inputs
			}
			
			private void on_first_bar()
			{
				m_conSum.Value = 0;
				var i = 0;
				var qSize = 0;
				while (qSize < length)
				{
					var debug = price[i];
					if (Bars.Volume[0] == 1)
					{
						m_conSum.Value += price[i];
						m_priceQueue.Enqueue(price[i]);
						++qSize;
					}
					++i;
				}
			}

			protected override System.Double CalcBar()
			{
				if (Bars.CurrentBar == 1)
					on_first_bar();
				else
				{
					if (Bars.Volume[0] == 1)
						m_conSum.Value -= m_conSum[1] - m_priceQueue.First() + price.Value;
				}
				return m_conSum.Value;
			}
		}
	}
}