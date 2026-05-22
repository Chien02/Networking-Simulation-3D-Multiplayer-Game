using System;

namespace NetworkingSimulation.Core.Network
{
    public sealed class EndDeviceNode : NetworkNode
    {
        public EndDeviceNode(string nodeId, string displayName, string ipAddress, int portCount = 1)
            : base(nodeId, displayName, ipAddress, NetworkNodeType.EndDevice, portCount)
        {
        }

        public event Action<Packet> PacketReceivedSuccessfully;

        public void NotifyPacketReceived(Packet packet)
        {
            PacketReceivedSuccessfully?.Invoke(packet);
        }
    }
}
