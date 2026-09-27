# 👥 DIVISÃO DE TAREFAS - GRUPO COM 3 INTEGRANTES

## 📋 Overview da Divisão

```
┌─────────────────────────────────────────────────────────────┐
│                   PROJETO: Twitter Simplificado              │
│                      Disciplina: Redes (EACH-USP)           │
├─────────────────────────────────────────────────────────────┤
│ Integrante 1      │ Integrante 2      │ Integrante 3        │
│ (Backend)         │ (Rede/Cliente)    │ (Interface)         │
│                   │                   │                     │
│ • Repositórios    │ • Conexão TCP     │ • Telas (UI)        │
│ • Banco de Dados  │ • ServicoApi      │ • Validações UI     │
│ • Lógica Server   │ • Protocolo       │ • Integração        │
│ • Handlers        │ • Testes Rede     │ • Testes Fluxo      │
└─────────────────────────────────────────────────────────────┘
```

---

## 👤 INTEGRANTE 1: Backend e Banco de Dados

**Especialidade**: Servidor, Repositórios, Banco de Dados  
**Linguagem**: C#  
**Tecnologias**: Entity Framework Core, SQLite, Sockets TCP  

### 📁 Arquivos Principais
```
epjb/
├── Repositorio/
│   ├── UsuarioRepositorio.cs      ← TODO
│   ├── MensagemRepositorio.cs     ← TODO
│   └── SeguidorRepositorio.cs     ← TODO
├── Sockets/
│   └── Class1.cs                  ← TODO (handlers)
├── Data/
│   └── AppDbContext.cs            ← Já existe
├── Models/
│   ├── Usuario.cs
│   ├── Mensagem.cs
│   └── UsuarioSeguidor.cs
├── Migrations/
│   └── [Geradas automaticamente]
```

### ✅ Tarefas Assign adas

#### **Semana 1: Repositórios (15h)**
1. [ ] **UsuarioRepositorio.cs**
   - [ ] `Autenticar(username, senha)` - Login
   - [ ] `Registrar(username, senha)` - Novo usuário
   - [ ] `ObterPorId(id)` - Buscar por ID
   - [ ] `ListarTodos()` - Listar todos
   - [ ] `ObterPorUsername(username)` - Buscar por username
   - Validações:
	 - [ ] Username não vazio
	 - [ ] Username único
	 - [ ] Senha não vazia

2. [ ] **MensagemRepositorio.cs**
   - [ ] `Criar(idUsuario, conteudo)` - Postar
   - [ ] `ListarTodas()` - Timeline
   - [ ] `ListarPorUsuario(idUsuario)` - Mensagens do user
   - [ ] `Editar(idMensagem, idUsuario, conteudo)` - Editar
   - [ ] `Deletar(idMensagem, idUsuario)` - Remover
   - [ ] `ObterPorId(id)` - Buscar por ID
   - Validações:
	 - [ ] Max 500 caracteres
	 - [ ] Não vazia
	 - [ ] Apenas proprietário pode deletar/editar

3. [ ] **SeguidorRepositorio.cs**
   - [ ] `Seguir(idUsuario, idASeguir)` - Começar seguir
   - [ ] `Desseguir(idUsuario, idDesseguir)` - Parar seguir
   - [ ] `ListarSeguindo(idUsuario)` - Meus seguindo
   - [ ] `ListarSeguidores(idUsuario)` - Meus seguidores
   - [ ] `EstaSeguidoPor(idUsuario, idOutro)` - Verificar
   - [ ] `ObterRelacionamento(id1, id2)` - Buscar relação
   - Validações:
	 - [ ] Usuário não pode seguir a si mesmo
	 - [ ] Não permite duplicatas

#### **Semana 2: Banco de Dados (10h)**
4. [ ] **Migrations do EF Core**
   - [ ] Criar migration inicial: `Add-Migration InitialCreate`
   - [ ] Aplicar: `Update-Database`
   - [ ] Verificar tabelas criadas:
	 ```sql
	 SELECT name FROM sqlite_master WHERE type='table';
	 ```
   - [ ] Adicionar dados de teste (3-5 usuários)

