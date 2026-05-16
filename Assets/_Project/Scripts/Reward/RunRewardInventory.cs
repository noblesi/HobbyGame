using UnityEngine;

namespace TopDownRoguelite.Reward
{
    public class RunRewardInventory : MonoBehaviour
    {
        public int Gold { get; private set; }
        public int Potions { get; private set; }
        public int Artifacts { get; private set; }
        public int Tablets { get; private set; }
        public int AttackModules { get; private set; }

        public void AddReward(RewardOption rewardOption)
        {
            if (rewardOption == null)
            {
                return;
            }

            int amount = Mathf.Max(1, rewardOption.Amount);

            switch (rewardOption.RewardType)
            {
                case RewardType.Gold:
                    Gold += amount;
                    break;
                case RewardType.Potion:
                    Potions += amount;
                    break;
                case RewardType.Artifact:
                    Artifacts += amount;
                    break;
                case RewardType.Tablet:
                    Tablets += amount;
                    break;
                case RewardType.AttackModule:
                    AttackModules += amount;
                    break;
            }

            Debug.Log(BuildDebugSummary(), this);
        }

        public string BuildDebugSummary()
        {
            return $"Run rewards - Gold: {Gold}, Potions: {Potions}, Artifacts: {Artifacts}, Tablets: {Tablets}, Attack Modules: {AttackModules}";
        }
    }
}
