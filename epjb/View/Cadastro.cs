using System;
using System.Text.Json;
using System.Windows.Forms;
using epjb.Cliente.Rede;

namespace epjb.View
{
    public partial class Cadastro : Form
    {
        public Cadastro()
        {
            InitializeComponent();
        }

        private async void btnCadastrar_Click(object sender, EventArgs e)
        {
            string username = txtUsuario.Text.Trim();
            string senha = txtSenha.Text;
            string confirmaSenha = txtConfirmaSenha.Text;

            // Validações
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(this, "Username não pode estar vazio.");
                return;
            }

            if (username.Length < 3)
            {
                MessageBox.Show(this, "Username deve ter no mínimo 3 caracteres.");
                return;
            }

            if (string.IsNullOrEmpty(senha))
            {
                MessageBox.Show(this, "Senha não pode estar vazia.");
                return;
            }

            if (senha.Length < 6)
            {
                MessageBox.Show(this, "Senha deve ter no mínimo 6 caracteres.");
                return;
            }

            if (senha != confirmaSenha)
            {
                MessageBox.Show(this, "As senhas não coincidem.");
                return;
            }

            var servico = new ServicoApi();
            try
            {
                btnCadastrar.Enabled = false;
                btnCadastrar.Text = "Cadastrando...";

                var resposta = await servico.CadastroAsync(username, senha);

                if (resposta == null)
                {
                    MessageBox.Show(this, "Sem resposta do servidor.");
                }
                else if (resposta.Sucesso)
                {
                    MessageBox.Show(this, "Cadastro realizado com sucesso! Faça login agora.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
                btnCadastrar.Enabled = true;
                btnCadastrar.Text = "Cadastrar";
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtSenha_TextChanged(object sender, EventArgs e)
        {
            txtSenha.PasswordChar = '*';
        }

        private void txtConfirmaSenha_TextChanged(object sender, EventArgs e)
        {
            txtConfirmaSenha.PasswordChar = '*';
        }
    }
}
