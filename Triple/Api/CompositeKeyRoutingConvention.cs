using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace Triple.Api
{
    public class CompositeKeyRoutingConvention : EntityRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            var action = base.SelectAction(odataPath, controllerContext, actionMap);
            if (action == null)
            {
                return null;
            }

            var routeValues = controllerContext.RouteData.Values;
            if (!routeValues.ContainsKey(ODataRouteConstants.Key))
            {
                return action;
            }

            var keyRaw = routeValues[ODataRouteConstants.Key] as string;
            if (keyRaw == null)
            {
                return action;
            }

            IEnumerable<string> compoundKeyPairs = keyRaw.Split(',');
            if (!compoundKeyPairs.Any())
            {
                return action;
            }

            foreach (var compoundKeyPair in compoundKeyPairs)
            {
                var pair = compoundKeyPair.Split('=');
                if (pair.Length != 2)
                {
                    continue;
                }

                var keyName = pair[0].Trim();
                var keyValue = pair[1].Trim();
                routeValues.Add(keyName, keyValue);
            }
            return action;
        }
    }
}
