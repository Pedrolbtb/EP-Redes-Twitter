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
            // Temporário: iniciar o servidor neste processo para testes locais.
            // Remover ou revisar quando o grupo decidir como o servidor será executado (processo separado conforme roteiro).
            System.Threading.Tasks.Task.Run(() => AsyncSocketListener.StartListener());

            ApplicationConfiguration.Initialize();
            Application.Run(new View.Login());
        }
    }
}