5. [ ] **Testes do Banco**
   - [ ] Testar CRUD Usuarios
   - [ ] Testar CRUD Mensagens
   - [ ] Testar CRUD UsuarioSeguidores
   - [ ] Testar validações
   - [ ] Testar relacionamentos (Foreign Keys)

#### **Semana 3: Servidor (15h)**
6. [ ] **Implementar handlers em Sockets/Class1.cs**
   - [ ] ProcessarLogin()
   - [ ] ProcessarCadastro()
   - [ ] ProcessarListarMensagens()
   - [ ] ProcessarPostarMensagem()
   - [ ] ProcessarDeletarMensagem()
   - [ ] ProcessarSeguir()
   - [ ] ProcessarListarUsuarios()
   - [ ] ProcessarListarMeusSeguidores()
   - [ ] ProcessarListarMeusSeguindo()

7. [ ] **Testes de Servidor**
   - [ ] Teste: Conectar ao servidor
   - [ ] Teste: Enviar comando LOGIN válido
   - [ ] Teste: Enviar comando LOGIN inválido
   - [ ] Teste: Enviar comando CADASTRO
   - [ ] Teste: Enviar comando POSTAR_MSG
   - [ ] Teste: Tratamento de erros

### 📝 Documentação
- [ ] Documentar cada repositório (comentários JSDoc)
- [ ] Documentar protocolo de erro no README
- [ ] Criar `BANCO_DADOS.md` com:
  - Schema do banco
  - Queries importantes
  - Como adicionar dados de teste

### 🧪 Testes
```csharp
// Teste exemplo: Autenticação
[Fact]
public void Autenticar_ComCredenciaisValidas_RetornaUsuario()
{
	var repo = new UsuarioRepositorio();
	var usuario = repo.Autenticar("testuser", "senha123");
	Assert.NotNull(usuario);
}
```

### 📊 Checklist de Entrega
- [ ] Todos os repositórios implementados
- [ ] Banco criado com migrations
- [ ] Todos os handlers do servidor funcionando
- [ ] Sem erros de compilação
- [ ] Testes passando
- [ ] Código documentado
- [ ] Commit no Git: `git commit -m "Backend: Repositórios e Handlers"`

---

## 👤 INTEGRANTE 2: Cliente de Rede

**Especialidade**: Comunicação TCP/IP, Protocolo, Cliente  
**Linguagem**: C#  
**Tecnologias**: System.Net.Sockets, Async/Await, JSON  

### 📁 Arquivos Principais
```
epjb/
├── Cliente/Rede/
│   ├── ConexaoServidor.cs         ← TODO
│   └── ServicoApi.cs              ← TODO
├── Common/
│   ├── Protocolo.cs               ← Já existe
│   ├── Mensagem.cs                ← Já existe
│   ├── Comando.cs                 ← TODO (enum)
│   └── DTO/
│       ├── LoginRequest.cs
│       ├── CadastroRequest.cs
│       ├── PostarMsgRequest.cs
│       └── SeguirRequest.cs
```

### ✅ Tarefas Designadas

#### **Semana 1: ConexaoServidor (12h)**
1. [ ] **Implementar ConexaoServidor.cs**
   - [ ] `Conectar()` - Conectar ao servidor
	 - Usar `IPAddress.Loopback` (127.0.0.1)
	 - Porta: `Config.Porta` (4343)
   - [ ] `Enviar(Mensagem)` - Enviar via Protocolo
   - [ ] `Receber()` - Receber via Protocolo
   - [ ] `Desconectar()` - Fechar conexão
   - [ ] Property: `EstaConectado` (verificar estado)
   - Tratamento de exceções:
	 - [ ] Connection refused
	 - [ ] Timeout
	 - [ ] Socket closed

