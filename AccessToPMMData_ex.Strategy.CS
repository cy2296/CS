
using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
  public class AccessToPMMData_ex : PortfolioSignalObject // instead SignalObject
  {
    public AccessToPMMData_ex(object _ctx) : base(_ctx) { }
    private IOrderMarket buy_order;
    private IOrderMarket sell_order;

    protected override void Create()
    {
      buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
      sell_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.SellShort));
    }

    protected override void StartCalc()
    {
      Output.Clear();
    }

    protected override void CalcBar()
    {
      if (Bars.CurrentBar % 5 == 0)
        buy_order.Send();
      else
        sell_order.Send();

      if (Bars.LastBarOnChart)
      {
        Output.WriteLine("# of downloaded instruments: {0}", PortfolioStrategies.Count());
        foreach (IPortfolioStrategy p_strtg in PortfolioStrategies)
        {
          foreach (IStrategy strtg in p_strtg.Signals)
          {
            Output.WriteLine("Calc Symbol: {3},  Strategy SymbolName: {2} Siganl Name: {0}, MaxDrawDown: {1}", 
                strtg.Name, strtg.MaxDrawDown, strtg.Bars.Info.Name, Bars.Info.Name);
          }
        }
      }
    }
  }
}