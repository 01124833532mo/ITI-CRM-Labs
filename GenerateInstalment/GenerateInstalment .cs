using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace GenerateInstalment
{
    public class GenerateInstalment : IPlugin
    {
        private const string TotalField = "cr53d_total";
        private const string CountField = "cr53d_numberofinstallment";
        private const string CarField = "cr53d_car";
        private const string ContactField = "cr53d_contact";

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                factory.CreateOrganizationService(context.UserId);

            Entity source = service.Retrieve(
                context.PrimaryEntityName,
                context.PrimaryEntityId,
                new ColumnSet(TotalField, CountField, CarField, ContactField));

            CreateInstalments(source, service);
        }

        private static void CreateInstalments(Entity source, IOrganizationService service)
        {
            int total = source.GetAttributeValue<int>(TotalField);
            int count = source.GetAttributeValue<int>(CountField);

            if (count <= 0)
            {
                return;
            }

            EntityReference car = source.GetAttributeValue<EntityReference>(CarField);
            EntityReference contact = source.GetAttributeValue<EntityReference>(ContactField);

            string carName = car != null ? car.Name : string.Empty;
            string contactName = contact != null ? contact.Name : string.Empty;
            int amountPerInstalment = total / count;

            for (int i = 1; i <= count; i++)
            {
                Entity instalment = new Entity("cr53d_instalment");
                instalment["cr53d_name"] = string.Format("{0} {1} Instalment {2}", carName, contactName, i);
                instalment["cr53d_amount"] = amountPerInstalment;
                instalment["cr53d_date"] = DateTime.Now.AddMonths(i);

                service.Create(instalment);
            }
        }
    }
}
