using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class _gTestFC1 : FunctionSeries<System.Double>
		{
			public _gTestFC1(CStudyControl _master) : base(_master) { }
			public _gTestFC1(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
            //private VariableSeries<Double> m_netchgavg;
            //private VariableSeries<Double> m_totchgavg;

            private VariableSeries<Double> m_gain;
            private VariableSeries<Double> m_loss;
            private VariableSeries<Double> m_sumgain;
            private VariableSeries<Double> m_sumloss;
            private VariableSeries<Double> m_rsi;



			
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
                //m_netchgavg = new VariableSeries<Double>(this);
                //m_totchgavg = new VariableSeries<Double>(this);

                m_gain = new VariableSeries<double>(this);
                m_loss = new VariableSeries<double>(this);
                m_sumgain = new VariableSeries<double>(this);
                m_sumloss = new VariableSeries<double>(this);
                m_rsi = new VariableSeries<double>(this);

				price = new VariableSeries<Double>(this);
			}

			protected override void StartCalc()
			{
            	m_sf = 1.0/length;
                

			}

			protected override System.Double CalcBar()
			{
                price.Value = price1[0] * leg1 + price2[0] * leg2 + price3[0] * leg3;
                
                if (Bars.CurrentBar == 1)
                {
                    m_gain.Value = 0;
                    m_loss.Value = 0;
                    m_sumgain.Value = 0;
                    m_sumloss.Value = 0;
                    return 100;
                }
                else 
                {
                    double diff = price[0] - price[1];
                    m_gain.Value = (diff >= 0) ? diff : 0;
                    m_loss.Value = (diff < 0) ? diff : 0;

                    m_sumgain.Value += m_gain[0];
                    m_sumloss.Value += m_loss[0];

                    if (Bars.CurrentBar <= length)
                    {
                        //double diff = price[0] - price[1];

                        //m_sumgain.Value += (diff >= 0) ? diff : 0;
                        //m_sumloss.Value += (diff < 0) ? diff : 0;
                        return 100;
                    }
          
                    if (Bars.CurrentBar == length + 1)
                    {
                        //for (int i = 0; i < length; ++i)
                        //{
                        //    double diff = price[i] - price[i + 1];
                        //    if (diff >= 0)
                        //        m_sumgain.Value += diff;
                        //    else
                        //        m_sumloss.Value += diff;
                        //}
                        //double diff = price[0] - price[1];

                        //m_sumgain.Value += (diff >= 0) ? diff : 0;
                        //m_sumloss.Value += (diff < 0) ? diff : 0;
                        if (m_sumloss.Value == 0)
                            return 100;
                        else
                            return 100 - 100 / (1 + m_sumgain.Value / Math.Abs(m_sumloss.Value));
                    }
                    else
                    {
                        //m_sumgain.Value += m_gain[0];
                        m_sumgain.Value -= m_gain[length];
                        //m_sumloss.Value += m_loss[0];
                        m_sumloss.Value -= m_loss[length];

                        if (m_sumloss.Value == 0)
                            return 100;
                        else
                            return 100 - 100 / (1 + m_sumgain.Value / Math.Abs(m_sumloss.Value));
                    }
                 }
            }
			
		}
	}
}