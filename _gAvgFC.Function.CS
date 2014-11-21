using System;

namespace PowerLanguage.Function
{
    public class _gAvgFC : FunctionSimple<Double>
    {
        private _gSumFC m_summationfc1;

        public _gAvgFC(CStudyControl ctx) :
            base(ctx) {}

        public _gAvgFC(CStudyControl ctx, int data_stream) :
            base(ctx, data_stream) {}

        public ISeries<Double> price { get; set; }

        public Int32 length { get; set; }

        protected override void Create(){
            m_summationfc1 = new _gSumFC(this);
        }

        protected override void StartCalc(){
            m_summationfc1.price = price;
            m_summationfc1.length = length;
        }


        protected override double CalcBar(){
            return m_summationfc1[0] / length;
        }
    }
}