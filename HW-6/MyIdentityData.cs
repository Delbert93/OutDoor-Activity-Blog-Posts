using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HW_6
{
    public class MyIdentityData
    {
        public const string AdminRoleName = "Admin";
        public const string EditorRoleName = "Editor";
        public const string ContributorRoleName = "Contributor";
        public const string TopicAdminRoleName = "TopicAdmin";

        public const string BlogPolicy_Add = "CanAddBlogPosts";
        public const string BlogPolicy_Edit = "CanEditBlogPosts";
        public const string BlogPolicy_Delete = "CanDeleteBlogPosts";

        internal static void SeedData(UserManager<OutdoorUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in new[] { AdminRoleName, EditorRoleName, ContributorRoleName, TopicAdminRoleName })
            {
                var role = roleManager.FindByNameAsync(roleName).Result;
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }
            }

            foreach (var userName in new[] { "admin@snow.edu", "editor@snow.edu", "contributor@snow.edu", "topicadmin@snow.edu" })
            {
                var user = userManager.FindByNameAsync(userName).Result;
                if (user == null)
                {
                    user = new OutdoorUser(userName);
                    user.Email = userName;
                    userManager.CreateAsync(user, "P@ssword1").GetAwaiter().GetResult();
                }
                if (userName.StartsWith("admin"))
                {
                    userManager.AddToRoleAsync(user, AdminRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("editor"))
                {
                    userManager.AddToRoleAsync(user, EditorRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("contributor"))
                {
                    userManager.AddToRoleAsync(user, ContributorRoleName).GetAwaiter().GetResult();
                }
                if (userName.StartsWith("topicadmin"))
                {
                    userManager.AddToRoleAsync(user, TopicAdminRoleName).GetAwaiter().GetResult();
                }
            }
        }
    }
}
