using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Custom_Actions
{
    public class CalcDays : CodeActivity
    {
        [Input("First Date")]
        public InArgument<DateTime> FirstDate { get; set; }

        [Input("Last Date")]
        public InArgument<DateTime> LastDate { get; set; }

        [Output("Days Count")]
        public OutArgument<int> DaysCount { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime first = FirstDate.Get(context);
            DateTime last = LastDate.Get(context);

            int days = CalculateWholeDaysBetween(first, last);

            DaysCount.Set(context, days);
        }

        private static int CalculateWholeDaysBetween(DateTime first, DateTime last)
        {

            TimeSpan span = last.Date - first.Date;
            return span.Days;
        }
    }
}
