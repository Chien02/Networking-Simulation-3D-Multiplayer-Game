using System.Collections.Generic;
using System.Linq;

namespace NetworkingSimulation.Core.Network
{
    public sealed class NetworkGraph
    {
        private readonly Dictionary<string, NetworkNode> nodes = new Dictionary<string, NetworkNode>();
        private readonly List<NetworkEdge> edges = new List<NetworkEdge>();

        public IReadOnlyCollection<NetworkNode> Nodes => nodes.Values;

        public IReadOnlyCollection<NetworkEdge> Edges => edges;

        public bool AddNode(NetworkNode node)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.NodeId) || nodes.ContainsKey(node.NodeId))
            {
                return false;
            }

            nodes.Add(node.NodeId, node);
            return true;
        }

        public bool TryGetNode(string nodeId, out NetworkNode node)
        {
            return nodes.TryGetValue(nodeId, out node);
        }

        public bool RemoveNode(string nodeId)
        {
            if (!nodes.ContainsKey(nodeId))
            {
                return false;
            }

            NetworkEdge[] attachedEdges = edges.Where(edge => edge.NodeAId == nodeId || edge.NodeBId == nodeId).ToArray();
            foreach (NetworkEdge edge in attachedEdges)
            {
                edges.Remove(edge);
                ReleasePort(edge.NodeAId, edge.PortAId);
                ReleasePort(edge.NodeBId, edge.PortBId);
            }

            nodes.Remove(nodeId);
            return true;
        }

        public void Clear()
        {
            foreach (NetworkEdge edge in edges.ToArray())
            {
                ReleasePort(edge.NodeAId, edge.PortAId);
                ReleasePort(edge.NodeBId, edge.PortBId);
            }

            edges.Clear();
            nodes.Clear();
        }

        public ConnectResult ConnectNodes(string nodeAId, int portAId, string nodeBId, int portBId, float length)
        {
            if (length < 0f)
            {
                return ConnectResult.InvalidLength;
            }

            if (nodeAId == nodeBId)
            {
                return ConnectResult.SelfConnection;
            }

            if (!nodes.TryGetValue(nodeAId, out NetworkNode nodeA) || !nodes.TryGetValue(nodeBId, out NetworkNode nodeB))
            {
                return ConnectResult.MissingNode;
            }

            if (!nodeA.TryGetPort(portAId, out NetworkPort portA) || !nodeB.TryGetPort(portBId, out NetworkPort portB))
            {
                return ConnectResult.InvalidPort;
            }

            if (edges.Any(edge => edge.Connects(nodeAId, portAId, nodeBId, portBId)))
            {
                return ConnectResult.AlreadyConnected;
            }

            if (portA.IsOccupied || portB.IsOccupied)
            {
                return ConnectResult.PortOccupied;
            }

            NetworkEdge edge = new NetworkEdge(nodeAId, portAId, nodeBId, portBId, length);
            edges.Add(edge);
            portA.Bind(nodeBId, portBId);
            portB.Bind(nodeAId, portAId);

            return ConnectResult.Success;
        }

        public DisconnectResult DisconnectPort(string nodeId, int portId)
        {
            if (!nodes.TryGetValue(nodeId, out NetworkNode node))
            {
                return DisconnectResult.MissingNode;
            }

            if (!node.TryGetPort(portId, out NetworkPort port))
            {
                return DisconnectResult.InvalidPort;
            }

            NetworkEdge edge = edges.FirstOrDefault(candidate => candidate.Contains(nodeId, portId));
            if (edge == null)
            {
                return DisconnectResult.NoConnection;
            }

            edges.Remove(edge);
            ReleasePort(edge.NodeAId, edge.PortAId);
            ReleasePort(edge.NodeBId, edge.PortBId);

            return DisconnectResult.Success;
        }

        public RouteSimulationResult SimulateRouting(Packet packet)
        {
            return RouteSimulationResult.NotImplemented(packet);
        }

        private void ReleasePort(string nodeId, int portId)
        {
            if (nodes.TryGetValue(nodeId, out NetworkNode node) && node.TryGetPort(portId, out NetworkPort port))
            {
                port.Release();
            }
        }
    }
}
