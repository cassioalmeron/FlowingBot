using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace FlowingBot.Api.Filters
{
    public class LoggingActionFilter : IActionFilter, IAsyncActionFilter
    {
        private readonly Serilog.ILogger _logger;
        private Stopwatch _stopwatch;

        public LoggingActionFilter()
        {
            _logger = Log.ForContext<LoggingActionFilter>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            var controllerName = context.Controller.GetType().Name;
            var actionName = GetActionName(context);
            var requestPath = context.HttpContext.Request.Path;
            var requestMethod = context.HttpContext.Request.Method;

            _logger.Information("Action executing: {Controller}.{Action} - {Method} {Path}", 
                controllerName, 
                actionName, 
                requestMethod, 
                requestPath);

            // Call custom method on controller if it's a BaseController
            CallCustomMethodOnController(context.Controller, "OnActionExecuting", actionName);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();
            var controllerName = context.Controller.GetType().Name;
            var actionName = GetActionName(context);
            var statusCode = context.HttpContext.Response.StatusCode;

            if (context.Exception != null)
            {
                _logger.Error(context.Exception, "Action failed: {Controller}.{Action} after {ElapsedMs}ms", 
                    controllerName, 
                    actionName, 
                    _stopwatch?.ElapsedMilliseconds);
                
                // Call custom exception method on controller
                CallCustomMethodOnController(context.Controller, "OnActionException", actionName, context.Exception);
            }
            else
            {
                _logger.Information("Action completed: {Controller}.{Action} - Status: {StatusCode} in {ElapsedMs}ms", 
                    controllerName, 
                    actionName, 
                    statusCode, 
                    _stopwatch?.ElapsedMilliseconds);
                
                // Call custom method on controller
                CallCustomMethodOnController(context.Controller, "OnActionExecuted", actionName, context.Result);
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            OnActionExecuting(context);

            if (context.Result == null)
            {
                var executedContext = await next();
                OnActionExecuted(executedContext);
            }
        }

        private string GetActionName(ActionExecutingContext context)
        {
            if (context.RouteData.Values.TryGetValue("action", out var actionValue))
                return actionValue?.ToString() ?? "Unknown";
            
            return context.ActionDescriptor.DisplayName ?? context.ActionDescriptor.Id;
        }

        private string GetActionName(ActionExecutedContext context)
        {
            if (context.RouteData.Values.TryGetValue("action", out var actionValue))
                return actionValue?.ToString() ?? "Unknown";
            
            return context.ActionDescriptor.DisplayName ?? context.ActionDescriptor.Id;
        }

        private void CallCustomMethodOnController(object controller, string methodName, params object[] parameters)
        {
            try
            {
                var controllerType = controller.GetType();
                var method = controllerType.GetMethod(methodName, 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (method != null)
                    method.Invoke(controller, parameters);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to call custom method {MethodName} on controller {ControllerType}", 
                    methodName, 
                    controller.GetType().Name);
            }
        }
    }
} 