#### **Semana 2: ServicoApi (12h)**
2. [ ] **Implementar ServicoApi.cs**
   - [ ] `LoginAsync(username, password)`
   - [ ] `CadastroAsync(username, password)`
   - [ ] `ListarMensagensAsync()`
   - [ ] `PostarMensagemAsync(idUsuario, conteudo)`
   - [ ] `DeletarMensagemAsync(idMensagem, idUsuario)`
   - [ ] `SeguirUsuarioAsync(idUsuario, idASeguir)`
   - [ ] `ListarUsuariosAsync()`
   - [ ] `ListarMeusSeguidoresAsync(idUsuario)`
   - [ ] `ListarMeusSeguindoAsync(idUsuario)`
   - Cada método deve:
	 - [ ] Usar ConexaoServidor para conectar
	 - [ ] Serializar payload com JsonSerializer
	 - [ ] Ser async com Task
	 - [ ] Gerenciar conexão (desconectar no finally)
	 - [ ] Retornar Mensagem com resposta

3. [ ] **Documentar Protocolo**
   - [ ] Criar `PROTOCOLO_TCP.md` com:
	 ```
	 ## Formato de Mensagem

	 ### Header (4 bytes)
	 - Tamanho do payload (big-endian int)

	 ### Payload (variável)
	 - JSON serializado (UTF-8)

	 ### Exemplo: LOGIN
	 Cliente envia:
	 {
	   "tipo": 0,  // Comando.LOGIN
	   "payloadJson": "{\"username\":\"pedro\",\"password\":\"123\"}",
	   "sucesso": false,
	   "erro": null
	 }

	 Servidor responde:
	 {
	   "tipo": 0,
	   "payloadJson": "{\"id\":1,\"username\":\"pedro\"}",
	   "sucesso": true,
	   "erro": null
	 }
	 ```

#### **Semana 3: Testes e Otimizações (8h)**
4. [ ] **Testes de Cliente**
   - [ ] Teste: Conectar/Desconectar
   - [ ] Teste: Enviar LOGIN bem-sucedido
   - [ ] Teste: Enviar LOGIN falhado
   - [ ] Teste: Enviar CADASTRO
   - [ ] Teste: Timeout de conexão
   - [ ] Teste: Servidor não responde
   - [ ] Teste: Múltiplas requisições sequenciais

5. [ ] **Tratamento de Erros Robustos**
   - [ ] Retry automático? (↓ Não necessário inicialmente)
   - [ ] Timeout configurável
   - [ ] Logging de requisições/respostas (console)
   - [ ] Exceções personalizadas:
	 ```csharp
	 public class ConexaoServidorException : Exception { }
	 ```

### 📝 Documentação
- [ ] Criar `PROTOCOLO_TCP.md`
- [ ] Criar `GUIA_CLIENTE.md` com exemplos de uso
- [ ] Documentar cada método com exemplos

### 🧪 Testes
```csharp
// Teste exemplo: Conectar
[Fact]
public void Conectar_ComServidorRodando_Sucesso()
{
	var conexao = new ConexaoServidor();
	conexao.Conectar();
	Assert.True(conexao.EstaConectado);
	conexao.Desconectar();
}

// Teste exemplo: Enviar/Receber
[Fact]
public async Task LoginAsync_ComCredenciaisValidas_RetornaSucesso()
{
	var servico = new ServicoApi();
	var resposta = await servico.LoginAsync("testuser", "senha123");
	Assert.True(resposta.Sucesso);
}
```

### 📊 Checklist de Entrega
- [ ] ConexaoServidor implementado e testado
- [ ] ServicoApi com todos os 9 métodos
- [ ] Protocolo documentado em PROTOCOLO_TCP.md
- [ ] Testes passando
- [ ] Sem erros de compilação
- [ ] Código documentado
- [ ] Commit no Git: `git commit -m "Cliente: Rede e APIs"`

---

## 👤 INTEGRANTE 3: Interface Gráfica (UI)

