using System;
using System.Collections.Generic;

namespace VodManageSystem.Models.DataModels
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserState { get; set; } = string.Empty;
    }
}
