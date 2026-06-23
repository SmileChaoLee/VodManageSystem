using System;
using System.Collections.Generic;

namespace VodManageSystem.Models.DataModels
{
    public partial class Computer
    {
        public string ComputerId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string SongNo { get; set; } = string.Empty;
    }
}
