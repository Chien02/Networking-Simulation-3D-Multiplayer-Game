namespace NetworkingSimulation.Core.Network
{
    public sealed class RouteSimulationResult
    {
        private RouteSimulationResult(RouteSimulationStatus status, Packet packet)
        {
            Status = status;
            Packet = packet;
        }

        public RouteSimulationStatus Status { get; }

        public Packet Packet { get; }

        public static RouteSimulationResult NotImplemented(Packet packet)
        {
            return new RouteSimulationResult(RouteSimulationStatus.NotImplemented, packet);
        }
    }
}