**Especialidade**: Windows Forms, UI/UX, Integração  
**Linguagem**: C#  
**Tecnologias**: Windows Forms, DataGridView, Validation  

### 📁 Arquivos Principais
```
epjb/View/
├── Login.cs & Login.Designer.cs       ← TODO
├── Menu.cs & Menu.Designer.cs         ← TODO
├── Cadastro.cs & Cadastro.Designer.cs ← TODO
```

### ✅ Tarefas Designadas

#### **Semana 1: Tela de Login (10h)**
1. [ ] **Implementar Login.cs e Designer**
   - [ ] Controles:
	 - [ ] Label "Username:"
	 - [ ] TextBox `txtUsuario`
	 - [ ] Label "Senha:"
	 - [ ] TextBox `txtSenha` (PasswordChar = '*')
	 - [ ] Button "Login"
	 - [ ] Button "Cadastro"
	 - [ ] Button "Fechar"

   - [ ] Funcionalidades:
	 - [ ] `btnLogin_Click()` - Chamar ServicoApi.LoginAsync()
	 - [ ] Se sucesso → Abrir Menu com ID e username
	 - [ ] Se erro → MessageBox com erro
	 - [ ] `btnCadastro_Click()` - Abrir tela Cadastro
	 - [ ] `btnFechar_Click()` - Fechar app
	 - [ ] Validações:
	   - [ ] Username não vazio
	   - [ ] Senha não vazia
	   - [ ] Mostrar spinner/mensagem "Autenticando..."

#### **Semana 2: Tela de Cadastro (10h)**
2. [ ] **Implementar Cadastro.cs e Designer**
   - [ ] Controles:
	 - [ ] Label "Username:"
	 - [ ] TextBox `txtUsuario`
	 - [ ] Label "Senha:"
	 - [ ] TextBox `txtSenha` (PasswordChar = '*')
	 - [ ] Label "Confirmar Senha:"
	 - [ ] TextBox `txtConfirmaSenha` (PasswordChar = '*')
	 - [ ] Button "Cadastrar"
	 - [ ] Button "Voltar"

   - [ ] Funcionalidades:
	 - [ ] `btnCadastrar_Click()` - Chamar ServicoApi.CadastroAsync()
	 - [ ] Validações:
	   - [ ] Username mínimo 3 caracteres
	   - [ ] Senha mínimo 6 caracteres
	   - [ ] Senhas devem coincidir
	   - [ ] Se OK → Mostrar mensagem de sucesso e fechar
	   - [ ] Se erro → MessageBox com erro
	 - [ ] `btnVoltar_Click()` - Voltar ao login

#### **Semana 3: Tela Menu/Timeline (15h)**
3. [ ] **Implementar Menu.cs e Designer**
   - [ ] **Painel Superior** (Cabeçalho)
	 - [ ] Label: `lblUsuario` - "Bem-vindo, {username}!"
	 - [ ] Button: `btnLogout` - Logout

   - [ ] **Painel Central Esquerdo** (Timeline)
	 - [ ] DataGridView: `dgvMensagens`
	   - Colunas: ID, IdUsuario, Username, Conteudo, DataCriacao, DataEdicao
	   - Mostrar todas as mensagens ordenadas por data (decrescente)
	   - Selectable para deletar
	 - [ ] Button: `btnAtualizar` - Recarregar timeline

   - [ ] **Painel Central Direito Inferior** (Postar)
	 - [ ] Label: "Nova Mensagem:"
	 - [ ] TextBox Multiline: `txtConteudo` (max 500 chars)
	 - [ ] Button: `btnPostar` - Postar mensagem
	 - [ ] Label contador de caracteres? (↓ Opcional)

   - [ ] **Painel Direita Superior** (Seguir)
	 - [ ] Label: "Seguir Usuários:"
	 - [ ] ComboBox: `cmbUsuarios` - Lista de usuários
	 - [ ] Button: `btnSeguir` - Começar seguir
	 - [ ] Validações:
	   - [ ] Não permitir seguir a si mesmo
	   - [ ] Mostrar mensagem de sucesso/erro

   - [ ] **Painel Direita Inferior** (Meus Seguindo)
	 - [ ] Label: "Meus Seguindo:"
	 - [ ] ListBox: `lstSeguindo` - Usuários que segue

   - [ ] **Botões Ação**
	 - [ ] Button: `btnDeletar` - Deletar mensagem selecionada
	   - [ ] Validar: apenas próprias mensagens
	   - [ ] Pedir confirmação
	 - [ ] Button: `btnAtualizar` - Recarregar tudo
	 - [ ] Button: `btnLogout` - Voltar ao login

