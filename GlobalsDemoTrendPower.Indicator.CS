using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    /// <summary>
    /// Based on TrendPower tutorial indicator from MultiCharts .NET Programming
    /// Guide, but with capability to use properties saved in global dictionaries.
    /// </summary>
    public class GlobalsDemoTrendPower : IndicatorObject
    {
        #region Fields
        private int fastLength;
        private int fastLengthGlobal;
        private int slowLength;
        private int slowLengthGlobal;
        private double strongLevel;
        private double strongLevelGlobal;
        private bool firstPass;
        private string chartID;
        private string fastLengthGlobalName;
        private string slowLengthGlobalName;
        private string strongLevelGlobalName;
        private IPlotObject powerLine;
        #endregion

        // Constructor
        public GlobalsDemoTrendPower(object ctx)
            : base(ctx)
        {
            firstPass = true;
            fastLength = 7;
            slowLength = 14;
            strongLevel = 0.05;
            fastLengthGlobal = fastLength;
            slowLengthGlobal = slowLength;
            strongLevelGlobal = strongLevel;
            UseGlobalProperties = false;
            WeakColor = Color.Red;
            StrongColor = Color.Cyan;
            DoPeriodicRefresh = false;
            RefreshIntervalSeconds = 1;
        }

        #region Properties
        [Input("False", "True")]
        public bool UseGlobalProperties { get; set; }

        [Input]
        public int FastLength
        {
            get
            {
                if (UseGlobalProperties && !firstPass)
                {
                    return fastLengthGlobal;
                }
                else
                {
                    return fastLength;
                }
            }

            set
            {
                // This value only applies if UseGlobalProperties == false.
                // The indicator does not modify the global dictionaries.
                fastLength = value;
            }
        }

        [Input]
        public int SlowLength
        {
            get
            {
                if (UseGlobalProperties && !firstPass)
                {
                    return slowLengthGlobal;
                }
                else
                {
                    return slowLength;
                }
            }

            set
            {
                // This value only applies if UseGlobalProperties == false.
                // The indicator does not modify the global dictionaries.
                slowLength = value;
            }
        }

        [Input]
        public double StrongLevel
        {
            get
            {
                if (UseGlobalProperties && !firstPass)
                {
                    return strongLevelGlobal;
                }
                else
                {
                    return strongLevel;
                }
            }

            set
            {
                // This value only applies if UseGlobalProperties == false.
                // The indicator does not modify the global dictionaries.
                strongLevel = value;
            }
        }

        [Input]
        public Color StrongColor { get; set; }

        [Input]
        public Color WeakColor { get; set; }

        [Input("False", "True")]
        public bool DoPeriodicRefresh { get; set; }

        [Input("1", "2", "3", "4", "5")]
        public double RefreshIntervalSeconds { get; set; }
        #endregion

        // Methods
        protected override void Create()
        {
            powerLine = AddPlot(new PlotAttributes("Trend Power", EPlotShapes.Histogram, Color.Cyan));
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
                firstPass = false;
            }

            if (UseGlobalProperties)
            {
                UpdateGlobalBackingStores();
            }
        }

        protected override void CalcBar()
        {
            double afast = AvgVal(FastLength);
            double aslow = AvgVal(SlowLength);
            double power = Math.Abs(100 * (afast - aslow) / Bars.Close[0]);
            Color plotColor = WeakColor;

            if (power >= StrongLevel)
            {
                plotColor = StrongColor;
            }

            powerLine.Set(power, plotColor);

            if (DoPeriodicRefresh)
            {
                ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(RefreshIntervalSeconds));
            }
        }

        protected override void OnRecalcLastBarAfterEvent()
        {
            ExecControl.Recalculate();
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

        private void UpdateGlobalBackingStores()
        {
            int retrievedInt = 0;
            double retrievedDouble = 0;

            if (Globals.NamedInteger.TryGetValue(fastLengthGlobalName, out retrievedInt))
            {
                fastLengthGlobal = retrievedInt;
            }

            if (Globals.NamedInteger.TryGetValue(slowLengthGlobalName, out retrievedInt))
            {
                slowLengthGlobal = retrievedInt;
            }

            if (Globals.NamedDouble.TryGetValue(strongLevelGlobalName, out retrievedDouble))
            {
                strongLevelGlobal = retrievedDouble;
            }
        }
    }
}