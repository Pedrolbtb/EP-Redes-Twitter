# 📚 BIBLIOTECAS RECOMENDADAS - Twitter Simplificado

## 🎯 Critérios de Seleção

✅ **DEVE TER**:
- Não esconde sockets (acesso direto a TCP/UDP via System.Net.Sockets)
- Suporta TCP e UDP (mesmo que use apenas um)
- .NET 10 compatible
- Bem documentada
- Ativa e mantida

❌ **NÃO USAR**:
- Abstrações que escondem a implementação de rede
- Microframeworks que não permitem controle fino
- Bibliotecas descontinuadas

---

## 📦 Bibliotecas por Categoria

### **1. REDE & COMUNICAÇÃO** 🌐

#### **System.Net.Sockets** ✅ (Já incluído)
```xml
<!-- Já vem com .NET -->
<PackageReference Include="System.Net.Sockets" Version="4.3.0" />
```
- **Uso**: Gerenciamento direto de sockets TCP/UDP
- **Vantagens**: Nativa, sem dependências, total controle
- **Status**: ✅ Usada no projeto

#### **System.Net** ✅ (Já incluído)
```xml
<!-- Já vem com .NET -->
<PackageReference Include="System.Net.Primitives" Version="4.3.0" />
```
- **Uso**: Tipos básicos (IPAddress, IPEndPoint, etc)
- **Vantagens**: Nativa, essencial
- **Status**: ✅ Usada no projeto

#### **System.IO.Pipelines** (Opcional, Avançado)
```xml
<PackageReference Include="System.IO.Pipelines" Version="8.0.0" />
```
- **Uso**: Melhor performance para streams grandes
- **Documentação**: https://github.com/dotnet/corefx/tree/master/src/System.IO.Pipelines
- **Status**: 🟡 Não necessário inicialmente
- **Código Exemplo**:
```csharp
// Melhor para processamento de múltiplos mensagens
// Não necessário para este projeto
```

---

### **2. SERIALIZAÇÃO & PARSING** 📄

#### **System.Text.Json** ✅ (Já incluído)
```xml
<!-- Já vem com .NET -->
<PackageReference Include="System.Text.Json" Version="8.0.0" />
```
- **Uso**: Serialização JSON (protocolo de comunicação)
- **Vantagens**: Nativa, performance, baixa memória
- **Status**: ✅ Usada no projeto
- **Código**:
```csharp
// Serializar
var json = JsonSerializer.Serialize(obj);

// Desserializar
var obj = JsonSerializer.Deserialize<T>(json);
```

#### **Newtonsoft.Json (Json.NET)** (Alternativa)
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```
- **Uso**: Alternativa mais flexível ao System.Text.Json
- **Vantagens**: Suporta mais tipos, mais customizável
- **Desvantagens**: Mais pesada
- **Status**: 🟡 Opcional (System.Text.Json já é bom)

---

### **3. DATABASE & ORM** 🗄️

#### **EntityFramework Core** ✅ (Já incluído)
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
```
- **Uso**: ORM para SQLite
- **Vantagens**: Nativa .NET, migrations automáticas
- **Status**: ✅ Usada no projeto

#### **Microsoft.EntityFrameworkCore.Sqlite** ✅ (Já incluído)
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
```
- **Uso**: Driver SQLite para EF Core
- **Status**: ✅ Usada no projeto

#### **Dapper** (Alternativa Lightweight)
```xml
<PackageReference Include="Dapper" Version="2.1.24" />
```
- **Uso**: Micro ORM, queries SQL rápidas
- **Vantagens**: Muito leve, performance alta
- **Desvantagens**: Requer escrever SQL à mão
- **Status**: 🟡 Se preferir controle total de queries

---

### **4. LOGGING** 📝

#### **Microsoft.Extensions.Logging** (Recomendado)
```xml
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
```
- **Uso**: Logging estruturado
- **Vantagens**: Padrão .NET, flexível
- **Status**: ✅ Recomendado
- **Código**:
```csharp
var logger = LoggerFactory.Create(builder => builder.AddConsole())
	.CreateLogger<MyClass>();
logger.LogInformation("Mensagem de teste");
```

#### **Serilog** (Alternativa Profissional)
```xml
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.1.0" />
```
- **Uso**: Logging profissional com structured logging
- **Vantagens**: Muito poderosa, output customizável
- **Desvantagens**: Overkill para este projeto
- **Status**: 🟡 Se quiser logging elaborado

#### **Log4Net** (Legado, não recomendado)
- **Status**: ❌ Evitar, prefira Serilog ou Extensions.Logging

---

### **5. TESTES** 🧪

#### **xUnit** ✅ (Recomendado)
```xml
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
```
- **Uso**: Framework de testes unitários
- **Vantagens**: Moderno, simples, popular em .NET Core
- **Código**:
```csharp
[Fact]
public void Teste_CenarioSimples()
{
	Assert.True(condicao);
}

[Theory]
[InlineData(1, 2, 3)]
[InlineData(2, 2, 4)]
public void Teste_ComMultiplosDados(int a, int b, int resultado)
{
	Assert.Equal(resultado, a + b);
}
```

#### **NUnit** (Alternativa)
```xml
<PackageReference Include="NUnit" Version="4.0.1" />
<PackageReference Include="NUnit3TestAdapter" Version="4.5.1" />
```
- **Uso**: Framework de testes (mais antigo que xUnit)
- **Status**: 🟡 Similar ao xUnit, mais clássico

#### **Moq** (Mocking)
```xml
<PackageReference Include="Moq" Version="4.20.70" />
```
- **Uso**: Criar mocks para testes
- **Código**:
```csharp
var mockRepo = new Mock<IUsuarioRepositorio>();
mockRepo.Setup(x => x.Autenticar("user", "pass"))
	.Returns(new Usuario { Id = 1 });