#### **Semana 3: Integração e Fluxo (10h)**
4. [ ] **Carregamento de Dados**
   - [ ] `Menu_Load()` deve:
	 - [ ] Chamar `CarregarTimeline()` - Listar todas mensagens
	 - [ ] Chamar `CarregarUsuarios()` - Listar usuários
	 - [ ] Chamar `CarregarMeusSeguindo()` - Listar meus seguindo

5. [ ] **Métodos Assíncronos**
   - [ ] `CarregarTimeline()` - await ListarMensagensAsync()
   - [ ] `CarregarUsuarios()` - await ListarUsuariosAsync()
   - [ ] `CarregarMeusSeguindo()` - await ListarMeusSeguindoAsync()
   - [ ] `btnPostar_Click()` - await PostarMensagemAsync()
   - [ ] `btnSeguir_Click()` - await SeguirUsuarioAsync()
   - [ ] `btnDeletar_Click()` - await DeletarMensagemAsync()
   - [ ] `btnLogout_Click()` - Fechar Menu e voltar Login

### 🎨 Design Recomendado
```
┌─────────────────────────────────────────────────────────────┐
│ Bem-vindo, usuario!                          [Logout]       │
├──────────────────────────────┬──────────────────────────────┤
│ TIMELINE                     │ SEGUIR                       │
│ ┌────────────────────────┐   │ ┌──────────────────────────┐ │
│ │ ID │ User │ Conteúdo  │   │ │ Dropdown [Seguir]        │ │
│ │────┼──────┼───────────│   │ └──────────────────────────┘ │
│ │    │      │           │   │                              │
│ │    │      │           │   │ MEUS SEGUINDO               │
│ │    │      │           │   │ ┌──────────────────────────┐ │
│ │    │      │           │   │ │ user1                    │ │
│ └────────────────────────┘   │ │ user2                    │ │
│                              │ │ user3                    │ │
│ NOVA MENSAGEM:              │ └──────────────────────────┘ │
│ ┌────────────────────────┐   │                              │
│ │ Escreva uma msg...     │   │                              │
│ └────────────────────────┘   │                              │
│                              │                              │
│ [Deletar] [Atualizar]        │ [Logout]                     │
└──────────────────────────────┴──────────────────────────────┘
```

### 📝 Documentação
- [ ] Documentar cada tela em `TELAS.md`
- [ ] Documentar fluxo de navegação
- [ ] Criar guia de uso (usuário final)

### 🧪 Testes
```csharp
// Teste: Login com dados válidos
[Fact]
public void Login_ComDadosValidos_AbreMenu()
{
	// Simular clique
	// Verificar que Menu foi aberto
	// Verificar que ID/username foram passados
}

// Teste: Validação de campos vazios
[Fact]
public void Login_ComCamposVazios_MostraErro()
{
	// Deixar campos vazios
	// Clicar Login
	// Verificar MessageBox
}
```

### 📊 Checklist de Entrega
- [ ] Telas de Login, Cadastro e Menu criadas
- [ ] Todos os botões funcionando
- [ ] Validações de entrada implementadas
- [ ] Mensagens de erro/sucesso mostradas
- [ ] DataGridView carregando mensagens
- [ ] ComboBox carregando usuários
- [ ] ListBox carregando seguindo
- [ ] Fluxo completo funcional (login → menu → logout → login)
- [ ] Sem erros de compilação
- [ ] Código documentado
- [ ] Commit no Git: `git commit -m "UI: Telas e Integração"`

