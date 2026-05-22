namespace NetworkingSimulation.Core.Network
{
    public enum NetworkNodeType
    {
        Router,
        Switch,
        EndDevice
    }

    public enum ConnectResult
    {
        Success,
        MissingNode,
        InvalidPort,
        PortOccupied,
        SelfConnection,
        InvalidLength,
        AlreadyConnected
    }

    public enum DisconnectResult
    {
        Success,
        MissingNode,
        InvalidPort,
        NoConnection
    }

    public enum RouteSimulationStatus
    {
        NotImplemented,
        NoRouteSimulationAvailable
    }
}
