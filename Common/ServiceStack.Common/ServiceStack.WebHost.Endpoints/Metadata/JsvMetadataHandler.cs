using System;
using System.Web.UI;
using ServiceStack.Common.Utils;
using ServiceStack.ServiceModel.Serialization;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints.Support.Metadata.Controls;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	public class JsvMetadataHandler : BaseMetadataHandler
    {
		public override EndpointType EndpointType { get { return EndpointType.Jsv; } }
		
		protected override string CreateMessage(Type dtoType)
        {
            var requestObj = ReflectionUtils.PopulateObject(Activator.CreateInstance(dtoType));
			return JsvFormatter.SerializeAndFormat(requestObj);
        }

        protected override void RenderOperations(HtmlTextWriter writer, Operations allOperations)
        {
            var defaultPage = new OperationsControl
            {
				Title = EndpointHost.Config.ServiceName,
                OperationNames = allOperations.Names,
                UsageExamplesBaseUri = EndpointHost.Config.UsageExamplesBaseUri,
            };

            defaultPage.RenderControl(writer);
        }

    }
}