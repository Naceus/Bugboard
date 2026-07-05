using BugBoard.Api.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace BugBoard.Api.Controllers
{
    public class BaseController : Controller
    {

        protected bool IsStaffUser()
        {
            return User.IsInRole(ApplicationRoles.Admin) || User.IsInRole(ApplicationRoles.Developer);
        }
    }
}
