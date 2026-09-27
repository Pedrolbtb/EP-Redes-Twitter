using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;
using epjb.Cliente.Rede;

namespace epjb.View
{
    public partial class Menu : Form
    {
        private int usuarioId;
        private string usuarioUsername;
        private ServicoApi servico;

        public Menu(int id, string username)
        {
            InitializeComponent();
            usuarioId = id;
            usuarioUsername = username;
            servico = new ServicoApi();
        }

        private async void Menu_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = $"Bem-vindo, {usuarioUsername}!";
            await CarregarTimeline();
            await CarregarUsuarios();
            await CarregarMeusSeguindo();
        }

        private async System.Threading.Tasks.Task CarregarTimeline()
        {
            try
            {
                var resposta = await servico.ListarMensagensAsync();

                if (resposta?.Sucesso == true)
                {
                    var mensagens = JsonSerializer.Deserialize<List<dynamic>>(resposta.PayloadJson);

                    dgvMensagens.DataSource = null;
                    dgvMensagens.DataSource = mensagens;
                    dgvMensagens.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar mensagens: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task CarregarUsuarios()
        {
            try
            {
                var resposta = await servico.ListarUsuariosAsync();

                if (resposta?.Sucesso == true)
                {
                    var usuarios = JsonSerializer.Deserialize<List<dynamic>>(resposta.PayloadJson);

                    cmbUsuarios.DataSource = null;
                    cmbUsuarios.DataSource = usuarios;
                    cmbUsuarios.DisplayMember = "username";
                    cmbUsuarios.ValueMember = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task CarregarMeusSeguindo()
        {
            try
            {
                var resposta = await servico.ListarMeusSeguindoAsync(usuarioId);

                if (resposta?.Sucesso == true)
                {
                    var seguindo = JsonSerializer.Deserialize<List<dynamic>>(resposta.PayloadJson);

                    lstSeguindo.DataSource = null;
                    foreach (var user in seguindo)
                    {
                        lstSeguindo.Items.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar seguindo: {ex.Message}");
            }
        }

        private async void btnPostar_Click(object sender, EventArgs e)
        {
            string conteudo = txtConteudo.Text.Trim();

            if (string.IsNullOrEmpty(conteudo))
            {
                MessageBox.Show(this, "Digite uma mensagem antes de postar.");
                return;
            }

            if (conteudo.Length > 500)
            {
                MessageBox.Show(this, "Mensagem não pode ultrapassar 500 caracteres.");
                return;
            }

            try
            {
                var resposta = await servico.PostarMensagemAsync(usuarioId, conteudo);

                if (resposta?.Sucesso == true)
                {
                    MessageBox.Show(this, "Mensagem postada com sucesso!");
                    txtConteudo.Clear();
                    await CarregarTimeline();
                }
                else
                {
                    MessageBox.Show(this, $"Erro: {resposta?.Erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao postar: {ex.Message}");
            }
        }

        private async void btnSeguir_Click(object sender, EventArgs e)
        {
            if (cmbUsuarios.SelectedValue == null)
            {
                MessageBox.Show(this, "Selecione um usuário para seguir.");
                return;
            }

            int idUsuarioASeguir = (int)cmbUsuarios.SelectedValue;

            if (idUsuarioASeguir == usuarioId)
            {
                MessageBox.Show(this, "Você não pode seguir a si mesmo.");
                return;
            }

            try
            {
                var resposta = await servico.SeguirUsuarioAsync(usuarioId, idUsuarioASeguir);

                if (resposta?.Sucesso == true)
                {
                    MessageBox.Show(this, "Usuário seguido com sucesso!");
                    await CarregarMeusSeguindo();
                }
                else
                {
                    MessageBox.Show(this, $"Erro: {resposta?.Erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao seguir: {ex.Message}");
            }
        }

        private async void btnDeletar_Click(object sender, EventArgs e)
        {
            if (dgvMensagens.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Selecione uma mensagem para deletar.");
                return;
            }

            var row = dgvMensagens.SelectedRows[0];
            int idMensagem = (int)row.Cells["id"].Value;
            int idUsuarioMensagem = (int)row.Cells["idUsuario"].Value;

            if (idUsuarioMensagem != usuarioId)
            {
                MessageBox.Show(this, "Você só pode deletar suas próprias mensagens.");
                return;
            }

            var result = MessageBox.Show(this, "Tem certeza que deseja deletar esta mensagem?", "Confirmação", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var resposta = await servico.DeletarMensagemAsync(idMensagem, usuarioId);

                    if (resposta?.Sucesso == true)
                    {
                        MessageBox.Show(this, "Mensagem deletada com sucesso!");
                        await CarregarTimeline();
                    }
                    else
                    {
                        MessageBox.Show(this, $"Erro: {resposta?.Erro}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Erro ao deletar: {ex.Message}");
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            servico?.Dispose();
            this.Close();
        }

        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            await CarregarTimeline();
        }
    }
}
