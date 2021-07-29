using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketAndNetCore.Web
{
    public class windowService
    {
        private ConcurrentDictionary<string, WebSocket> _users = new ConcurrentDictionary<string, WebSocket>();
        private System.Diagnostics.Process prc = new System.Diagnostics.Process();
        private bool exist = false;
        public async Task AddUser(WebSocket socket)
        {
            try
            {
                var name = GenerateName();
                var userAddedSuccessfully = _users.TryAdd(name, socket);
                while (!userAddedSuccessfully)
                {
                    name = GenerateName();
                    userAddedSuccessfully = _users.TryAdd(name, socket);
                }
                
                GiveUserTheirName(name, socket).Wait();
                AnnounceNewUser(name).Wait();
                while (socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult socketResponse;
                    var package = new List<byte>();
                    do
                    {
                        socketResponse = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        package.AddRange(new ArraySegment<byte>(buffer, 0, socketResponse.Count));
                    } while (!socketResponse.EndOfMessage);
                    var bufferAsString = System.Text.Encoding.ASCII.GetString(package.ToArray());
                    if (!string.IsNullOrEmpty(bufferAsString))
                    {
                        var changeRequest = ChangeRequest.FromJson(bufferAsString);
                        await HandleChangeRequest(changeRequest);
                    }
                }
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch (Exception ex)
            { //aqui la excepcion
            }
        }

        private string GenerateName()
        {
            var prefix = "El usuario";
            Random ran = new Random();
            var name = prefix + ran.Next(1, 1000);
            while (_users.ContainsKey(name))
            {
                name = prefix + ran.Next(1, 1000);
            }
            return name;
        }

        private async Task SendAll(string message)
        {
            await Send(message, _users.Values.ToArray());
        }

        private async Task Send(string message, params WebSocket[] socketsToSendTo)
        {
            var sockets = socketsToSendTo.Where(s => s.State == WebSocketState.Open);
            foreach (var theSocket in sockets)
            {
                var stringAsBytes = System.Text.Encoding.ASCII.GetBytes(message);
                var byteArraySegment = new ArraySegment<byte>(stringAsBytes, 0, stringAsBytes.Length);
                await theSocket.SendAsync(byteArraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task GiveUserTheirName(string name, WebSocket socket)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "name",
                Payload = name
            };
            await Send(message.ToJson(), socket);
        }

        private async Task AnnounceNewUser(string name)
        {
            var message = new SocketMessage<string>
            {
                MessageType = "announce",
                Payload = $"{name} se ha unido"
            };
            await SendAll(message.ToJson());
        }

        private async Task AnnounceChange(ChangeRequest request)
        {
            var comp = string.Empty;
            comp = "no";
            if (request.Close==true)
            {
                comp = $"{request.Name} detuvo la aplicacion";
            }
            var message = new SocketMessage<string>
            {
                MessageType = "announce",
                Payload = comp,
                X = request.X,
                Y = request.Y,
                He = request.Height,
                Wi = request.Width
            };
            await SendAll(message.ToJson());
        }

        private async Task HandleChangeRequest(ChangeRequest request)
        { 
            if (exist == false ||  prc.HasExited)
            {
                prc = System.Diagnostics.Process.Start("notepad.exe");
                exist = true;
            }
            await AnnounceChange(request);
            if(request.Close==true && exist)
            {
                prc.Kill();
                exist = false;
                prc = new System.Diagnostics.Process();
            }
            else
            {
                prc.WaitForInputIdle();
                bool ok = MoveWindow(prc.MainWindowHandle, request.X, request.Y, request.Height, request.Width, true);
                if (!ok) throw new System.ComponentModel.Win32Exception();
            }
            

        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

    }
}
