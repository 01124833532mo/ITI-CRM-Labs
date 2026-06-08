using Microsoft.Xrm.Sdk;
using System;

namespace LoanPlugin
{
    public class LoanPlugin : IPlugin
    {
        private const string DaysField = "cr53d_numberofdays";
        private const string PriceField = "cr53d_rentpriceperday";
        private const string TotalField = "cr53d_total";
        private const string PreImageName = "PreImage";

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (!context.InputParameters.Contains("Target") ||
                !(context.InputParameters["Target"] is Entity))
            {
                return;
            }

            Entity target = (Entity)context.InputParameters["Target"];
            Entity preImage = null;

            if (context.PreEntityImages.Contains(PreImageName))
            {
                preImage = context.PreEntityImages[PreImageName];
            }

            int days = GetIntValue(target, preImage, DaysField);
            int price = GetIntValue(target, preImage, PriceField);

            decimal total = CalculateTotal(days, price);

            target[TotalField] = total;
        }

        private static int GetIntValue(Entity target, Entity preImage, string attributeName)
        {
            if (target.Attributes.Contains(attributeName))
            {
                return target.GetAttributeValue<int>(attributeName);
            }

            if (preImage != null && preImage.Attributes.Contains(attributeName))
            {
                return preImage.GetAttributeValue<int>(attributeName);
            }

            return 0;
        }

        private static decimal CalculateTotal(int days, int price)
        {
            return days * price;
        }
    }
}
