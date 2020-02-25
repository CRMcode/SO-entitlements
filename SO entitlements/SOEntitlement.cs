using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO_entitlements
{
    public class SOEntitlement : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Get a reference to the Organization service.
            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            ITracingService tracingService =
               (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") &&
               context.InputParameters["Target"] is Entity) {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName != "salesorderdetail")
                    return;
                try
                {
                    var productId = "6d47c934-1de7-e611-80f4-e0071b661f01";
                    var so_detail_id = "5b8d511d-d456-ea11-a811-000d3a530fe5";
                    Guid retreivedProductGuid = new Guid(productId.ToString());
;                   var cols = new ColumnSet(true);
                    Entity retrievedProductEntity = service.Retrieve("product", retreivedProductGuid, cols);
                    var isEligibleForEntitlement = retrievedProductEntity["so_eligibleforentitlement"];
                    var warranty = retrievedProductEntity["so_warrantymonths"];

                    var SO_CreatedDate = entity["createdon"];

                    Console.Write("retrieved ");
                    
                    Entity entitlementEntity = new Entity("so_soentitlement");
                    entitlementEntity["so_name"] = "sampleEntitlement";
                    entitlementEntity["so_startdate"] = SO_CreatedDate;
                    entitlementEntity["so_enddate"] = SO_CreatedDate;

                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "salesorderdetail";
                        entitlementEntity["so_solineitemid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                    }

                    service.Create(entitlementEntity);
                    //test commit develop branch
                    throw new Exception("Some test exception : null handled");
                }
                catch (Exception e) {
                    tracingService.Trace("Issue in SO entitlement plugin: {0}", e.ToString());
                    throw;
                }
            }
        }
    }
}
