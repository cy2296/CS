using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
    namespace Function
    {
        public sealed class _gConAvgFC : FunctionSimple<System.Double>
        {
            private _gConSumFC m_conditionsumfc1;
            
            public _gConAvgFC(CStudyControl _master) : base(_master) { }
            public _gConAvgFC(CStudyControl _master, int _ds) : base(_master, _ds) { }
            
            public ISeries<Double> price { get; set; }
            public ISeries<Double> volume { get; set; }

            public double eventId { get; set; }  
            public Int32 length { get; set; }           

            protected override void Create()
            {
                m_conditionsumfc1 = new _gConSumFC(this);
            }

            protected override void StartCalc()
            {
                m_conditionsumfc1.price = price;
                m_conditionsumfc1.volume = volume;
                m_conditionsumfc1.eventId = eventId;
                m_conditionsumfc1.length = length;
            }

            protected override System.Double CalcBar()
            {
                return m_conditionsumfc1[0] / length;
            }
        }
    }
}