namespace NetworkingSimulation.Core.Network
{
    public sealed class Packet
    {
        public Packet(string sourceIP, string destinationIP, string payload = null)
        {
            SourceIP = sourceIP;
            DestinationIP = destinationIP;
            Payload = payload;
        }

        public string SourceIP { get; }

        public string DestinationIP { get; }

        public string Payload { get; }
    }
}
