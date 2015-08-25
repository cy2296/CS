using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class _gTestStrat1 : SignalObject 
    {
        #region Fields
        private XAverage m_averagefc;
        private bool saveGlobalProperties;
        private bool firstPass;
        private string chartID;
        private string positionGlobalName;
        private IOrderMarket trendLE;
        private IOrderMarket trendLX;
        #endregion

		public _gTestStrat1(object _ctx):base(_ctx)
        {
            saveGlobalProperties = true;
            firstPass = true;
            length = 100;
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
        public int length { get; set; }
        #endregion
        protected override void Create() 
        {
            m_averagefc = new XAverage(this);
            trendLE = OrderCreator.MarketThisBar(
                new SOrderParameters(Contracts.Default, "Trend LE", EOrderAction.Buy));
            trendLX = OrderCreator.MarketThisBar(
                new SOrderParameters(Contracts.Default, "Trend LX", EOrderAction.Sell));
		}
		protected override void StartCalc() 
        {
            m_averagefc.Price = Bars.Close;
            m_averagefc.Length = length;
            if (firstPass)
            {
               // chartID = Environment.ChartWindowHWND.ToString();

                //positionGlobalName = "pos" + chartID;
                positionGlobalName = "pos";
                if (SaveGlobalProperties)
                {
                    Globals.NamedInteger.AddOrUpdate(positionGlobalName, 0, (k, v) => 0);
                }
                firstPass = false;
            }
		}
		protected override void CalcBar()
        {
            UpdateAllGlobals();
            if (Bars.Close[0] > m_averagefc[0] && Bars.Close[1] < m_averagefc[1])
                trendLE.Send();
            if (Bars.Close[0] < m_averagefc[0] && Bars.Close[1] > m_averagefc[1])
                trendLX.Send();
		}

        private void UpdateAllGlobals()
        {
            Globals.NamedInteger.AddOrUpdate(positionGlobalName, StrategyInfo.MarketPosition, (k, v) => StrategyInfo.MarketPosition);
        }
	}
}