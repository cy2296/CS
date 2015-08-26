using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gTrailingStop : SignalObject
    {
        #region Fields
        private string positionGlobalName;
        private int positionWatch;
        private IOrderMarket trendLE;
        private IOrderMarket trendLX;
        #endregion

        public _gTrailingStop(object _ctx):base(_ctx){}
		
		protected override void Create() 
        {
            trendLE = OrderCreator.MarketThisBar(
                new SOrderParameters(Contracts.Default, "LE", EOrderAction.Buy));
            trendLX = OrderCreator.MarketThisBar(
                new SOrderParameters(Contracts.Default, "LX", EOrderAction.Sell));
		}
		protected override void StartCalc()
        {
            UpdateWatchingPos();
            positionGlobalName = "pos" + "984796";
		}
		protected override void CalcBar()
        {
            if (Bars.CurrentBar > 2)
            {
                UpdateWatchingPos();
                if (positionWatch != 0)
                    trendLE.Send();
                else
                    trendLX.Send();
	    	}
        }

        private void UpdateWatchingPos()
        {
            int retrievedInt = 0;
            if (!Globals.NamedInteger.IsEmpty
                && Globals.NamedInteger.TryGetValue(positionGlobalName, out retrievedInt))
            {
                positionWatch = retrievedInt;
            }
        }
	}
}