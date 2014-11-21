using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _GaryTrendPower : SignalObject 
	{
		public _GaryTrendPower(object _ctx):base(_ctx)
		{
			fastLength = 7;
			slowLength = 14;
			strongLevel = 0.9;			
		}

		[Input]
		public int fastLength { get; set; }
		[Input]
		public int slowLength { get; set; }
		[Input]
		public double strongLevel { get; set; }

		private IOrderMarket trend_LE;
		private IOrderMarket trend_SE;
		private IOrderMarket trend_LX;
		private IOrderMarket trend_SX;

		// compute the "avg median price" for the period
		private double AvgVal(int length)
		{
			double aval = 0.0;
			for (int i = 0; i < length; ++i)
				aval += Bars.AvgPrice(i);
			return aval / length;
		}
		// trend direction definition method
		private int TrendDir()
		{
			// lower low && lower high
			if ((Bars.Low[0] < Bars.Low[1]) && (Bars.High[0] < Bars.High[1]))
				return -1;
			// high low && higher high
			if ((Bars.Low[0] > Bars.Low[1]) && (Bars.High[0] > Bars.High[1]))
				return 1;
			return 0;
		}

		protected override void Create() 
		{
			// create variable objects, function objects, order objects etc.
			trend_LE = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend LE", EOrderAction.Buy));
			trend_SE = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend SE", EOrderAction.SellShort));
			trend_LX = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend LX", EOrderAction.Sell));
			trend_SX = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Tend SX", EOrderAction.BuyToCover));
		}
		
		double old_power;
		protected override void StartCalc() 
		{
			// assign inputs 
			old_power = 0;
		}

		protected override void CalcBar()
		{
			double afast = AvgVal(fastLength);
			double aslow = AvgVal(slowLength);
			double power = Math.Abs(100 * (afast - aslow) / Bars.Close[0]);
			if ((power >= strongLevel) && (old_power < strongLevel))
			{
				switch (TrendDir())
				{
					case -1:
						trend_SE.Send();
						break;
					case 1:
						trend_LE.Send();
						break;
				}
			}
			if ((CurrentPosition.Side != EMarketPositionSide.Flat)
				&& (old_power >= strongLevel)
				&& (power < strongLevel))
			{
				trend_LX.Send();
				trend_SX.Send();
			}
		}
	}
}