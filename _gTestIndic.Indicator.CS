using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

using System.Text;


namespace PowerLanguage.Indicator{
	public class _gTestIndic : IndicatorObject 
    {
        
		public _gTestIndic(object _ctx):base(_ctx)
        {
            //delta_length = 100;
            //delta_vxWeight = 0.6;
            //delta_upRatio = 0.5;
            //lookBack = 10;               
        }

        private VariableSeries<Double> m_spread;
        private IPlotObject plot1;

        private _gVX_fv m_theoVX_indic;
        private ISeries<double> vxFv { get; set; }

        private System.Text.StringBuilder csv;    

        //[Input]
        //public int delta_length { get; set; }
        //[Input]
        //public double delta_vxWeight { get; set; }
        //[Input]
        //public double delta_upRatio { get; set; }
        //[Input]
        //public int lookBack { get; set; }

		protected override void Create() 
        {
            
            csv = new StringBuilder();

            m_spread = new VariableSeries<double>(this);
            plot1 = AddPlot(new PlotAttributes("theoVX_vx_spread", EPlotShapes.Histogram, Color.Red));
           
            m_theoVX_indic = (_gVX_fv)AddIndicator("_gVX_fv");
            //m_theoVX_indic.delta_length = 100;
            //m_theoVX_indic.delta_vxWeight = 0.6;
            //m_theoVX_indic.delta_upRatio = 0.5;
            m_theoVX_indic.spreadLookBack = 10;		
		}
		protected override void StartCalc() 
        {
			// assign inputs 
            vxFv = BarsOfData(4).Close;    
        }
		protected override void CalcBar()
        {
			// indicator logic 
            m_spread.Value = vxFv[0] - m_theoVX_indic.Plots[0].Values[0];
            plot1.Set(0, m_spread.Value);

            string data1 = m_spread.Value.ToString();
            string data2 = m_spread.Value.ToString();
            var newLine = string.Format("{0},{1}{2}", data1, data2, System.Environment.NewLine);
            csv.Append(newLine);
            //System.IO.File.WriteAllText("C:/Users/gyang/Google Drive/DeltaAnalysis/historical", csv.ToString());
            System.IO.File.WriteAllText("C:/Users/gyang/Dropbox/mapping/test.csv", csv.ToString());
		}
        
	}
}