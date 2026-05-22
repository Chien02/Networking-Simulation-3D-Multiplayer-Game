namespace NetworkingSimulation.Core.Network
{
    public sealed class NetworkPort
    {
        public NetworkPort(int portId)
        {
            PortId = portId;
        }

        public int PortId { get; }

        public bool IsOccupied { get; private set; }

        public string ConnectedNodeId { get; private set; }

        public int? ConnectedPortId { get; private set; }

        public void Bind(string nodeId, int portId)
        {
            IsOccupied = true;
            ConnectedNodeId = nodeId;
            ConnectedPortId = portId;
        }

        public void Release()
        {
            IsOccupied = false;
            ConnectedNodeId = null;
            ConnectedPortId = null;
        }
    }
}
