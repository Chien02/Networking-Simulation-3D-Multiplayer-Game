using System.Collections.Generic;

namespace NetworkingSimulation.Core.Network
{
    public sealed class SwitchNode : NetworkNode
    {
        public SwitchNode(string nodeId, string displayName, string ipAddress, int portCount = 8)
            : base(nodeId, displayName, ipAddress, NetworkNodeType.Switch, portCount)
        {
        }

        public Dictionary<string, int> MacAddressTable { get; } = new Dictionary<string, int>();
    }
}