```

#### **FluentAssertions** (Assertions melhores)
```xml
<PackageReference Include="FluentAssertions" Version="6.12.0" />
```
- **Uso**: Assertions mais legíveis
- **Código**:
```csharp
usuario.Username.Should().Be("pedro");
mensagens.Should().HaveCount(3);
```

---

### **6. SEGURANÇA** 🔐

#### **BCrypt.Net-Core** (Hash de Senha)
```xml
<PackageReference Include="BCrypt.Net-Core" Version="1.0.0" />
```
- **Uso**: Hash seguro de senha (futura implementação)
- **Documentação**: https://github.com/neoKbryXor/BCrypt.Net-Core
- **Status**: 🟡 Para Fase 4 (Segurança)
- **Código**:
```csharp
// Hash
string hash = BCrypt.Net.BCrypt.HashPassword(senha);

// Verificar
bool valido = BCrypt.Net.BCrypt.Verify(senha, hash);
```

#### **System.Security.Cryptography** (Já incluído)
```xml
<!-- Já vem com .NET -->
```
- **Uso**: Criptografia de dados
- **Status**: ✅ Disponível se necessário

---

### **7. UTILITIES & HELPERS** 🔧

#### **Humanizer** (Formatação de dados)
```xml
<PackageReference Include="Humanizer" Version="2.14.1" />
```
- **Uso**: Formatar datas, strings de forma legível
- **Status**: 🟡 Opcional (nice-to-have)
- **Código**:
```csharp
DateTime.UtcNow.AddDays(-5).Humanize(); // "5 days ago"
```

#### **CommunityToolkit.Mvvm** (MVVM - UI pattern)
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
```
- **Uso**: MVVM pattern para Windows Forms (avançado)
- **Status**: ❌ Não necessário para este projeto
- **Razão**: Windows Forms puro é mais simples

---

### **8. PERFORMANCE & PROFILING** ⚡

#### **BenchmarkDotNet** (Benchmarking)
```xml
<PackageReference Include="BenchmarkDotNet" Version="0.13.10" />
```
- **Uso**: Medir performance de código
- **Status**: 🟡 Para otimizações futuras
- **Código**:
```csharp
[MemoryDiagnoser]
public class MyBenchmark
{
	[Benchmark]
	public void TestePerformance()
	{
		// Código a medir
	}
}
```

---

## 📊 Resumo: O que Instalar

### **Essencial (Já incluído no projeto)**
```xml
<ItemGroup>
	<!-- Rede -->
	<PackageReference Include="System.Net.Sockets" Version="4.3.0" />

	<!-- EF Core + SQLite -->
	<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />

	<!-- JSON -->
	<!-- System.Text.Json já incluído no .NET -->
</ItemGroup>
```

### **Recomendado para Adicionar**
```xml
<ItemGroup>
	<!-- Logging -->
	<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
	<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />

	<!-- Testes -->
	<PackageReference Include="xunit" Version="2.6.6" />
	<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
	<PackageReference Include="Moq" Version="4.20.70" />
	<PackageReference Include="FluentAssertions" Version="6.12.0" />
</ItemGroup>
```

### **Futuro (Fase 4+)**
```xml
<ItemGroup>
	<!-- Segurança -->
	<PackageReference Include="BCrypt.Net-Core" Version="1.0.0" />

	<!-- Utils -->
	<PackageReference Include="Humanizer" Version="2.14.1" />
</ItemGroup>
```

---

## 🚫 O Que NÃO Usar

| Biblioteca | Por quê | Alternativa |
|-----------|---------|------------|
| **Polly** | Abstrai retry/circuit breaker demais | Task/async nativo |
| **RestSharp** | Abstrai HTTP, não TCP puro | HttpClient ou Sockets direto |
| **SignalR** | Overkill, abstrai tudo | Sockets TCP puro |
| **gRPC** | Muito complexo, abstrai protobuf | JSON + TCP puro |
| **MassTransit** | Message bus, desnecessário | Comunicação ponto-a-ponto |
| **Castle.DynamicProxy** | Intercepção complexa, não necessária | Interfaces simples |

---

## 📦 Como Instalar Pacotes

### **Via Package Manager Console**
```powershell
Install-Package Microsoft.Extensions.Logging
Install-Package xunit
Install-Package Moq
```

### **Via Comando no Terminal**
```bash
dotnet add package Microsoft.Extensions.Logging
dotnet add package xunit
```

### **Editar .csproj Diretamente**
```xml
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
```

---

## ✅ Checklist de Dependências do Projeto

- [x] System.Net.Sockets (TCP/UDP puro)
- [x] System.Net.Primitives (IP, EndPoint)
- [x] System.Text.Json (Serialização)
- [x] Microsoft.EntityFrameworkCore
- [x] Microsoft.EntityFrameworkCore.Sqlite
- [ ] Microsoft.Extensions.Logging (recomendado)
- [ ] xunit (recomendado para testes)
- [ ] Moq (recomendado para mocks)
- [ ] FluentAssertions (recomendado)

---

## 🎓 Conclusão

O projeto **usa apenas bibliotecas nativas .NET** para networking TCP. Não há abstrações que escondam os sockets.

**Para adicionar mais funcionalidades**, consulte esta lista antes de instalar novos pacotes!

**Dúvida?** Revisar a documentação oficial da Microsoft:
- https://docs.microsoft.com/en-us/dotnet/
- https://github.com/dotnet/runtime
