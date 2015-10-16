using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
    [IOGMode(IOGMode.Enabled)]
	public class _gTestStrat4 : SignalObject 
    {
        private System.Text.StringBuilder csv;  // for debug only

		public _gTestStrat4(object _ctx):base(_ctx)
        {
        
        
        }
		
		protected override void Create() 
        {
            csv = new StringBuilder();

		}
		protected override void StartCalc()
        {

		}
		protected override void CalcBar()
        {
           
    

		}
	}
}