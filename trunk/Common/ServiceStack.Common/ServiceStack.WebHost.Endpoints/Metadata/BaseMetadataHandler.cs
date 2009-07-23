using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using ServiceStack.Logging;
using ServiceStack.WebHost.Endpoints.Support;
using ServiceStack.WebHost.Endpoints.Support.Metadata.Controls;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	public abstract class BaseMetadataHandler : HttpHandlerBase
    {
		private static ILog log = LogManager.GetLogger(typeof(BaseMetadataHandler));

        const string RESPONSE_SUFFIX = "Response";

		public abstract EndpointType EndpointType { get; }
		protected HttpRequest Request { get; set; }

        public override void Execute(HttpContext context)
        {
            this.Request = context.Request;
            var writer = new HtmlTextWriter(context.Response.Output);
            context.Response.ContentType = "text/html";

			ProcessOperations(writer);
        }

		protected virtual void ProcessOperations(HtmlTextWriter writer)
		{
			var operations = EndpointHost.ServiceOperations;
			var operationName = Request.QueryString["op"];
			if (operationName != null)
			{
				var allTypes = operations.AllOperations.Types;
				var operationType = allTypes.Single(x => x.Name == operationName);
				var requestMessage = CreateMessage(operationType);
				string responseMessage = null;
				if (allTypes.Any(x => x.Name == operationName + RESPONSE_SUFFIX))
				{
					var operationResponseType = allTypes.Single(x => x.Name == operationName + RESPONSE_SUFFIX);
					responseMessage = CreateMessage(operationResponseType);
				}
				RenderOperation(writer, operationName, requestMessage, responseMessage);
				return;
			}

			RenderOperations(writer, operations.AllOperations);
		}

        protected abstract string CreateMessage(Type dtoType);

		protected virtual void RenderOperation(HtmlTextWriter writer, string operationName, 
			string requestMessage, string responseMessage)
		{
			//TODO: Fix bug in mono fastcgi, when trying to get 'Request.Url.Host'
			var urlHostName = "bug-in-mono-fastcgi";
			try
			{
				urlHostName = Request.Url.Host;
			}
			catch (Exception ex)
			{				
				EndpointHost.Config.LogFactory.GetLogger(GetType()).ErrorFormat("Error trying to get 'Request.Url.Host'", ex);
				log.ErrorFormat("Error trying to get 'Request.Url.Host'", ex);
			}
			var operationControl = new OperationControl {
				MetadataConfig = EndpointHost.Config.ServiceEndpointsMetadataConfig,
				Title = EndpointHost.Config.ServiceName,
                EndpointType = this.EndpointType,
				OperationName = operationName,
				HostName = urlHostName,
				RequestMessage = requestMessage,
				ResponseMessage = responseMessage,
			};
			operationControl.Render(writer);
		}

        protected abstract void RenderOperations(HtmlTextWriter writer, Operations allOperations);
    }
}