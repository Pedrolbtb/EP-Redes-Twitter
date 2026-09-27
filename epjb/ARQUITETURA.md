# Arquitetura do Projeto EP-Redes-Twitter

## Estado Atual (Fase 3 Concluída)

### Estrutura de Pastas
```
epjb/
├── Cliente/
│   └── Rede/
│       ├── ConexaoServidor.cs  (Camada TCP pura)
│       └── ServicoApi.cs        (Wrapper de chamadas)
├── Common/
│   ├── Comando.cs              (Enum de comandos)
│   ├── Config.cs               (Configurações)
│   ├── Mensagem.cs             (Formato de mensagem)
│   ├── Protocolo.cs            (Serialização/envio)
│   └── DTO/                     (Data Transfer Objects)
├── Data/
│   └── AppDbContext.cs         (Entity Framework)
├── Models/
│   ├── Usuario.cs
│   ├── Mensagem.cs
│   └── UsuarioSeguidor.cs
├── Repositorio/
│   └── UsuarioRepositorio.cs   (Acesso ao banco)
├── Sockets/
│   └── Class1.cs               (Servidor TCP com processamento)
├── View/
│   ├── Login.cs/Designer.cs    (UI - Usa ServicoApi)
│   └── Menu.cs/Designer.cs     (UI - Futuro)
├── Program.cs                  (Entry point)
└── epjb.csproj
```

### Fluxo de Dados (Login Example)
```
┌──────────┐
│   User   │ Insira username/senha
└────┬─────┘
	 │
	 ▼
┌────────────────────────────────┐
│  View/Login.cs                 │
│  btnLogin_Click()              │
│  Cria ServicoApi               │
└────┬───────────────────────────┘
	 │ await serviço.LoginAsync()
	 ▼
┌──────────────────────────────┐
│  Cliente/Rede/ServicoApi.cs  │
│  ExecutarComandoAsync()       │
│  Serializa payload JSON       │
└────┬────────────────────────┘
	 │
	 ▼
┌──────────────────────────────┐
│  Cliente/Rede/                │
│  ConexaoServidor.cs          │
│  Conectar()                  │
│  Enviar(Mensagem)            │
│  Receber()                   │
└────┬──────────────────────────┘
	 │ Socket TCP
	 │
	 ▼
  [REDE - Loopback:4343]
	 │
	 ▼
┌──────────────────────────────┐
│  Sockets/Class1.cs           │
│  AsyncSocketListener          │
│  HandleClient()              │
│  ProcessarComando()          │
└────┬──────────────────────────┘
	 │ ProcessarLogin()
	 ▼
┌──────────────────────────────┐
│  Repositorio/                 │
│  UsuarioRepositorio.cs       │
│  Autenticar(username, pwd)   │
└────┬──────────────────────────┘
	 │ Query SQLite
	 ▼
┌──────────────────────────────┐
│  SQLite Database             │
│  epjb.db                     │
│  Tabela: Usuarios            │
└──────────────────────────────┘
```

## Decisão Arquitetural: Projeto Único com Separação de Camadas

### Opção Escolhida: ✅ Projeto Único
- **Um arquivo .csproj**: `epjb/epjb.csproj`
- **Servidor e Cliente no mesmo processo** (temporariamente)
- **Separação lógica por pastas/namespaces**

### Justificativa
1. **Fase de Desenvolvimento**: Mais fácil fazer debug com tudo no mesmo processo
2. **Simplifica Deployment Inicial**: Uma única DLL para executar
3. **Compartilhamento de Código**: Common, Models, Data são reutilizáveis
4. **Escalação Futura**: Estrutura permite separar em múltiplos projetos depois

### Camadas Implementadas

#### 1. **Camada de Apresentação (View)**
- `View/Login.cs` - Formulário de login
- `View/Menu.cs` - Menu principal (futuro)
- **Responsabilidade**: UI, não contém lógica de negócio

#### 2. **Camada de API/Serviços (Cliente)**
- `Cliente/Rede/ServicoApi.cs` - Chamadas de alto nível
- **Responsabilidade**: Orquestrar ConexaoServidor para operações específicas

#### 3. **Camada de Comunicação (Cliente)**
- `Cliente/Rede/ConexaoServidor.cs` - Gerenciamento de socket TCP
- **Responsabilidade**: Conectar, enviar, receber, desconectar

#### 4. **Camada de Aplicação (Servidor)**
- `Sockets/Class1.cs` - Listener TCP e processamento de comandos
- **Responsabilidade**: Receber conexões, validar comandos, chamar repositório

#### 5. **Camada de Dados (Repositório)**
- `Repositorio/UsuarioRepositorio.cs` - Acesso ao banco
- **Responsabilidade**: Autenticar, registrar, consultar usuários

#### 6. **Camada de Persistência (EF Core)**
- `Data/AppDbContext.cs` - Entity Framework
- `Models/` - Modelos de dados
- **Responsabilidade**: Mapear ORM, executar queries

