# Sockets manuais e diagnóstico de conexão

## O erro “a máquina de destino as recusou ativamente”

A conexão TCP foi recusada antes de o servidor processar LOGIN/CADASTRO.
Isso não significa senha inválida nem prova de erro no banco. Entre as causas
possíveis estão servidor não iniciado, endereço/porta incorretos ou rejeição na
rede. A imagem enviada não identifica qual dessas causas ocorreu no outro PC.

Após separar o banco central, abrir só `epjb` não inicia mais `ServerSide`.
Essa separação é necessária: iniciar um servidor em cada cliente recriaria o
problema de bancos independentes.

No Windows, com SDK .NET 10 instalado:

1. No PC que guarda o banco, execute `iniciar-servidor.cmd`. Mantenha a janela
   aberta e aguarde **Servidor TCP aguardando clientes em 0.0.0.0:11000**.
   Se houver erro de migrations, banco ou porta ocupada, resolva o erro mostrado
   nessa janela antes de tentar login.
2. Execute `ipconfig` no PC servidor e identifique o IPv4 da rede usada pelos PCs.
3. Nos PCs dos usuários, execute `iniciar-cliente.cmd`. Na tela de login, abra
   **Servidor...**, informe esse IPv4 e a porta **11000**.
4. Clique em **Testar conexão**. O teste conecta por TCP, envia LISTAR_USUARIOS
   e aguarda resposta do protocolo; portanto verifica mais que uma porta aberta.
5. Clique em **Salvar** e faça cadastro/login. No próprio PC servidor, o cliente
   também pode usar `localhost`; no outro PC, `localhost` aponta para o PC errado.

Alternativamente, execute na raiz do repositório em terminais separados:

```powershell
# Apenas no PC servidor:
dotnet run --project .\ServerSide\ServerSide.csproj

# Em cada PC cliente, substituindo o exemplo pelo IP real:
$env:EPJB_SERVER_HOST = "192.168.1.50"
dotnet run --project .\epjb\epjb.csproj
```

A variável `EPJB_SERVER_HOST` tem prioridade sobre o JSON. Quando definida, a tela
mostra o motivo de o campo de endereço estar bloqueado. Para usar a tela, remova
a variável no terminal que inicia o programa e reabra o cliente:

```powershell
Remove-Item Env:EPJB_SERVER_HOST -ErrorAction SilentlyContinue
```

No Visual Studio, execute `ServerSide` sem depuração e depois `epjb`, ou configure
ambos como projetos de inicialização no PC servidor. No PC remoto execute só
`epjb`. Não tente executar um segundo servidor na mesma porta.

## Conferência de rede

No cliente Windows:

```powershell
Test-NetConnection 192.168.1.50 -Port 11000
```

`TcpTestSucceeded: False` exige conferir IP, servidor e firewall antes de investigar
credenciais. No PC servidor, para permitir a rede privada local (PowerShell como
administrador):

```powershell
New-NetFirewallRule -DisplayName "EPJB TCP 11000" -Direction Inbound -Protocol TCP -LocalPort 11000 -Action Allow -Profile Private -RemoteAddress LocalSubnet
# Somente para a demonstração UDP:
New-NetFirewallRule -DisplayName "EPJB UDP 11001" -Direction Inbound -Protocol UDP -LocalPort 11001 -Action Allow -Profile Private -RemoteAddress LocalSubnet
```

Redes diferentes precisam de um caminho alcançável, como uma VPN, e regras de
firewall compatíveis com essa rede. Não basta usar o IP privado de outra casa.

## Onde a comunicação é programada

| Etapa | Fonte | Operação explícita |
| --- | --- | --- |
| Abrir conexão TCP | `epjb/Cliente/Rede/ConexaoServidor.cs` | `new Socket`, `ConnectAsync` |
| Escutar/aceitar TCP | `epjb/Sockets/Class1.cs` | `Bind`, `Listen`, `Accept` |
| Enquadrar/enviar/receber | `epjb/Common/Protocolo.cs` | `Send` e `Receive` em loops |
| Escolher comando | `epjb/Sockets/Class1.cs` | `switch` e handlers do grupo |
| Encerrar TCP | `ConexaoServidor.cs` e `Class1.cs` | `Shutdown`, `Close`/`Dispose` |
| Enviar/receber diagnóstico UDP | `epjb/Cliente/Rede/DiagnosticoUdp.cs` | `Socket(Dgram, Udp)`, `Bind`, `SendTo`, `Poll`, `ReceiveFrom` |
| Responder UDP | `epjb/Sockets/DiagnosticoUdpListener.cs` | `Bind`, `ReceiveFrom`, `SendTo` |

