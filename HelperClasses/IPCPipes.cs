using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_127.HelperClasses
{
    /// <summary>
    /// This class implements a basic named pipe server, combined with a simple endpoint manager
    /// </summary>
    public class IPCPipeServer
    {
        private int maxThreads;
        private Thread baseServerThread;
        private bool active, hasDefault = false;
        private Dictionary<string, Func<byte[], byte[]>> endpoints = new Dictionary <string, Func<byte[],byte[]>>();
        private PipeSecurity defPipeSecurity;

        private Func<byte[], byte[]> _DefaultHandler;

        /// <summary>
        /// Defines the default handler (whats called for unrecognized inputs)
        /// </summary>
        public Func<byte[], byte[]> DefaultHandler
        {
            set
            {
                hasDefault = true;
                _DefaultHandler = value;
            }
        }

        /// <summary>
        /// Name of the IPC pipe
        /// </summary>
        public string pipeName { get; private set; }

        /// <summary>
        /// Generates an named pipe server
        /// </summary>
        /// <param name="pipeName">Name for the pipe</param>
        /// <param name="maxThreads">Maximum number of server threads</param>
        public IPCPipeServer(string pipeName, int maxThreads = 4)
        {
            this.maxThreads = maxThreads;
            this.pipeName = pipeName;
            defPipeSecurity = new PipeSecurity();
            var id = new System.Security.Principal.SecurityIdentifier(
                System.Security.Principal.WellKnownSidType.AuthenticatedUserSid,
                null);
            defPipeSecurity.SetAccessRule(new PipeAccessRule(id, 
                PipeAccessRights.ReadWrite,
                System.Security.AccessControl.AccessControlType.Allow));

        }

        private void BaseServerThread(object threadInfo)
        {
            while (active)
            {
                NamedPipeServerStream pS;
                try
                {
                    pS = new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxThreads, PipeTransmissionMode.Message,
                        PipeOptions.None, 0, 0, defPipeSecurity);

                }
                catch
                {
                    Thread.Sleep(100);
                    continue;
                }

                pS.WaitForConnection();
                Thread t = new Thread(BaseProcessingThread);
                t.Start(pS);
            }
        }

        /// <summary>
        /// Starts the pipe server
        /// </summary>
        public void run()
        {
            if (active)
            {
                return;
            }
            active = true;
            baseServerThread = new Thread(BaseServerThread);
            baseServerThread.Start();
        }


        /// <summary>
        /// Stops the pipe server
        /// </summary>
        public void stop()
        {
            active = false;
            baseServerThread.Abort();
        }


        private void BaseProcessingThread(object pipeStream)
        {
            var pS = (NamedPipeServerStream)pipeStream;
            //Do stuff
            var input = new List<byte>(65536);
            var buffer = new byte[256];
            int readCount;
            do
            {
                readCount = pS.Read(buffer, 0, 256);
                input.AddRange(buffer.Take(readCount));
            } 
            while (!pS.IsMessageComplete);
            var epEnd = input.FindIndex(a => a == 0);
            string epTarget = null;
            if (epEnd != -1)
            {
                epTarget = Encoding.UTF8.GetString(input.Take(epEnd).ToArray());
            }


            if (epTarget != null && endpoints.ContainsKey(epTarget))
            {
                var ret = endpoints[epTarget](input.Skip(epEnd + 1).ToArray());
                if (ret != null)
                {
                    try
                    {
                        pS.Write(ret, 0, ret.Length);
                    }
                    catch
                    {
                        HelperClasses.Logger.Log("Pipe write failure");
                    }
                }
                
            } 
            else
            {
                if (hasDefault)
                {
                    _DefaultHandler(input.ToArray());
                }
            }
            
            pS.Close();
            pS.Dispose();
        }


        /// <summary>
        /// Adds and enpoint for an pipe call
        /// </summary>
        /// <param name="name">Enpoint name</param>
        /// <param name="f">Endpoint function</param>
        public void registerEndpoint(string name, Func<byte[], byte[]> f)
        {
            endpoints.Add(name, f);
        }

        /// <summary>
        /// Adds and enpoint for an pipe call
        /// </summary>
        /// <param name="name">Enpoint name</param>
        /// <param name="f"></param>
        public void registerEndpoint(string name, Func<byte[]> f)
        {
            endpoints.Add(name, a=>f());
        }
    }

    /// <summary>
    /// This class implements a basic named pipe client
    /// </summary>
    public class IPCPipeClient
    {
        /// <summary>
        /// Name of the IPC pipe
        /// </summary>
        public string pipeName { get; private set; }

        /// <summary>
        /// Generates an named pipe client
        /// </summary>
        /// <param name="pipeName">Name of the pipe to connect to</param>
        public IPCPipeClient(string pipeName)
        {
            this.pipeName = pipeName;
        }

        /// <summary>
        /// Special function to call pipe endpoints
        /// </summary>
        /// <param name="funcname">Enpoint function</param>
        /// <param name="arg">Argument Data</param>
        /// <returns>Response</returns>
        public byte[] call(string funcname, byte[] arg)
        {
            var ConstructedCall =  new List<byte>(Encoding.UTF8.GetBytes(funcname));
            ConstructedCall.Add(0);
            ConstructedCall.AddRange(arg);
            return call(ConstructedCall.ToArray());
        }

        /// <summary>
        /// Sorta Emulates CallNamedPipe (sends data, returns response)
        /// </summary>
        /// <param name="data">Data to sent</param>
        /// <returns>Response</returns>
        public byte[] call(byte[] data)
        {
            var client = new NamedPipeClientStream(pipeName);
            client.Connect(20000);
            client.ReadMode = PipeTransmissionMode.Message;
            client.Write(data, 0, data.Length);
            var ret = new List<byte>(65536);
            var buffer = new byte[256];
            int readCount;
            do
            {
                readCount = client.Read(buffer, 0, 256);
                ret.AddRange(buffer.Take(readCount));
            }
            while (!client.IsMessageComplete);
            client.Dispose();
            return ret.ToArray();
        }

        /// <summary>
        /// Sends data via pipe
        /// </summary>
        /// <param name="data">Data to send</param>
        public void send(byte[] data)
        {
            var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            client.Connect(20000);
            client.Write(data, 0, data.Length);
            client.Dispose();
        }

        /// <summary>
        /// Recieves data from pipe
        /// </summary>
        /// <returns>Data recieved</returns>
        public byte[] recv()
        {
            var client = new NamedPipeClientStream(".", pipeName, PipeDirection.In);
            client.ReadMode = PipeTransmissionMode.Message;
            client.Connect(20000);
            var ret = new List<byte>(65536);
            var buffer = new byte[256];
            int readCount;
            do
            {
                readCount = client.Read(buffer, 0, 256);
                ret.AddRange(buffer.Take(readCount));
            }
            while (!client.IsMessageComplete);
            client.Dispose();
            return ret.ToArray();
        }

    }
}
