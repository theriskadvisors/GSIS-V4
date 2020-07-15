using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SEA_Application.Models;
using System.Web.Optimization;
using System;

[assembly: OwinStartupAttribute(typeof(SEA_Application.Startup))]
namespace SEA_Application
{
    public partial class Startup
    {

        Sea_Entities db = new Sea_Entities();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            BundleTable.EnableOptimizations = true;
            CreateRolesandUsers();

        }

        private void AddGenders()
        {
            db.AspNetGenders.Add(new AspNetGender
            {
                Title = "Male"
            });
            db.AspNetGenders.Add(new AspNetGender
            {
                Title = "Female"
            });
        }

        private void AddNationalities()
        {
            db.AspNetNationalities.Add(new AspNetNationality
            {
                Title = "Pakistani"
            });
        }

        private void AddReligions()
        {
            db.AspNetReligions.Add(new AspNetReligion
            {
                Title = "Islam"
            });
        }

        private void AddEmployeePositions()
        {
            
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Super Principal"
            });
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Branch Principal"
            });
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Branch Admin"
            });
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Teacher"
            });
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Accountant"
            });
            db.AspNetEmployeePositions.Add(new AspNetEmployeePosition
            {
                PositionName = "Management Staff"
            });
        }

        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Super_Admin"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "Super_Admin"
                };
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser()
                {
                    UserName = "Bilal",
                    Email = "bilalb53@gmail.com"
                };
                string userPWD = "Bilal@1234";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Super_Admin");
                }

                var user1 = new ApplicationUser()
                {
                    UserName = "dummy",
                    Email = "dummy@gmail.com"
                };
                string userPWD1 = "Dummy@1234";

                var chkUser1 = UserManager.Create(user1, userPWD1);

                //Add default User to Role Admin   
                if (chkUser1.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user1.Id, "Super_Admin");
                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Teacher"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Teacher";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Student"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Student";
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists("Accountant"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Accountant";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Parent"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Parent";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Branch_Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Branch_Admin";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Branch_Principal"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Branch_Principal";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Super_Principal"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Super_Principal";
                roleManager.Create(role);

            }

        }
    }
}
