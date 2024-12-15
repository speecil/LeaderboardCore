using System;
using System.Collections.Generic;
using Zenject;
using LeaderboardCore.Interfaces;

namespace LeaderboardCore.Managers
{
    internal class LeaderboardCoreManager
    {
        private readonly List<INotifyLeaderboardLoad> notifyLeaderboardLoads;

        public LeaderboardCoreManager(List<INotifyLeaderboardLoad> notifyLeaderboardLoads)
        {
            this.notifyLeaderboardLoads = notifyLeaderboardLoads;
        }
    }
}
