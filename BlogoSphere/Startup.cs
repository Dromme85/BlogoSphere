using BlogoSphere.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogoSphere.Startup))]
namespace BlogoSphere
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitialRolesAndUsers();
        }

        public void InitialRolesAndUsers()
		{
            var db = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roleManager.RoleExists("Admin"))
			{
                // No error checking made, assume everything will work anyway. TODO: Fix
                roleManager.Create(new IdentityRole("Admin"));
                userManager.Create(new ApplicationUser() { UserName = "Admin", Email = "admin@site.com" }, "adminpassword");

                userManager.AddToRole(userManager.FindByEmail("admin@site.com").Id, "Admin");
			}
		}
    }
}