---

## 📅 Timeline Proposto

```
Semana 1:
  Integrante 1: Repositórios (UsuarioRepositorio, MensagemRepositorio, SeguidorRepositorio)
  Integrante 2: ConexaoServidor.cs (Gerenciamento TCP)
  Integrante 3: Tela Login.cs

Semana 2:
  Integrante 1: Banco de Dados + Migrations
  Integrante 2: ServicoApi.cs (APIs)
  Integrante 3: Tela Cadastro.cs

Semana 3:
  Integrante 1: Handlers do Servidor + Testes
  Integrante 2: Testes de Cliente + Documentação Protocolo
  Integrante 3: Tela Menu.cs + Integração Completa

Apresentação Final:
  Todos: Apresentar funcionalidade completa (15 min)
```

---

## 🔗 Dependências Entre Integrantes

```
Integrante 3 (UI)
	↓ usa
Integrante 2 (Cliente/API)
	↓ usa
Integrante 1 (Servidor/BD)
```

**Ordem de Implementação Recomendada**:
1. **Integrante 1 primeiro** (Backend precisa estar pronto)
2. **Integrante 2 depois** (Cliente se conecta ao servidor)
3. **Integrante 3 por último** (UI chama as APIs)

**Integração**: Faça merge no Git ao fim de cada semana!

---

## 🤝 Comunicação e Git

### **Branch Strategy**
```
master (main)
├── integrante1/backend
├── integrante2/cliente
└── integrante3/ui
```

### **Commits Sugeridos**
```bash
# Integrante 1
git commit -m "feat(backend): Implementar UsuarioRepositorio"
git commit -m "feat(backend): Implementar MensagemRepositorio"
git commit -m "feat(database): Criar migrations EF Core"
git commit -m "feat(server): Implementar handlers de comandos"

# Integrante 2
git commit -m "feat(client): Implementar ConexaoServidor"
git commit -m "feat(client): Implementar ServicoApi"
git commit -m "docs: Documentar protocolo TCP"
git commit -m "test(client): Testes de conexão"

# Integrante 3
git commit -m "feat(ui): Criar tela de Login"
git commit -m "feat(ui): Criar tela de Cadastro"
git commit -m "feat(ui): Criar Menu/Timeline"
git commit -m "feat(ui): Integração com ServicoApi"
git commit -m "test(ui): Testar fluxo completo"
```

### **Code Review**
Ao terminar cada tarefa:
1. Push para sua branch
2. Abra Pull Request
3. Colega revisa código
4. Merge para main após aprovação

---

## 📚 Recursos Úteis

### **Para Integrante 1 (Backend)**
- Entity Framework Core Docs: https://docs.microsoft.com/en-us/ef/core/
- SQLite: https://www.sqlite.org/
- LINQ: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/

### **Para Integrante 2 (Cliente/Rede)**
- System.Net.Sockets: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets
- Async/Await: https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming
- JSON: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview

### **Para Integrante 3 (UI)**
- Windows Forms: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/
- DataGridView: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridview
- UI Best Practices: https://docs.microsoft.com/en-us/windows/win32/uxguide/vis-layout

---

## ✅ Checklist Final

- [ ] Todos os arquivos implementados
- [ ] Sem erros de compilação (`Ctrl+Shift+B`)
- [ ] Aplicação executa (`F5`)
- [ ] Fluxo completo funciona (Login → Menu → Logout)
- [ ] Todos os commits feitos no Git
- [ ] Documentação completa
- [ ] Testes implementados
- [ ] Pronto para apresentação (15 min)

---

**Boa sorte! 🚀**  
Qualquer dúvida, revisar `ARQUITETURA.md` ou `STATUS_IMPLEMENTACAO.md`
