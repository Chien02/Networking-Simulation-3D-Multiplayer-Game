using System;
using NetworkingSimulation.Core.Network;
using UnityEngine;

namespace NetworkingSimulation.Core.Levels
{
    public sealed class LevelManager
    {
        public LevelManager()
            : this(new NetworkGraph(), new EconomyManager())
        {
        }

        public LevelManager(NetworkGraph networkGraph, EconomyManager economyManager)
        {
            NetworkGraph = networkGraph ?? throw new ArgumentNullException(nameof(networkGraph));
            EconomyManager = economyManager ?? throw new ArgumentNullException(nameof(economyManager));
        }

        public event Action<LevelData> LevelLoaded;
        public event Action<LevelData> LevelStarted;
        public event Action<LevelData> LevelFailed;

        public LevelData CurrentLevel { get; private set; }

        public float TimeRemaining { get; private set; }

        public bool IsLevelActive { get; private set; }

        public NetworkGraph NetworkGraph { get; }

        public EconomyManager EconomyManager { get; }

        public void LoadLevel(LevelData data)
        {
            CurrentLevel = data ?? throw new ArgumentNullException(nameof(data));
            TimeRemaining = Mathf.Max(0f, data.timeLimitInSeconds);
            IsLevelActive = false;

            EconomyManager.SetBudget(Mathf.Max(0f, data.startingBudget));
            NetworkGraph.Clear();

            if (data.fixedNodes != null)
            {
                foreach (FixedNodeData fixedNode in data.fixedNodes)
                {
                    NetworkGraph.AddNode(CreateNode(fixedNode));
                }
            }

            LevelLoaded?.Invoke(CurrentLevel);
        }

        public void StartLevel()
        {
            if (CurrentLevel == null)
            {
                throw new InvalidOperationException("Cannot start a level before loading LevelData.");
            }

            IsLevelActive = true;
            LevelStarted?.Invoke(CurrentLevel);
        }

        public void UpdateTimer(float deltaTime)
        {
            if (!IsLevelActive || deltaTime <= 0f)
            {
                return;
            }

            TimeRemaining = Mathf.Max(0f, TimeRemaining - deltaTime);

            if (TimeRemaining <= 0f)
            {
                IsLevelActive = false;
                LevelFailed?.Invoke(CurrentLevel);
            }
        }

        private static NetworkNode CreateNode(FixedNodeData data)
        {
            NetworkNodeType nodeType = ParseNodeType(data.type);

            switch (nodeType)
            {
                case NetworkNodeType.Router:
                    return new RouterNode(data.id, data.name, data.ipAddress);
                case NetworkNodeType.Switch:
                    return new SwitchNode(data.id, data.name, data.ipAddress);
                case NetworkNodeType.EndDevice:
                default:
                    return new EndDeviceNode(data.id, data.name, data.ipAddress);
            }
        }

        private static NetworkNodeType ParseNodeType(string type)
        {
            if (Enum.TryParse(type, true, out NetworkNodeType parsed))
            {
                return parsed;
            }

            return NetworkNodeType.EndDevice;
        }
    }
}