#### 7. **Camada Comum**
- `Common/Protocolo.cs` - Serialização (cliente e servidor usam)
- `Common/Mensagem.cs` - Formato padrão de mensagem
- `Common/DTO/` - Objetos de transferência de dados
- **Responsabilidade**: Contrato compartilhado

## Como Será Executado

### Hoje (Fase 3)
```
Program.cs:
  │
  ├─ Task.Run(() => AsyncSocketListener.StartListener())  [Servidor em background]
  └─ Application.Run(new View.Login())                     [UI em primeiro plano]
```

### Recomendação Futura (Fase 4+)

#### Opção A: Dois Projetos
```
epjb.sln
├── epjb.Client.csproj      (UI + Cliente)
│   └── Referencia epjb.Common.csproj
│
├── epjb.Server.csproj      (Servidor console)
│   └── Referencia epjb.Common.csproj
│
└── epjb.Common.csproj      (Compartilhado)
	├── Common/
	├── Models/
	├── Data/
	└── Repositorio/
```

#### Opção B: Três Projetos
```
epjb.sln
├── epjb.Client.Wpf.csproj
├── epjb.Server.Console.csproj
└── epjb.Shared.csproj
```

#### Opção C: Continuação com Projeto Único
- Manter tudo junto
- Adicionar feature flag para modo "server-only"
- Útil se servidor sempre roda com cliente

## Pontos de Decisão do Grupo

Discuta com o grupo sobre:

### 1. **Modo de Execução do Servidor**
- [ ] **Junto com Cliente** (atual) - Servidor em background na mesma app
- [ ] **Processo Separado** - Servidor roda em outro .exe
- [ ] **Serviço Windows** - Servidor como NT Service
- [ ] **Containerizado** - Servidor em Docker

**Recomendação**: Começar com processo separado na próxima fase

### 2. **Estrutura de Projetos Futura**
- [ ] Manter tudo em um projeto
- [ ] Separar em 2 projetos (Client + Server)
- [ ] Separar em 3 projetos (Client + Server + Shared)
- [ ] Usar padrão Monorepo com múltiplas soluções

**Recomendação**: 3 projetos para melhor separação

### 3. **Segurança de Senha**
- [ ] Texto plano (atual - **NÃO RECOMENDADO**)
- [ ] Hash MD5
- [ ] Hash SHA256
- [ ] bcrypt

**Recomendação**: Implementar bcrypt (NuGet: BCrypt.Net-Core)

### 4. **Autenticação/Sessão**
- [ ] Sem sessão (atual)
- [ ] Token (JWT)
- [ ] Session ID
- [ ] OAuth2

**Recomendação**: JWT com refresh token

## Checklist de Implementação Completa

### ✅ Fase 1 - Protocolo (Concluído)
- [x] Estrutura básica de mensagem
- [x] Serialização JSON
- [x] Envio/recebimento TCP

### ✅ Fase 2 - Comunicação (Concluído)
- [x] Cliente conectando ao servidor
- [x] Servidor respondendo com eco
- [x] Testes de ponta a ponta

### ✅ Fase 3 - Arquitetura (Concluído - Esta Fase)
- [x] Extrair ConexaoServidor.cs
- [x] Criar ServicoApi.cs
- [x] Refatorar Login.cs
- [x] Criar UsuarioRepositorio.cs
- [x] Conectar servidor ao banco

### 📋 Fase 4 - Produção (Próximo)
- [ ] Hash de senha (bcrypt)
- [ ] Validação de campos
- [ ] Testes unitários
- [ ] Separar servidor em processo independente
- [ ] Implementar Cadastro na UI
- [ ] Fechar conexão após deslogar

### 📋 Fase 5 - Funcionalidades (Futuro)
- [ ] Implementar LISTAR_MSGS
- [ ] Implementar POSTAR_MSG
- [ ] Implementar SEGUIR
- [ ] Menu principal funcional
- [ ] Timeline de mensagens

## Referências de Código

### Usando ServicoApi
```csharp
var servico = new ServicoApi();
var resposta = await servico.LoginAsync(username, password);
if (resposta.Sucesso)
{
	// Login bem-sucedido
}
```

### Usando UsuarioRepositorio Diretamente
```csharp
var repo = new UsuarioRepositorio();
var usuario = repo.Autenticar(username, senha);
```

### Adicionando Novo Comando
1. Adicionar enum em `Common/Comando.cs`
2. Adicionar case em `ProcessarComando()`
3. Adicionar método ProcessarXXX()
4. Adicionar método em `ServicoApi.cs`
5. Chamar de `View/Login.cs` ou `View/Menu.cs`

## Próximos Passos Imediatos

1. **Com o Grupo**: Decidir sobre pontos de decisão acima
2. **Implementação**: 
   - Adicionar hash de senha
   - Implementar Cadastro na UI
   - Adicionar testes
3. **Estrutura**: Refatorar para múltiplos projetos se decidido

---
**Última atualização**: Fase 3 Concluída  
**Próxima revisão**: Antes de Fase 4
