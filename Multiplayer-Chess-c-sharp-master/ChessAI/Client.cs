using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChessClient.Net;

namespace ChessAI {
    /// <summary>
    /// A client to communicate with the server
    /// </summary>
    public class Client {
        const int PacketSendAmount = 5;
        /// <summary>
        /// Used as an unique identifier for sent packets
        /// </summary>
        private byte GlobalPacketId { get; set; }
        /// <summary>
        /// The port of the server
        /// </summary>
        const int ServerPort = 64425;
        /// <summary>
        /// The port the server will use to connect to this client
        /// </summary>
        readonly ushort _localPort = 64426;
        /// <summary>
        /// The name of the user
        /// </summary>
        public string ClientName { get; private set; }
        /// <summary>
        /// Socket to receive packets from the server
        /// </summary>
        private readonly Socket _listener;
        /// <summary>
        /// Disposes the socket to stop listening
        /// </summary>
        public void DisposeSocket() {
            _listener.Close();
        }
        /// <summary>
        /// To store the board object when returning the packet
        /// </summary>
        private byte[] _referenceGamePacket;
        /// <summary>
        /// To send packets to the server
        /// </summary>
        readonly UdpClient _sender = new UdpClient();
        /// <summary>
        /// The List of Packets and corresponding bytes that the server has returned from a message
        /// </summary>
        private readonly List<Tuple<Packet, byte[]>> _returns = new List<Tuple<Packet, byte[]>>(40);
        /// <summary>
        /// The list of Packets that the Clients what the server to respond to
        /// </summary>
        private readonly List<Packet> _wantedReturns = new List<Packet>(40);
        /// <summary>
        /// Is the client running?
        /// </summary>
        public bool IsRunning = true;
        /// <summary>
        /// History of received packets
        /// </summary>
        private readonly Queue<Packet> _history = new Queue<Packet>(20);

        //public ChessGame CurrentGame;
        public Client(IPAddress ip, string name, byte startingPacketNo) {
            ClientName = name;
            _listener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            // Connect to the server
            _sender.Connect(new IPEndPoint(ip, ServerPort));
            // Bind listener to a port to receive Packets from the server 
            var foundPort = false;
            while (!foundPort) {
                try {
                    _listener.Bind(new IPEndPoint(IPAddress.Any, _localPort));
                    foundPort = true;
                } catch {
                    // Port already in use, try next port.
                    _localPort += 1;
                }
            }

            GlobalPacketId = startingPacketNo;

            // Start a new thread to listen for packets
            var clientThread = new Thread(ReceiveThread) {
                Name = "Receive Thread",
                Priority = ThreadPriority.AboveNormal
            };
            clientThread.Start();
        }

        /// <summary>
        /// While running constantly loops to listen for packets
        /// </summary>
        private void ReceiveThread() {
            while (IsRunning) {
                var received = new byte[4096];
                // Receive a message from the server
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                try {
                    _listener.ReceiveFrom(received, ref sender);
                } catch (Exception) {
                    // ignored
                }
                // Deal with the message
                InterpretRequest(received);
            }

            // Send a disconnect packet to the server
            var dissconectPacket = new Packet(GlobalPacketId++, _localPort, Packet.Packets.Disconnect, ClientName);
            _sender.Send(dissconectPacket.GetData(), dissconectPacket.GetData().Length);
            _sender.Close();
        }

        /// <summary>
        /// Cause client updates based on received data
        /// </summary>
        /// <param name="data">The data that was received</param>
   
