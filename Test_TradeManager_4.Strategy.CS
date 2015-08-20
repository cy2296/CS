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



namespace PowerLanguage.Strategy
{
  public class Test_TradeManager_4 : SignalObject
  {
    public Test_TradeManager_4(object _ctx) : base(_ctx) { ctx = _ctx; }

    object ctx;
    private IOrderMarket buy_order;

    private String MBTProfileName = "MB Trading";
    private String MBTAccount = "35031736";
    private String MBTSymbol = "/ESU2";

    private String OECProfileName = "Open E Cry";
    private String OECAccount = "DEMO342738";
    private String OECSymbol = "ESM5";

    bool MBTIsLastTradePositive;
    bool OECIsLastTradePositive;

    bool MBTIsOrderPased;
    bool OECIsOrderPased;

    double m_TakeProfitValue = 12.5;
    double m_StopLossValue = -25;

    ITradingProfile OEC;
    ITradingProfile MBT;
    OrderParams MBTop;
    OrderParams OECop;
    OpenPositionManager m_OpenPositionManager;

    bool isSubscribet;

    protected override void Create()
    {
      isSubscribet = false;
      MBTop = new OrderParams();
      OECop = new OrderParams();
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
				m_OpenPositionManager.Change( p.Profile, p.BrokerSymbol, p.Value, p.OpenPL, p.AvgPrice );
      }
    }

