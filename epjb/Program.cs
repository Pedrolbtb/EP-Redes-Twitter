using epjb.Sockets;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace epjb
{
    class Program
    {
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new View.Login());
        }
    }
}