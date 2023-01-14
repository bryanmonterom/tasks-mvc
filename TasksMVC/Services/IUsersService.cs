using System.Security.Claims;

namespace TasksMVC.Services
{
    public interface IUsersService
    {
        string GetUserId();
    }

    public class UsersService : IUsersService
    {
        private HttpContext httpContext;

        public UsersService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public string GetUserId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                return idClaim.Value;
            }
            else
            {
                throw new Exception("The user is not authenticated");
            }
        }
    }
}
