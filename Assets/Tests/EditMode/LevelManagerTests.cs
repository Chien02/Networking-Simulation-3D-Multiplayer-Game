using System.Collections.Generic;
using NetworkingSimulation.Core.Levels;
using NetworkingSimulation.Core.Network;
using NUnit.Framework;
using UnityEngine;

namespace NetworkingSimulation.Tests
{
    public sealed class LevelManagerTests
    {
        [Test]
        public void LoadLevelCreatesFixedNodesBudgetAndTimer()
        {
            LevelData level = CreateLevelData();
            LevelManager levelManager = new LevelManager();
            bool loadedEventRaised = false;
            levelManager.LevelLoaded += loaded => loadedEventRaised = loaded == level;

            levelManager.LoadLevel(level);

            Assert.IsTrue(loadedEventRaised);
            Assert.AreSame(level, levelManager.CurrentLevel);
            Assert.AreEqual(300f, levelManager.TimeRemaining);
            Assert.AreEqual(1200f, levelManager.EconomyManager.CurrentBudget);
            Assert.IsFalse(levelManager.IsLevelActive);
            Assert.AreEqual(2, levelManager.NetworkGraph.Nodes.Count);
            Assert.IsTrue(levelManager.NetworkGraph.TryGetNode("node_village_a", out NetworkNode nodeA));
            Assert.AreEqual(NetworkNodeType.EndDevice, nodeA.NodeType);
            Assert.AreEqual(1, nodeA.LogicPorts.Count);
            Assert.IsTrue(levelManager.NetworkGraph.TryGetNode("node_router", out NetworkNode router));
            Assert.AreEqual(NetworkNodeType.Router, router.NodeType);
            Assert.AreEqual(4, router.LogicPorts.Count);
        }

        [Test]
        public void StartAndUpdateTimerFailLevelWhenTimeRunsOut()
        {
            LevelManager levelManager = new LevelManager();
            levelManager.LoadLevel(CreateLevelData());
            bool startedEventRaised = false;
            bool failedEventRaised = false;
            levelManager.LevelStarted += _ => startedEventRaised = true;
            levelManager.LevelFailed += _ => failedEventRaised = true;

            levelManager.StartLevel();
            levelManager.UpdateTimer(299f);

            Assert.IsTrue(startedEventRaised);
            Assert.IsTrue(levelManager.IsLevelActive);
            Assert.AreEqual(1f, levelManager.TimeRemaining);
            Assert.IsFalse(failedEventRaised);

            levelManager.UpdateTimer(2f);

            Assert.IsTrue(failedEventRaised);
            Assert.IsFalse(levelManager.IsLevelActive);
            Assert.AreEqual(0f, levelManager.TimeRemaining);
        }

        [Test]
        public void LoadLevelClearsPreviousGraphState()
        {
            LevelManager levelManager = new LevelManager();
            levelManager.LoadLevel(CreateLevelData());
            levelManager.LoadLevel(CreateLevelData());

            Assert.AreEqual(2, levelManager.NetworkGraph.Nodes.Count);
        }

        [Test]
        public void JsonUtilityCanDeserializeLevelData()
        {
            string json = @"{
                ""levelId"": ""json_level"",
                ""title"": ""JSON Level"",
                ""description"": ""Loaded from JSON"",
                ""objectiveDescription"": ""Connect villages"",
                ""sourceVillageIP"": ""10.0.0.10"",
                ""destinationVillageIP"": ""10.0.0.20"",
                ""startingBudget"": 500.0,
                ""timeLimitInSeconds"": 60.0,
                ""fixedNodes"": [
                    {
                        ""id"": ""node_village_a"",
                        ""name"": ""Village A"",
                        ""type"": ""EndDevice"",
                        ""ipAddress"": ""10.0.0.10"",
                        ""position"": { ""x"": 0.0, ""y"": 0.0, ""z"": 10.0 }
                    }
                ],
                ""providedTools"": [
                    { ""type"": ""Router"", ""maxQuantity"": 2 }
                ]
            }";

            LevelData level = JsonUtility.FromJson<LevelData>(json);

            Assert.AreEqual("json_level", level.levelId);
            Assert.AreEqual(1, level.fixedNodes.Count);
            Assert.AreEqual("node_village_a", level.fixedNodes[0].id);
            Assert.AreEqual(new Vector3(0f, 0f, 10f), level.fixedNodes[0].position.ToVector3());
            Assert.AreEqual(2, level.providedTools[0].maxQuantity);
        }

        private static LevelData CreateLevelData()
        {
            return new LevelData
            {
                levelId = "level_test",
                title = "Test Level",
                description = "Test Description",
                objectiveDescription = "Test Objective",
                sourceVillageIP = "10.0.0.10",
                destinationVillageIP = "10.0.0.20",
                startingBudget = 1200f,
                timeLimitInSeconds = 300f,
                fixedNodes = new List<FixedNodeData>
                {
                    new FixedNodeData
                    {
                        id = "node_village_a",
                        name = "Village A",
                        type = "EndDevice",
                        ipAddress = "10.0.0.10",
                        position = new SerializableVector3Data(0f, 0f, 10f)
                    },
                    new FixedNodeData
                    {
                        id = "node_router",
                        name = "Router",
                        type = "Router",
                        ipAddress = "10.0.0.1",
                        position = new SerializableVector3Data(5f, 0f, 5f)
                    }
                },
                providedTools = new List<ProvidedToolData>
                {
                    new ProvidedToolData { type = "Switch", maxQuantity = 1 }
                }
            };
        }
    }
}
