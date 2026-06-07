using System;
using System.Collections.Generic;
using TopDownRoguelite.Artifact;
using UnityEngine;

namespace TopDownRoguelite.Reward
{
    public class RunRewardInventory : MonoBehaviour
    {
        private readonly List<ArtifactInstance> artifactInstances = new List<ArtifactInstance>();
        private int nextArtifactInstanceId = 1;

        public int Gold { get; private set; }
        public int Potions { get; private set; }
        public int Artifacts { get; private set; }
        public int Tablets { get; private set; }
        public int AttackModules { get; private set; }
        public IReadOnlyList<ArtifactInstance> ArtifactInstances => artifactInstances;

        public event Action<ArtifactInstance> ArtifactAdded;

        public ArtifactInstance AddReward(RewardOption rewardOption)
        {
            if (rewardOption == null)
            {
                return null;
            }

            int amount = Mathf.Max(1, rewardOption.Amount);
            ArtifactInstance lastCreatedArtifact = null;

            switch (rewardOption.RewardType)
            {
                case RewardType.Gold:
                    Gold += amount;
                    break;
                case RewardType.Potion:
                    Potions += amount;
                    break;
                case RewardType.Artifact:
                    for (int i = 0; i < amount; i++)
                    {
                        lastCreatedArtifact = CreateArtifactReward();
                        artifactInstances.Add(lastCreatedArtifact);
                        ArtifactAdded?.Invoke(lastCreatedArtifact);
                    }

                    Artifacts = artifactInstances.Count;
                    break;
                case RewardType.Tablet:
                    Tablets += amount;
                    break;
                case RewardType.AttackModule:
                    AttackModules += amount;
                    break;
            }

            Debug.Log(BuildDebugSummary(), this);
            return lastCreatedArtifact;
        }

        public string BuildDebugSummary()
        {
            return $"Run rewards - Gold: {Gold}, Potions: {Potions}, Artifacts: {Artifacts}, Tablets: {Tablets}, Attack Modules: {AttackModules}";
        }

        private ArtifactInstance CreateArtifactReward()
        {
            int artifactId = nextArtifactInstanceId++;
            ArtifactShape shape = CreateRewardShape(artifactId);
            return new ArtifactInstance($"reward-artifact-{artifactId}", $"Reward Relic {artifactId}", shape);
        }

        private ArtifactShape CreateRewardShape(int artifactId)
        {
            switch (artifactId % 3)
            {
                case 1:
                    return ArtifactShape.Rectangle(2, 1);
                case 2:
                    return ArtifactShape.LShape();
                default:
                    return ArtifactShape.Rectangle(1, 2);
            }
        }
    }
}
