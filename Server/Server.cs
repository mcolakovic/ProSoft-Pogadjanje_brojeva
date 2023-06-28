using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Common;

namespace Server
{
    public class Server
    {
        private Socket serverSocket;
        private bool isRunning = false;
        private List<ClientHandler> clients = new List<ClientHandler>();
        public List<ClientHandler> Clients { get => clients; }
        private List<int> brojevi = new List<int>();
        public List<int> Brojevi { get => brojevi; set => brojevi = value; }
        private static Object zakljucaj = new Object();

        public void Start()
        {
            if (!isRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
                serverSocket.Listen(5);
                isRunning = true;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                serverSocket.Dispose();
                serverSocket = null;
                isRunning = false;
            }
        }

        public void HandleClients()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    ClientHandler handler = new ClientHandler(clientSocket, brojevi);
                    clients.Add(handler);
                    handler.OdjavljenKorisnik += Handler_OdjavljenKorisnik;
                    handler.ObrisiBroj += Handler_ObrisiBroj;
                    Thread klijentskaNit = new Thread(handler.HandleRequests);
                    klijentskaNit.IsBackground = true;
                    klijentskaNit.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Handler_ObrisiBroj(object sender, EventArgs e)
        {
            int? odg = ObrisiBrojIzListe();
            
            if(odg != null)
            {
                Poruka poruka = new Poruka
                {
                    IsSuccessful = true,
                    Operations = Operations.Igra,
                    PorukaObject = new Info
                    {
                        Broj = (int)odg
                    }
                };
                ((ClientHandler)sender).Helper.Send(poruka);
            }
            else
            {
                Poruka poruka = new Poruka
                {
                    IsSuccessful = true,
                    Operations = Operations.ZavrsiIgru
                };
                ((ClientHandler)sender).Helper.Send(poruka);
            }
        }

        private int? ObrisiBrojIzListe()
        {
            int number;
            if(brojevi.Count != 0)
            {
                lock (zakljucaj)
                {
                    number = brojevi[0];
                    brojevi.RemoveAt(0);
                }
                return number;
            }
            return null;
        }

        private void Handler_OdjavljenKorisnik(object sender, EventArgs e)
        {
            clients.Remove((ClientHandler)sender);
        }
    }
}
