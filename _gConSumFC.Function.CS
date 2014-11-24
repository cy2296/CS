using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
    namespace Function
    {
        public sealed class _gConSumFC : FunctionSeries<System.Double>
        {
            private VariableSeries<Double> m_conSum;

            private Stack<double> m_initStack; 
            private Queue<double> m_priceQueue;  
        
            public _gConSumFC(CStudyControl _master) : base(_master) { }
            public _gConSumFC(CStudyControl _master, int _ds) : base(_master, _ds) { }

            public ISeries<Double> price { get; set; }
    
            public Int32 length { get; set; }           
            
            protected override void Create()
            {
                m_conSum = new VariableSeries<Double>(this);
                m_initStack = new Stack<double>();
                m_priceQueue = new Queue<double>();
            }

            protected override void StartCalc()
            {
                // assign inputs
            }
            
            private void on_first_bar()
            {
                m_conSum.Value = 0;
                m_initStack.Clear();    
                var i = 0;
                var sSize = 0;
                while (sSize != length)
                {
                    var debug = price[i];
                    if (Bars.Volume[i] == 1)
                    {
                        m_conSum.Value += price[i];
                        m_initStack.Push(price[i]);
                        ++sSize;
                    }
                    ++i;
                }
                if (m_initStack.Count() == length)
                { 
                    // initialize price queue
                    for (var j = 0; j < length; ++j)
                        m_priceQueue.Enqueue(m_initStack.Pop());
                }
            }

            protected override System.Double CalcBar()
            {
                if (Bars.CurrentBar == 1)
                    on_first_bar();
                else
                {
                    if (Bars.Volume[0] == 1)
                    {
                        m_conSum.Value = m_conSum[1] - m_priceQueue.Dequeue() + price.Value;
                        m_priceQueue.Enqueue(price.Value);  
                    }
                }
                return m_conSum.Value;
            }
        }
    }
}