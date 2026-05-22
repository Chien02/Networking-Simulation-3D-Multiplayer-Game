using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkingSimulation.Core.Levels
{
    [Serializable]
    public sealed class LevelData
    {
        public string levelId;
        public string title;
        public string description;
        public string objectiveDescription;
        public string sourceVillageIP;
        public string destinationVillageIP;
        public float startingBudget;
        public float timeLimitInSeconds;
        public List<FixedNodeData> fixedNodes = new List<FixedNodeData>();
        public List<ProvidedToolData> providedTools = new List<ProvidedToolData>();
    }

    [Serializable]
    public sealed class FixedNodeData
    {
        public string id;
        public string name;
        public string type;
        public string ipAddress;
        public SerializableVector3Data position;
    }

    [Serializable]
    public sealed class ProvidedToolData
    {
        public string type;
        public int maxQuantity;
    }

    [Serializable]
    public struct SerializableVector3Data
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
