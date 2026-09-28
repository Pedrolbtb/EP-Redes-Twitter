using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace epjb.Common
{
    public static class Protocolo
    {
        public static void Enviar(Socket socket, Mensagem mensagem)
        {
            string json = JsonSerializer.Serialize(mensagem);
            byte[] payload = Encoding.UTF8.GetBytes(json);
            byte[] tamanho = BitConverter.GetBytes(payload.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(tamanho);
            EnviarExato(socket, tamanho);
            EnviarExato(socket, payload);
        }

        public static Mensagem Receber(Socket socket)
        {
            byte[] cabecalho = ReceberExato(socket, 4);
            if (cabecalho == null) return null;
            if (BitConverter.IsLittleEndian) Array.Reverse(cabecalho);
            int tamanho = BitConverter.ToInt32(cabecalho, 0);
            if (tamanho <= 0) return null;
            byte[] payload = ReceberExato(socket, tamanho);
            if (payload == null) return null;
            string json = Encoding.UTF8.GetString(payload);
            return JsonSerializer.Deserialize<Mensagem>(json);
        }

        private static void EnviarExato(Socket socket, byte[] buffer)
        {
            int enviados = 0;
            while (enviados < buffer.Length)
            {
                int quantidade = socket.Send(buffer, enviados, buffer.Length - enviados, SocketFlags.None);
                if (quantidade == 0) throw new IOException("Conexão encerrada durante o envio.");
                enviados += quantidade;
            }
        }

        private static byte[] ReceberExato(Socket socket, int quantidade)
        {
            byte[] buffer = new byte[quantidade];
            int totalLido = 0;
            while (totalLido < quantidade)
            {
                int lido = socket.Receive(buffer, totalLido, quantidade - totalLido, SocketFlags.None);
                if (lido == 0) return null;
                totalLido += lido;
            }
            return buffer;
        }
    }
}
