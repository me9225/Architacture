//using BsdFinalProject.Services;
//using System.Security.Claims;

//namespace BsdFinalProject.Middleware
//{
//    public class IsManegerMiddlewarw
//    {
//        private readonly RequestDelegate _next;
//        public IsManegerMiddlewarw(RequestDelegate next)
//        {
//            _next = next;
//        }
//        public async Task InvokeAsync(HttpContext context)
//        {
//            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrEmpty(userId))
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                await context.Response.WriteAsync("Unauthorized: No userId in token.");
//                return;
//            }

//            // בדיקה ב-DB אם המשתמש הוא מנהל
//          //  var isManager = await UserService.IsUserManagerAsync(userId);

//            if (!isManager)
//            {
//                context.Response.StatusCode = StatusCodes.Status403Forbidden;
//                await context.Response.WriteAsync("Forbidden: You do not have access to this resource.");
//                return;
//            }

//            // המשתמש הוא מנהל, ממשיכים
//            await _next(context);
//        }
    
//}
//}
