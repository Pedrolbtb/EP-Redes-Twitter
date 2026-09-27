using System;
using System.Text.Json;
using System.Windows.Forms;
using epjb.Cliente.Rede;

namespace epjb.View
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsuario.Text.Trim();
            string password = txtSenha.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(this, "Preencha username e senha.");
                return;
            }

            var servico = new ServicoApi();
            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Autenticando...";

                var resposta = await servico.LoginAsync(username, password);

                if (resposta == null)
                {
                    MessageBox.Show(this, "Sem resposta do servidor.");
                }
                else if (resposta.Sucesso)
                {
                    // Parse response para obter ID do usuário
                    var userData = JsonSerializer.Deserialize<JsonElement>(resposta.PayloadJson);
                    int usuarioId = userData.GetProperty("id").GetInt32();
                    string usuarioUsername = userData.GetProperty("username").GetString();

                    // Abre tela Menu
                    Menu menu = new Menu(usuarioId, usuarioUsername);
                    this.Hide();
                    menu.ShowDialog();
                    this.Show();

                    // Limpa campos
                    txtUsuario.Clear();
                    txtSenha.Clear();
                }
                else
                {
                    MessageBox.Show(this, $"Erro: {resposta.Erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao conectar: {ex.Message}");
            }
            finally
            {
                servico?.Dispose();
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            var cadastro = new Cadastro();
            var result = cadastro.ShowDialog();

            if (result == DialogResult.OK)
            {
                MessageBox.Show(this, "Cadastro concluído! Faça login com suas credenciais.");
            }
        }
    }
}
