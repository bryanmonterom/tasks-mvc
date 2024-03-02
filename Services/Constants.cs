using Microsoft.AspNetCore.Mvc.Rendering;

namespace TasksMVC.Services
{
    public class Constants
    {
        public const string AdminRole = "Administrator";
        public static readonly SelectListItem[] CultureUISupported = new SelectListItem[]
        {
            new SelectListItem(value:"es",text:"Español"),
            new SelectListItem(value:"en", text:"English")
    };
    }
}
