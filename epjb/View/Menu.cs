using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;
using epjb.Cliente.Rede;
using epjb.Common.DTO;

namespace epjb.View
{
    public partial class Menu : Form
    {
        private int usuarioId;
        private string usuarioUsername;
        private ServicoApi servico;

        // Recebe a identidade retornada pelo login e prepara as chamadas ao servidor.
        public Menu(int id, string username)
        {
            InitializeComponent();
            usuarioId = id;
            usuarioUsername = username;
            servico = new ServicoApi();
        }

        // Carrega feed e relacionamentos quando o formulário fica pronto.
        private async void Menu_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = $"Bem-vindo, {usuarioUsername}!";
            await CarregarTimeline();
            await CarregarUsuarios();
            await CarregarMeusSeguindo();
        }

        // Busca o feed central e vincula DTOs tipados às colunas da tabela.
        private async System.Threading.Tasks.Task CarregarTimeline()
        {
            try
            {
                var resposta = await servico.ListarMensagensAsync();

                if (resposta?.Sucesso == true)
                {
                    var mensagens = JsonSerializer.Deserialize<List<MensagemResumo>>(resposta.PayloadJson);

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

        // Atualiza os candidatos a seguir, mantendo ID como valor e username como texto.
        private async System.Threading.Tasks.Task CarregarUsuarios()
        {
            try
            {
                var resposta = await servico.ListarUsuariosAsync();

                if (resposta?.Sucesso == true)
                {
                    var usuarios = JsonSerializer.Deserialize<List<UsuarioResumo>>(resposta.PayloadJson);

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

        // Substitui a lista de seguindo para não duplicar itens a cada atualização.
        private async System.Threading.Tasks.Task CarregarMeusSeguindo()
        {
            try
            {
                var resposta = await servico.ListarMeusSeguindoAsync(usuarioId);

                if (resposta?.Sucesso == true)
                {
                    var seguindo = JsonSerializer.Deserialize<List<UsuarioResumo>>(resposta.PayloadJson) ?? new();

                    lstSeguindo.DataSource = null;
                    lstSeguindo.Items.Clear();
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

        // Valida o limite de texto, solicita a gravação no servidor e recarrega o feed.
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

        // Envia os IDs de quem segue e de quem será seguido ao servidor.
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

        // Confirma a exclusão e envia o comando; o repositório também verifica a autoria.
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

        // Fecha o feed e retorna ao login sem encerrar o servidor.
        private void btnLogout_Click(object sender, EventArgs e)
        {
            servico?.Dispose();
            this.Close();
        }

        // Consulta novamente o servidor para refletir alterações feitas pelo outro PC.
        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            await CarregarTimeline();
            await CarregarUsuarios();
            await CarregarMeusSeguindo();
        }
    }
}
