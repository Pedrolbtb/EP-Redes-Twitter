using epjb.Cliente.Rede;
using epjb.Common;

namespace epjb.View;

// Tela escrita manualmente: permite corrigir o endereço sem editar arquivos no Windows.
public sealed class ConfiguracaoConexao : Form
{
    private readonly TextBox host = new() { Dock = DockStyle.Fill };
    private readonly NumericUpDown porta = new() { Minimum = 1, Maximum = 65535, Value = Config.Porta, Dock = DockStyle.Fill };
    private readonly Button testar = new() { Text = "Testar conexão", AutoSize = true };
    private readonly Button testarUdp = new() { Text = "Testar UDP (diagnóstico)", AutoSize = true };
    private readonly Button salvar = new() { Text = "Salvar", AutoSize = true };
    private readonly Label resultado = new() { AutoSize = true, MaximumSize = new Size(510, 0) };

    public ConfiguracaoConexao()
    {
        Text = "Conexão com o servidor";
        ClientSize = new Size(560, 470);
        AutoScaleMode = AutoScaleMode.Dpi;
        StartPosition = FormStartPosition.CenterParent;
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, AutoScroll = true, Padding = new Padding(16)
        };
        Controls.Add(layout);
        layout.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(510, 0), Text =
            "Primeiro inicie ServerSide no PC do banco. Informe o IPv4 desse PC. " +
            "localhost só funciona quando o servidor está neste mesmo computador." });
        var campos = new TableLayoutPanel { Width = 510, Height = 70, ColumnCount = 2, RowCount = 2 };
        campos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        campos.Controls.Add(new Label { Text = "IP ou nome:", AutoSize = true }, 0, 0);
        campos.Controls.Add(host, 1, 0);
        campos.Controls.Add(new Label { Text = "Porta TCP:", AutoSize = true }, 0, 1);
        campos.Controls.Add(porta, 1, 1);
        layout.Controls.Add(campos);
        layout.Controls.Add(testar);
        layout.Controls.Add(testarUdp);
        layout.Controls.Add(salvar);
        layout.Controls.Add(resultado);

        try
        {
            var config = ConfiguracaoServidor.Carregar();
            host.Text = config.Host;
            porta.Value = config.Porta;
        }
        catch (Exception ex)
        {
            // Até um JSON corrompido pode ser recuperado: o usuário preenche e salva novamente.
            host.Text = "localhost";
            resultado.Text = ex.Message;
        }
        if (Environment.GetEnvironmentVariable("EPJB_SERVER_HOST") is string hostAmbiente)
        {
            // Não permitir salvar um endereço que seria ignorado na próxima leitura da configuração.
            host.Text = hostAmbiente;
            host.ReadOnly = true;
            resultado.Text = "O endereço vem de EPJB_SERVER_HOST. Altere/remova essa variável e reabra o cliente para usar outro IP.";
        }
        testar.Click += TestarConexao;
        testarUdp.Click += TestarUdp;
        salvar.Click += SalvarConfiguracao;
    }

    // Captura os controles na thread da interface antes de iniciar qualquer trabalho em segundo plano.
    private ConfiguracaoServidor LerCampos() => new() { Host = host.Text.Trim(), Porta = (int)porta.Value };

    private async void TestarConexao(object? sender, EventArgs e)
    {
        var config = LerCampos();
        testar.Enabled = testarUdp.Enabled = salvar.Enabled = false;
        resultado.Text = $"Testando {config.Host}:{config.Porta}...";
        try
        {
            await Task.Run(() =>
            {
                using var conexao = new ConexaoServidor(config);
                conexao.Conectar();
                // Validar uma resposta do protocolo, não apenas uma porta aberta de outro programa.
                conexao.Enviar(new Mensagem(Comando.LISTAR_USUARIOS, "{}"));
                var resposta = conexao.Receber();
                if (resposta.Tipo != Comando.LISTAR_USUARIOS || !resposta.Sucesso)
                    throw new IOException(resposta.Erro ?? "Resposta incompatível com o protocolo EPJB.");
            });
            resultado.Text = $"Servidor e banco responderam em {config.Host}:{config.Porta}. Clique em Salvar para usar este endereço.";
        }
        catch (Exception ex) { resultado.Text = ex.Message; }
        finally { testar.Enabled = testarUdp.Enabled = salvar.Enabled = true; }
    }

    // O diagnóstico UDP não substitui o teste TCP, que também verifica protocolo e acesso ao banco.
    private async void TestarUdp(object? sender, EventArgs e)
    {
        var config = LerCampos();
        testar.Enabled = testarUdp.Enabled = salvar.Enabled = false;
        resultado.Text = $"Testando UDP em {config.Host}:{ProtocoloUdp.Porta}...";
        try
        {
            await Task.Run(() => DiagnosticoUdp.Testar(config));
            resultado.Text = "PING/PONG UDP confirmado. Para login/feed, confira também Testar conexão (TCP).";
        }
        catch (Exception ex) { resultado.Text = ex.Message; }
        finally { testar.Enabled = testarUdp.Enabled = salvar.Enabled = true; }
    }

    private void SalvarConfiguracao(object? sender, EventArgs e)
    {
        try
        {
            LerCampos().Salvar();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            resultado.Text = $"Não foi possível salvar {ConfiguracaoServidor.CaminhoArquivo}: {ex.Message}";
        }
    }
}
