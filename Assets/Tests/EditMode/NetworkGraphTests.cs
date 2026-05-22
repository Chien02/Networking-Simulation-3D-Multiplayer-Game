using System.Linq;
using NetworkingSimulation.Core.Network;
using NUnit.Framework;

namespace NetworkingSimulation.Tests
{
    public sealed class NetworkGraphTests
    {
        [Test]
        public void AddNodeRejectsDuplicateNodeId()
        {
            NetworkGraph graph = new NetworkGraph();

            Assert.IsTrue(graph.AddNode(new RouterNode("router_a", "Router A", "10.0.0.1")));
            Assert.IsFalse(graph.AddNode(new RouterNode("router_a", "Router Duplicate", "10.0.0.2")));
            Assert.AreEqual(1, graph.Nodes.Count);
        }

        [Test]
        public void ConnectNodesCreatesBidirectionalEdgeAndOccupiesPorts()
        {
            NetworkGraph graph = CreateGraphWithRouterAndDevice();

            ConnectResult result = graph.ConnectNodes("router", 0, "device", 0, 12.5f);

            Assert.AreEqual(ConnectResult.Success, result);
            Assert.AreEqual(1, graph.Edges.Count);
            Assert.IsTrue(graph.TryGetNode("router", out NetworkNode router));
            Assert.IsTrue(graph.TryGetNode("device", out NetworkNode device));
            Assert.IsTrue(router.LogicPorts[0].IsOccupied);
            Assert.AreEqual("device", router.LogicPorts[0].ConnectedNodeId);
            Assert.IsTrue(device.LogicPorts[0].IsOccupied);
            Assert.AreEqual("router", device.LogicPorts[0].ConnectedNodeId);
            Assert.AreEqual(12.5f, graph.Edges.First().Length);
        }

        [Test]
        public void ConnectNodesValidatesMissingNodeInvalidPortSelfConnectionLengthAndOccupiedPorts()
        {
            NetworkGraph graph = CreateGraphWithRouterAndDevice();

            Assert.AreEqual(ConnectResult.MissingNode, graph.ConnectNodes("router", 0, "missing", 0, 1f));
            Assert.AreEqual(ConnectResult.InvalidPort, graph.ConnectNodes("router", 99, "device", 0, 1f));
            Assert.AreEqual(ConnectResult.SelfConnection, graph.ConnectNodes("router", 0, "router", 1, 1f));
            Assert.AreEqual(ConnectResult.InvalidLength, graph.ConnectNodes("router", 0, "device", 0, -1f));
            Assert.AreEqual(ConnectResult.Success, graph.ConnectNodes("router", 0, "device", 0, 1f));
            Assert.AreEqual(ConnectResult.PortOccupied, graph.ConnectNodes("router", 1, "device", 0, 1f));
        }

        [Test]
        public void DisconnectPortRemovesEdgeAndReleasesBothPorts()
        {
            NetworkGraph graph = CreateGraphWithRouterAndDevice();
            graph.ConnectNodes("router", 0, "device", 0, 1f);

            DisconnectResult result = graph.DisconnectPort("router", 0);

            Assert.AreEqual(DisconnectResult.Success, result);
            Assert.AreEqual(0, graph.Edges.Count);
            graph.TryGetNode("router", out NetworkNode router);
            graph.TryGetNode("device", out NetworkNode device);
            Assert.IsFalse(router.LogicPorts[0].IsOccupied);
            Assert.IsFalse(device.LogicPorts[0].IsOccupied);
            Assert.IsNull(router.LogicPorts[0].ConnectedNodeId);
            Assert.IsNull(device.LogicPorts[0].ConnectedNodeId);
        }

        [Test]
        public void DisconnectPortValidatesMissingNodeInvalidPortAndNoConnection()
        {
            NetworkGraph graph = CreateGraphWithRouterAndDevice();

            Assert.AreEqual(DisconnectResult.MissingNode, graph.DisconnectPort("missing", 0));
            Assert.AreEqual(DisconnectResult.InvalidPort, graph.DisconnectPort("router", 99));
            Assert.AreEqual(DisconnectResult.NoConnection, graph.DisconnectPort("router", 0));
        }

        [Test]
        public void RemoveNodeRemovesAttachedEdgesAndReleasesRemainingPorts()
        {
            NetworkGraph graph = CreateGraphWithRouterAndDevice();
            graph.ConnectNodes("router", 0, "device", 0, 1f);

            Assert.IsTrue(graph.RemoveNode("device"));

            Assert.IsFalse(graph.TryGetNode("device", out _));
            Assert.AreEqual(0, graph.Edges.Count);
            graph.TryGetNode("router", out NetworkNode router);
            Assert.IsFalse(router.LogicPorts[0].IsOccupied);
            Assert.IsFalse(graph.RemoveNode("missing"));
        }

        private static NetworkGraph CreateGraphWithRouterAndDevice()
        {
            NetworkGraph graph = new NetworkGraph();
            graph.AddNode(new RouterNode("router", "Router", "10.0.0.1"));
            graph.AddNode(new EndDeviceNode("device", "Device", "10.0.0.10"));
            return graph;
        }
    }
}
