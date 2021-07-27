using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketAndNetCore.Web
{
    public class windowApp
    {
        public int Id { get; set; }
        public string Color { get; set; }

        public static IEnumerable<windowApp> GetInitialSquares()
        {
            var colors = new string[] { "red", "green", "blue" };
            var squares = new List<windowApp>();
            for (int i = 0; i < 1; i++)
            {
                var random = new Random();
                squares.Add(new windowApp()
                {
                    Id = i,
                    Color = colors[(random.Next(1, 3)) - 1]
                });
            }
            return squares;
        }
    }

    public class ChangeRequest
    {
        //public int Id { get; set; }
        public bool Close { get; set; }
        public string Name { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public static ChangeRequest FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ChangeRequest>(json);
        }
    }
}
