using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gConditionAvgFC : FunctionSimple<System.Double>
		{
			private _gConditionSumFC m_conditionsumfc1;
			
			public _gConditionAvgFC(CStudyControl _master) : base(_master) { }
			public _gConditionAvgFC(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
 	        public ISeries<Double> price { get; set; }

    	    public Int32 length { get; set; }			

			protected override void Create()
			{
				m_conditionsumfc1 = new _gConditionSumFC(this);
			}

			protected override void StartCalc()
			{
				m_conditionsumfc1.price = price;
				//m_conditionsumfc1.isCount = Bars.Volume;
				m_conditionsumfc1.length = length;
			}

			protected override System.Double CalcBar()
			{
				return m_conditionsumfc1[0] / length;
			}
		}
	}
}