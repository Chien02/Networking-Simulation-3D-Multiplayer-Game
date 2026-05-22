using System.Collections.Generic;

namespace NetworkingSimulation.Core.Network
{
    public sealed class RouterNode : NetworkNode
    {
        public RouterNode(string nodeId, string displayName, string ipAddress, int portCount = 4)
            : base(nodeId, displayName, ipAddress, NetworkNodeType.Router, portCount)
        {
        }

        public Dictionary<string, int> RoutingTable { get; } = new Dictionary<string, int>();
    }
}
