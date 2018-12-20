using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace NodeWebApi
{
	public static class HttpContextExtensions
	{
		public static Boolean IsLocalRequest(this HttpContext context) =>
			context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress) || IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
	}
}