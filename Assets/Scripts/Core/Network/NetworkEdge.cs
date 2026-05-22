namespace NetworkingSimulation.Core.Network
{
    public sealed class NetworkEdge
    {
        public NetworkEdge(string nodeAId, int portAId, string nodeBId, int portBId, float length)
        {
            NodeAId = nodeAId;
            PortAId = portAId;
            NodeBId = nodeBId;
            PortBId = portBId;
            Length = length;
        }

        public string NodeAId { get; }

        public int PortAId { get; }

        public string NodeBId { get; }

        public int PortBId { get; }

        public float Length { get; }

        public bool Contains(string nodeId, int portId)
        {
            return (NodeAId == nodeId && PortAId == portId)
                || (NodeBId == nodeId && PortBId == portId);
        }

        public bool Connects(string nodeAId, int portAId, string nodeBId, int portBId)
        {
            return (NodeAId == nodeAId && PortAId == portAId && NodeBId == nodeBId && PortBId == portBId)
                || (NodeAId == nodeBId && PortAId == portBId && NodeBId == nodeAId && PortBId == portAId);
        }
    }
}
