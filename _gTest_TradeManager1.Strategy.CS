using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

using PowerLanguage.TradeManager;
using ATCenterProxy;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PowerLanguage.Strategy {
	public class _gTest_TradeManager1 : SignalObject 
    {
		public _gTest_TradeManager1(object _ctx):base(_ctx){}

        private String MBTProfileName = "Interactive Brokers";
        private String MBTAccount = "DU197962";
        private String MBTSymbol = "XLE";

        private String OECProfileName = "Interactive Brokers";
        private String OECAccount = "DU197962";
        private String OECSymbol = "SPY";

        ITradingProfile OEC;
        ITradingProfile MBT;
        OrderParams MBTop;
        OrderParams OECop;
        OpenPositionManager m_OpenPositionManager;

        bool isSubscribet;

        private IOrderMarket m_rsiBbCrossLE1_Mkt;
        private IOrderMarket m_rsiBbCrossLE2_Mkt;


        protected override void Create() 
        {
            isSubscribet = false;
            MBTop = new OrderParams();
            OECop = new OrderParams();

            m_rsiBbCrossLE1_Mkt = OrderCreator.MarketThisBar
    (new SOrderParameters(Contracts.Default, "L", EOrderAction.Buy));
            m_rsiBbCrossLE2_Mkt = OrderCreator.MarketThisBar
(new SOrderParameters(Contracts.Default, "L", EOrderAction.Sell));

		}

        void Positions_Deleted(params Position[] _items)
        {
            Output.WriteLine("------>  Positions_Deleted");
            foreach (Position p in _items)
            {
                m_OpenPositionManager.Delete(p.Profile, p.BrokerSymbol);
            }
        }

        void Positions_Changed(params Position[] _items)
        {
            Output.WriteLine("------>  Positions_Changed");
            foreach (Position p in _items)
            {
                m_OpenPositionManager.Change(p.Profile, p.BrokerSymbol, p.Value, p.OpenPL, p.AvgPrice);
            }
        }

        void Positions_Added(params Position[] _items)
        {
            Output.WriteLine("------>  Positions_Added");
            foreach (Position p in _items)
            {
                m_OpenPositionManager.Add(p.Profile, p.BrokerSymbol, p.Value, p.OpenPL, p.AvgPrice);
            }
        }

		protected override void StartCalc() 
        {
            if (MBT == null)
                MBT = GetProfile(MBTProfileName);
            if (OEC == null)
                OEC = GetProfile(OECProfileName);
            if (MBT == null || OEC == null) ExecControl.Abort("No Profiles");

            m_OpenPositionManager = new OpenPositionManager(Output);

            if (!isSubscribet)
            {
                TradeManager.TradingData.Positions.Added += new TItemsChanged<Position>(Positions_Added);
                TradeManager.TradingData.Positions.Changed += new TItemsChanged<Position>(Positions_Changed);
                TradeManager.TradingData.Positions.Deleted += new TItemsChanged<Position>(Positions_Deleted);
                isSubscribet = true;
            }

            Output.Clear();
            MBTop = GetDefaultParams();
            OECop = GetDefaultParams();
		}
		protected override void CalcBar()
        {
           // m_rsiBbCrossLE1_Mkt.Send(); 
            m_rsiBbCrossLE2_Mkt.Send();


            //if (Environment.IsRealTimeCalc)
            {
                Output.WriteLine("Calc");
                TradeManager.ProcessEvents();

                PositionInfo MBTPosValue = m_OpenPositionManager.GetValue(MBTProfileName, MBTSymbol);
                PositionInfo OECPosValue = m_OpenPositionManager.GetValue(OECProfileName, OECSymbol);

                Output.WriteLine("MBTPL = {0}, OECPL = {1}", MBTPosValue.OpenPL, OECPosValue.OpenPL);
                //if (OECPosValue.OpenPL == 0 && MBTPosValue.OpenPL == 0)
                /// {
                MBTop.action = MTPA_OrdrActn.eMTPA_OA_Sell;
                OECop.action = MTPA_OrdrActn.eMTPA_OA_Buy;
                // }

                PlaceOrder(MBT, MBTSymbol, MBTAccount, MBTop);
                PlaceOrder(OEC, OECSymbol, OECAccount, OECop);



            }	
		}

        protected void PlaceOrder(ITradingProfile p, String Symbol, String Account, OrderParams op)
        {
            if (p.ConnectionState == ETM_ConnectionChanged.eTM_CC_Connected)
            {
                p.CurrentSymbol = GetSymbolInfo(p.PluginName, ESymbolCategory.Future, Symbol, "CME");
                p.CurrentAccount = Account;
                p.PlaceOrder(op);
            }
        }

        protected TradeManager.ITradingProfile GetProfile(String Name)
        {
            foreach (ITradingProfile p in TradeManager.TradingProfiles)
            {
                if (String.Equals(p.Name, Name)) return p;
            }
            return null;
        }


        protected MCSymbolInfo GetSymbolInfo(String DataSource, ESymbolCategory cat, String SymbolName, String exch)
        {
            MCSymbolInfo msymbInfo = new MCSymbolInfo();
            MTPA_MCSymbolInfo2[] si2 = SymbolStorage.GetSymbols(DataSource, SymbolName, cat);
            if (si2.Length > 1)
            {
                foreach (MTPA_MCSymbolInfo2 s in si2)
                {
                    if (String.Equals(s.SymbolExchange, exch))
                    {
                        msymbInfo.symbol = s;
                        break;
                    }
                }
            }
            else if (si2.Length == 1)
                msymbInfo.symbol = si2[0];
            else Output.WriteLine(" Error in GetSymbolInfo()");
            msymbInfo.data_feed = DataSource;
            return msymbInfo;
        }

        protected OrderParams GetDefaultParams()
        {
            OrderParams p = new OrderParams();
            p.action = ATCenterProxy.interop.MTPA_OrdrActn.eMTPA_OA_Buy;
            p.category = ATCenterProxy.interop.MTPA_OrdrCtgry.eMTPA_OC_Market;

            p.tif = ATCenterProxy.interop.MTPA_OrdrTimeInForce.eMTPA_TIF_DAY;
            p.limit_price = 0;
            p.stop_price = 0;
            p.contracts = 1;
            return p;
        }
	}


}