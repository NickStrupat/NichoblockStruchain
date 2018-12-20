using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NodeWebApi
{
	public class LocalRequestFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.HttpContext.IsLocalRequest())
				context.Result = new StatusCodeResult(401);
			base.OnActionExecuting(context);
		}
	}
}