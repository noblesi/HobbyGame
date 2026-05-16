using System;

namespace TopDownRoguelite.Reward
{
    [Serializable]
    public class RewardOption
    {
        public RewardOption(RewardType rewardType, string title, string description, int amount)
        {
            RewardType = rewardType;
            Title = title;
            Description = description;
            Amount = amount;
        }

        public RewardType RewardType { get; }
        public string Title { get; }
        public string Description { get; }
        public int Amount { get; }
        public string DisplayText => $"{Title}\n{Description}";
    }
}
