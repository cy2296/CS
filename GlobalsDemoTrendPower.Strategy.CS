using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    /// <summary>
    /// Based on TrendPower tutorial strategy from MultiCharts .NET Programming
    /// Guide, but with capability to save properties to global dictionaries.
    /// </summary>
    public class GlobalsDemoTrendPower : SignalObject
    {
        #region Fields
        private bool saveGlobalProperties;
        private int fastLength;
        private int slowLength;
        private double strongLevel;
        private bool firstPass;
        private string chartID;
        private string fastLengthGlobalName;
        private string slowLengthGlobalName;
        private string strongLevelGlobalName;
        private double oldPower;
        private IOrderMarket trendLE;
        private IOrderMarket trendSE;
        private IOrderMarket trendLX;
        private IOrderMarket trendSX;
        #endregion

        // Constructor
        public GlobalsDemoTrendPower(object ctx)
            : base(ctx)
        {
            saveGlobalProperties = false;
            fastLength = 7;
            slowLength = 14;
            strongLevel = 0.05;
            firstPass = true;
        }

        #region Properties
        [Input("False", "True")]
        public bool SaveGlobalProperties
        {
            get
            {
                return saveGlobalProperties;
            }

            set
            {
                saveGlobalProperties = value;

                if (value && !firstPass)
                {
                    UpdateAllGlobals();
                }
            }
        }

        [Input]
        public int FastLength
        {
            get
            {
                return fastLength;
            }

            set
            {
                fastLength = value;

                if (SaveGlobalProperties && !firstPass)
                {
                    Globals.NamedInteger.AddOrUpdate(fastLengthGlobalName, value, (k, v) => value);
                }
            }
        }

        [Input]
        public int SlowLength
        {
            get
            {
                return slowLength;
            }

            set
            {
                slowLength = value;

                if (SaveGlobalProperties && !firstPass)
                {
                    Globals.NamedInteger.AddOrUpdate(slowLengthGlobalName, value, (k, v) => value);
                }
            }
        }

        [Input]
        public double StrongLevel
        {
            get
            {
                return strongLevel;
            }

            set
            {
                strongLevel = value;

                if (SaveGlobalProperties && !firstPass)
                {
                    Globals.NamedDouble.AddOrUpdate(strongLevelGlobalName, value, (k, v) => value);
                }
            }
        }
        #endregion

        // Methods
        protected override void Create()
        {
            trendLE = OrderCreator.MarketNextBar(
                new SOrderParameters(Contracts.Default, "Trend LE", EOrderAction.Buy));

            trendSE = OrderCreator.MarketNextBar(
                new SOrderParameters(Contracts.Default, "Trend SE", EOrderAction.SellShort));

            trendLX = OrderCreator.MarketNextBar(
                new SOrderParameters(Contracts.Default, "Trend LX", EOrderAction.Sell));

            trendSX = OrderCreator.MarketNextBar(
                new SOrderParameters(Contracts.Default, "Trend SX", EOrderAction.BuyToCover));
        }

        protected override void StartCalc()
        {
            if (firstPass)
            {
                // The chartID and global names are computed here instead of in the class 
                // constructor to insure ChartWindowHWND has a valid value.  We only need
                // to do this once, so the firstPass flag makes sure of that.
                chartID = Environment.ChartWindowHWND.ToString();
                fastLengthGlobalName = "FastLength" + chartID;
                slowLengthGlobalName = "SlowLength" + chartID;
                strongLevelGlobalName = "StrongLevel" + chartID;

                if (SaveGlobalProperties)
                {
                    UpdateAllGlobals();
                }

                firstPass = false;
            }

            oldPower = 0;
        }

        protected override void CalcBar()
        {
            double aslow = AvgVal(SlowLength);
            double afast = AvgVal(FastLength);
            double power = Math.Abs(100 * (afast - aslow) / Bars.Close[0]);

            if ((power >= StrongLevel) && (oldPower < StrongLevel))
            {
                switch (TrendDir())
                {
                    case -1:
                        trendSE.Send();
                        break;
                    case 1:
                        trendLE.Send();
                        break;
                }
            }

            if ((CurrentPosition.Side != EMarketPositionSide.Flat)
                 && (oldPower >= StrongLevel)
                 && (power < StrongLevel))
            {
                trendLX.Send();
                trendSX.Send();
            }

            oldPower = power;
        }

        private double AvgVal(int length)
        {
            double aval = 0.0;

            for (int i = 0; i < length; ++i)
            {
                aval += Bars.AvgPrice(i);
            }

            return aval / length;
        }

        private int TrendDir()
        {
            if ((Bars.Low[0] < Bars.Low[1]) && (Bars.High[0] < Bars.High[1]))
            {
                return -1;
            }
            else if ((Bars.Low[0] > Bars.Low[1]) && (Bars.High[0] > Bars.High[1]))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void UpdateAllGlobals()
        {
            Globals.NamedInteger.AddOrUpdate(fastLengthGlobalName, FastLength, (k, v) => FastLength);
            Globals.NamedInteger.AddOrUpdate(slowLengthGlobalName, SlowLength, (k, v) => SlowLength);
            Globals.NamedDouble.AddOrUpdate(strongLevelGlobalName, StrongLevel, (k, v) => StrongLevel);
        }
    }
}