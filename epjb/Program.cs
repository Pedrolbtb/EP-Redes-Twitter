namespace epjb
{
    internal static class Program
    {
        // Windows Forms exige uma thread STA para os controles da interface.
        [STAThread]
        static void Main()
        {
            // O cliente só abre telas: o servidor precisa ser iniciado separadamente, uma única vez.
            ApplicationConfiguration.Initialize();
            Application.Run(new View.Login());
        }
    }
}
