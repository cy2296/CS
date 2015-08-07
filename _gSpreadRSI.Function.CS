using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gSpreadRSI : FunctionSeries<System.Double>
		{
			public _gSpreadRSI(CStudyControl _master) : base(_master) { }
			public _gSpreadRSI(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
			private VariableSeries<Double> m_netchgavg;
    		private VariableSeries<Double> m_totchgavg;
			
			private VariableSeries<Double> price;
	
    	    private double m_sf;

         	public ISeries<Double> price1 { get; set; }
			public ISeries<Double> price2 { get; set; }
            public ISeries<Double> price3 { get; set; }		
			public double leg1 { get; set; }
			public double leg2 { get; set; }
            public double leg3 { get; set; }
 		    public Int32 length { get; set; }

	        protected override void Create()
			{
     	        m_netchgavg = new VariableSeries<Double>(this);
        	    m_totchgavg = new VariableSeries<Double>(this);
				price = new VariableSeries<Double>(this);
    	    }

        	protected override void StartCalc()
			{
            	m_sf = 1.0/length;
        	}

        	protected override double CalcBar()
        	{
                price.Value = price1[0] * leg1 + price2[0] * leg2 + price3[0] * leg3;
				
            	if (Bars.CurrentBar == 1)
            	{
                	m_netchgavg.Value = (price.Value - price[length]) / length;
                	m_totchgavg.Value = new Lambda<Double>(_bb => Math.Abs(price[_bb] - price[1 + _bb])).Average(length);
            	}
            	else
            	{
                	double m_change = price.Value - price[1];
                	m_netchgavg.Value = m_netchgavg[1] + m_sf*(m_change - m_netchgavg[1]);
                	m_totchgavg.Value = m_totchgavg[1] + m_sf*(Math.Abs(m_change) - m_totchgavg[1]);
            	}
				
            	Double chgratio = 0.0;
            	if (!PublicFunctions.DoubleEquals(m_totchgavg.Value , 0.0))
            	{
                	chgratio = m_netchgavg.Value/m_totchgavg.Value;
            	}

            	return 50*(chgratio + 1);
        	}
		}
	}
}