﻿using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gConXAvgFC : FunctionSeries<System.Double>
		{
            //private VariableSeries<Double> m_conXAvg;

			public _gConXAvgFC(CStudyControl _master) : base(_master) { }
			public _gConXAvgFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }
            public ISeries<Double> volume { get; set; }
            public double eventId { get; set; }    // 0: not a event, 1: dwon, 2: up
            public int length { get; set; }

			protected override void Create()
			{
                //m_conXAvg = new VariableSeries<Double>(this);
			}

			protected override void StartCalc()
			{
			}

			protected override System.Double CalcBar()
			{
				var barVol = volume[0];
				var barPrice = price[0];
				
				if (1 == Bars.CurrentBar)
				{
					if (volume[0] == eventId)
						return price[0];
					else
						return 0;
				}
                double prev = this[1];
				if (volume[0] == eventId)
                    return prev + 2.0 / (length + 1) * (price[0] - prev);
                else
                    return prev;
			}
		}
	}
}