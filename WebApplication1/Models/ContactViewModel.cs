using System;
namespace DapperDashboard.ViewModels
{
    public class ContactViewModel
    {
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string AvatarUrl => $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(ContactName)}&background=random";
    }
}