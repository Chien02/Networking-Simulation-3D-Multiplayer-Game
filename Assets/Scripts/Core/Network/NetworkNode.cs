using System.Collections.Generic;

namespace NetworkingSimulation.Core.Network
{
    public abstract class NetworkNode
    {
        private readonly List<NetworkPort> logicPorts;

        protected NetworkNode(string nodeId, string displayName, string ipAddress, NetworkNodeType nodeType, int portCount)
        {
            NodeId = nodeId;
            DisplayName = displayName;
            IPAddress = ipAddress;
            NodeType = nodeType;
            logicPorts = CreatePorts(portCount);
        }

        public string NodeId { get; }

        public string DisplayName { get; }

        public string IPAddress { get; }

        public NetworkNodeType NodeType { get; }

        public IReadOnlyList<NetworkPort> LogicPorts => logicPorts;

        public bool TryGetPort(int portId, out NetworkPort port)
        {
            if (portId >= 0 && portId < logicPorts.Count)
            {
                port = logicPorts[portId];
                return true;
            }

            port = null;
            return false;
        }

        private static List<NetworkPort> CreatePorts(int portCount)
        {
            int count = portCount < 0 ? 0 : portCount;
            List<NetworkPort> ports = new List<NetworkPort>(count);

            for (int i = 0; i < count; i++)
            {
                ports.Add(new NetworkPort(i));
            }

            return ports;
        }
    }
}