`ServicoApi` é uma classe do próprio projeto que monta comandos; não é HTTP,
ASP.NET, RPC, SignalR nem uma biblioteca de transporte. `ConnectAsync` pertence à
própria classe `Socket`. `Task` agenda atendimento concorrente, `System.Text.Json`
converte objetos em texto e Entity Framework acessa o SQLite no servidor. Nenhuma
dessas bibliotecas substitui o código de rede listado acima.

Os comentários `//` e os resumos nos arquivos explicam responsabilidade, sequência
de operações, dados enviados e liberação de recursos. Modelos, repositórios,
eventos das telas e testes também estão comentados. Arquivos gerados pelo
Designer/EF estão identificados como gerados; telas antigas de `ServerSide/View`
estão marcadas como legado excluído da compilação.

## TCP: protocolo principal do Twitter

O quadro é **[4 bytes de comprimento em big-endian][JSON em UTF-8]**. O comprimento
conta somente bytes do JSON. O envelope contém Tipo, PayloadJson, Sucesso e Erro.
Os números do enum Comando não devem ser reordenados.

TCP é fluxo: uma chamada Send/Receive não necessariamente envia/recebe um quadro
inteiro. O código soma bytes transferidos e repete a operação até completar o
cabeçalho e o corpo. Quadros de comprimento inválido ou acima de 4 MiB são
rejeitados. Fechar a conexão no meio do quadro produz erro; fechar entre quadros
é um encerramento normal. O limite de 4 MiB também limita respostas de feeds
muito grandes; paginação ainda não foi implementada.

Cada chamada do cliente possui uma conexão própria. O servidor aceita conexões
em um loop e atende cada socket em uma Task. O prazo de conexão é 10 segundos;
envio/recepção síncronos têm timeout de 15 segundos. O SQLite permanece somente
no processo servidor.

## UDP: demonstração de diagnóstico

Ao iniciar, `ServerSide` também tenta abrir **UDP 11001**. Na tela Servidor...,
**Testar UDP (diagnóstico)** envia `EPJB_PING:<nonce>` em UTF-8. O servidor valida
o nonce (GUID hexadecimal de 32 caracteres) e devolve `EPJB_PONG:<mesmo nonce>`.
Não há prefixo de tamanho: um datagrama já possui fronteiras.

O cliente espera até três segundos por um PONG com nonce correspondente e porta
11001. O IP de origem da resposta pode variar num servidor com várias interfaces.
O nonce correlaciona a resposta; não é autenticação. UDP pode perder, duplicar ou
reordenar pacotes. Não há retransmissão nem confirmação de entrega neste diagnóstico.
Um PONG comprova a troca UDP, não o estado do banco. Uma falha UDP não impede
login/feed por TCP, inclusive se a porta UDP estiver ocupada ao iniciar o servidor.

Cadastro, login, posts e relações continuam em **TCP**. Não há gravações por UDP.
Isso demonstra os dois transportes usando `System.Net.Sockets.Socket`, mantendo
apenas um deles no fluxo principal, conforme a possibilidade descrita no roteiro.

## Testes e limites da validação

O teste de integração cobre configuração salva/variável de ambiente/JSON inválido,
conexão recusada com indicação de IP/porta e instruções, mensagens fragmentadas,
quadros consecutivos, payload grande, tamanho inválido, EOF parcial, PING/PONG UDP,
ausência de servidor UDP, duas contas simultâneas e persistência do banco.

```powershell
dotnet build epjb.slnx
dotnet build tests/Integracao/Integracao.csproj
dotnet tests/Integracao/bin/Debug/net10.0/Integracao.dll "$PWD/ServerSide/bin/Debug/net10.0/ServerSide.dll"
```

No Linux, a compilação da solução requer `-p:EnableWindowsTargeting=true`.
Os testes de rede/banco rodam no Linux; a interface Windows Forms e os atalhos
`.cmd` exigem validação no Windows. As portas 11000/TCP e 11001/UDP devem estar
livres antes do teste. O banco de teste é temporário e não altera o banco legado.

A edição de posts tem enum/repositório, mas ainda não tem handler/tela completos;
a lista de seguidores tem handler/serviço, porém não tem uma tela dedicada.
Esta correção de conexão não declara essas funcionalidades pendentes como prontas.
TLS e sessão autenticada também continuam pendentes: usar rede de confiança.

Referências oficiais: [Socket e operações TCP/UDP](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=net-10.0)
e [códigos de erro de socket](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socketerror?view=net-10.0).

Validação desta revisão (30/09/2026): compilação da solução e do projeto de testes
sem erros, com avisos de nulabilidade; todos os cenários automatizados acima
aprovados com .NET SDK 10.0.401 no Linux. `git diff --check` também aprovado.
