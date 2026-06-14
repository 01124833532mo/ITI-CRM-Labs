using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

public class GenerateNumber : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));

        if (context.MessageName != "Create")
            return;

        var factory = (IOrganizationServiceFactory)
            serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        var service = factory.CreateOrganizationService(context.UserId);

        var target = (Entity)context.InputParameters["Target"];

        var query = new QueryExpression("ititrain_numbersequence")
        {
            ColumnSet = new ColumnSet(true)
        };

        query.Criteria.AddCondition(
            "ititrain_tablename",
            ConditionOperator.Equal,
            context.PrimaryEntityName);

        var sequence = service
            .RetrieveMultiple(query)
            .Entities
            .FirstOrDefault();

        if (sequence == null)
            return;

        string prefix = sequence.GetAttributeValue<string>("ititrain_sequence");
        string columnName = sequence.GetAttributeValue<string>("ititrain_columnname");

        int currentNumber =
            sequence.GetAttributeValue<int>("ititrain_lastnumber");

        currentNumber++;

        target[columnName] = $"{prefix}{currentNumber:D5}";

        sequence["ititrain_lastnumber"] = currentNumber;

        service.Update(sequence);
    }
}