    void Positions_Added(params Position[] _items)
    {
      Output.WriteLine("------>  Positions_Added");
      foreach (Position p in _items)
      {
				m_OpenPositionManager.Add( p.Profile, p.BrokerSymbol, p.Value, p.OpenPL, p.AvgPrice );
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
      if (Environment.IsRealTimeCalc)
      {
        Output.WriteLine("Calc");
        TradeManager.ProcessEvents();

        PositionInfo MBTPosValue = m_OpenPositionManager.GetValue(MBTProfileName, MBTSymbol);
        PositionInfo OECPosValue = m_OpenPositionManager.GetValue(OECProfileName, OECSymbol);

        Output.WriteLine("MBTPL = {0}, OECPL = {1}", MBTPosValue.OpenPL, OECPosValue.OpenPL);
        if (OECPosValue.OpenPL == 0 && MBTPosValue.OpenPL == 0)
        {
          MBTop.action = MTPA_OrdrActn.eMTPA_OA_Sell;
          OECop.action = MTPA_OrdrActn.eMTPA_OA_Buy;
        }

        if (OECPosValue.OpenPL < m_StopLossValue && OECIsOrderPased)
        {
          OECop.action = GetAction(OECPosValue.Quantity);
          OECIsLastTradePositive = false;
          PlaceOrder(OEC, OECSymbol, OECAccount, OECop);
          OECIsOrderPased = false;
        }
        if (OECPosValue.OpenPL >= m_TakeProfitValue && OECIsOrderPased)
        {
          OECop.action = GetAction(OECPosValue.Quantity);
          OECIsLastTradePositive = true;

          PlaceOrder(OEC, OECSymbol, OECAccount, OECop);
          OECIsOrderPased = false;
        }

        if (OECPosValue.OpenPL == 0 && !OECIsOrderPased)
        {
          if (OECIsLastTradePositive)
          {
            OECop.action = ReverseAction(OECop.action);
          }
          PlaceOrder(OEC, OECSymbol, OECAccount, OECop);
          OECIsOrderPased = true;
        }


        if (MBTPosValue.OpenPL < m_StopLossValue && MBTIsOrderPased)
        {
          MBTop.action = GetAction(MBTPosValue.Quantity);
          MBTIsLastTradePositive = false;
          PlaceOrder(MBT, MBTSymbol, MBTAccount, MBTop);
          MBTIsOrderPased = false;
        }
        if (MBTPosValue.OpenPL >= m_TakeProfitValue && MBTIsOrderPased)
        {
          MBTop.action = GetAction(MBTPosValue.Quantity);
          MBTIsLastTradePositive = true;

          PlaceOrder(MBT, MBTSymbol, MBTAccount, MBTop);
          MBTIsOrderPased = false;
        }

        if (MBTPosValue.OpenPL == 0 && !MBTIsOrderPased)
        {
          if (MBTIsLastTradePositive)
          {
            MBTop.action = ReverseAction(MBTop.action);
          }
          PlaceOrder(MBT, MBTSymbol, MBTAccount, MBTop);
          MBTIsOrderPased = true;
        }
      }

      if (Bars.LastBarOnChart)
      {
        Output.WriteLine("PNL MBT = {0}", GetClosedPL(MBTProfileName, MBTSymbol));
        Output.WriteLine("PNL OEC = {0}", GetClosedPL(OECProfileName, OECSymbol));
      }
    }

    protected IEnumerable<PowerLanguage.TradeManager.Order> GetOrderList(String Profile, 
                                                                         String symbol, 
                                                                         ETM_OrderState _stat)
    {
      TradeManager.ProcessEvents();
      return from o in TradeManager.TradingData.Orders.Items
             where (o.Profile == Profile && o.BrokerSymbol == symbol && o.State == _stat)
             select o;
    }


    protected double GetClosedPL(String Profile, String Symbol)
    {
      IEnumerable<PowerLanguage.TradeManager.Order> ords = GetOrderList(Profile, Symbol, ETM_OrderState.eTM_OS_Filled);
      double result = 0;
      MCSymbolInfo si2 = GetSymbolInfo(Profile, ESymbolCategory.Future, Symbol, "CME");
      double pipPrice = si2.symbol.PriceScale * si2.symbol.MinMove * si2.symbol.BigPointValue;
      foreach (TradeManager.Order o in ords)
      {
        if (o.Action == ETM_OrderAction.eTM_OA_Buy)
          result += o.ExecPrice.Value * o.FilledContracts * pipPrice;
        else
          result -= o.ExecPrice.Value * o.FilledContracts * pipPrice;
      }

      return result;
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

    protected MTPA_OrdrActn GetAction(int Value)
    {
      if (Value > 0)
        return MTPA_OrdrActn.eMTPA_OA_Sell;

      return MTPA_OrdrActn.eMTPA_OA_Buy;
    }

    protected MTPA_OrdrActn ReverseAction(MTPA_OrdrActn Value)
    {
      if (Value == MTPA_OrdrActn.eMTPA_OA_Buy)
        return MTPA_OrdrActn.eMTPA_OA_Sell;

      return MTPA_OrdrActn.eMTPA_OA_Buy;
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

  public struct PositionInfo
  {
    public int Quantity;
    public double OpenPL;
		public double FilledPrice;
  }

  public class OpenPositionManager
  {
    Dictionary<String, PositionInfo> m__Positions;

    object mLocker = new object();
    IOutput OutPut; 

    public OpenPositionManager(object _ctx)
    {
      OutPut = (IOutput)_ctx;
      m__Positions = new Dictionary<String, PositionInfo>();
    }

		public void Add( String Profile, String Symbol, int value, double OpenPL, double AvgPrice )
    {
      String key = GetKey(Profile, Symbol);

      lock (mLocker)
      {
        if (m__Positions.ContainsKey(key))
        {
          OutPut.WriteLine("Error, Key \"{0}\" already Exist", key);
          m__Positions.Remove(key);
        }

        PositionInfo p = new PositionInfo();
        p.OpenPL = OpenPL;
        p.Quantity = value;
				p.FilledPrice = AvgPrice;
        m__Positions.Add(key, p);
      }
    }

    public void Change(String Profile, String Symbol, int value, double OpenPL, double AvgPrice)
    {
      String key = GetKey(Profile, Symbol);

      lock (mLocker)
      {
        if (!m__Positions.ContainsKey(key))
        {
          OutPut.WriteLine("Error, Key \"{0}\" is not found", key);
        }
        m__Positions.Remove(key);
        Add(Profile, Symbol, value, OpenPL, AvgPrice);
      }
    }

    public void Delete(String Profile, String Symbol)
    {
      String key = GetKey(Profile, Symbol);

      lock (mLocker)
      {
        if (!m__Positions.ContainsKey(key))
        {
          OutPut.WriteLine("Error, Key \"{0}\" is not found", key);
        }
        m__Positions.Remove(key);
      }
    }

    public PositionInfo GetValue(String Profile, String Symbol)
    {
      String key = GetKey(Profile, Symbol);
      PositionInfo res = new PositionInfo();
      lock (mLocker)
      {
        if (m__Positions.ContainsKey(key))
          res = m__Positions[key];
      }
      return res;
    }

    protected String GetKey(String Profile, String Symbol)
    {
      return Profile + "_" + Symbol;
    }
  }
}