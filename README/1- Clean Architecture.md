## #1 - Início
### CRUD - PostgreSQL - Controllers - Clean Architectures

Só para contextualizar, eu tinha certos problemas para criar essas soluções quando envolvia mais projetos.  E quando buscava a respeito tinha certa dificuldade para entender o que as pessoas estavam fazendo e tal. Espero que eu possa ajudar você nem que seja uma pequena parte desse arquivo todo. 

Se quiser pular tudo e ir direto pro repositório do Github: [Gameapp - Github](https://github.com/GMessias/Gameapp)

[Começo](#Começo)
[New Solution](#NewSolution)
[Pastas](#Pastas)
[EF Core](#EFCore)
[Startup](#Startup)
[AutoMapper](#AutoMapper)
[Repositório](#Repositório)
[Serviço](#Serviço)
[Controller](#Controller)
[Conteúdos](#Conteúdos)
[Agradecimentos](#Agradecimentos)

### Começo

* Eu utilizo [Visual Studio Community]([Visual Studio 2022 Community Edition – Baixe a Versão Gratuita Mais Recente (microsoft.com)](https://visualstudio.microsoft.com/pt-br/vs/community/))
* Instalei o [PostgreSQL 16 para Windows]([PostgreSQL: Downloads](https://www.postgresql.org/download/))

Na instalação do executável do PostgreSQL aparece algumas coisas para marcar e configurar, e normalmente fazem assim:
Instalar:
* PostgreSQL Server
* pgAdmin 4
* Stack Builder
* Command Line Tools

Fica estabelecido: Database superuser "postgres", padrão, mas você pode alterar da sua forma:
* Password: postgres
* Port: 5432
* Locale: Portuguese, Brazil

### NewSolution

Abrindo Visual Studio: **New Solution** -> **Blank Solution**
Criar as pastas:
* src
* tests

Dentro de **src**:
* New Project -> ASP.NET Core Web API
	* .NET 7
	* Configure Https
	* Use Controllers
	* Enable OpenAPI Support
* New Project -> Class Library: Application .Net 7
* New Project -> Class Library: Infrastructure .Net 7

Obs: a nomenclatura costuma seguir essa forma 'NomeDaSolucao.NomeDoProjetoCriado'. Exemplo:
* Gameapp.Api
* Gameapp.Application
* Gameapp.Infrastructure

Necessário colocar as referências dos projetos.
Visual Studio tem a opção de colocar, mas você também pode colocar no arquivo do .csproj:
**Gameapp.Api.csproj**
```csproj
<ItemGroup>
  <ProjectReference Include="..\Gameapp.Application\Gameapp.Application.csproj" />
  <ProjectReference Include="..\Gameapp.Infrastructure\Gameapp.Infrastructure.csproj" />
</ItemGroup>
```
**Gameapp.Application.csproj**
Não possui referência, algumas soluções as pessoas adicionam o Project Domain, nesse caso o Application faria referência a ela.

**Gameapp.Infrastructure.csproj**
```csproj
<ItemGroup>
  <ProjectReference Include="..\Gameapp.Application\Gameapp.Application.csproj" />
</ItemGroup>
```

### Pastas

Criando algumas pastas, lembrando que é uma ideia que adotei, outras pessoas e empresas organizam de outras formas:
Gameapp.Api:
* Controllers

Gameapp.Application:
* Dtos
* Enums
* Interfaces
	* Repositories
	* Services
* Mappings
* Models
* Services

Gameapp.Infrastructure (com o EF Core que é a biblioteca para o banco de dados, vai ser criado uma pasta chamada Migrations, deixemos com a biblioteca essa parte):
* Data
* Repositories

### EFCore

Criando o arquivo **Model**: 
**Item.cs**
```cs
namespace Gameapp.Application.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnumItemType ItemType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```
Criando o arquivo **Enum**, que está na quarta propriedade do **Item.cs**: 
**EnumItemType.cs**
```cs
namespace Gameapp.Application.Enums;

public enum EnumItemType
{
    Equipment,
    Consumables,
    Materials,
    Quests
}
```

Instalando as bibliotecas do **EF Core** no projeto da **Infrastructure**, você pode instalar pelo **Nuget** ou **linha de comando**:
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Design
* Microsoft.EntityFrameworkCore.Tools
* Npgsql.EntityFrameworkCore.PostgreSQL
O EF Core da suporte a vários outros banco de dados.

Criando o arquivo do **DbContext** que fica na pasta **Data**: 
**GameContext.cs**
```cs
namespace Gameapp.Infrastructure.Data;

public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
}
```

Quando utiliza os comandos do **Microsoft.EntityFrameworkCore.Tools**, no console "**Package Manager Console**" para fazer as Migrations, Update, Database etc, ele pode reclamar, um erro comum de acontecer e que costuma resolver com isto:
([Design-time DbContext Creation - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli))
Resolvendo de acordo com o link acima:
Criar um arquivo de **Factory** para o **DbContext** na pasta **Data**: 
**GameContextFactory.cs**
```cs
namespace Gameapp.Infrastructure.Data;

public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
        optionsBuilder.UseNpgsql("Host=localhost; Database=gamedb; Username=postgres; Password=postgres");

        return new GameContext(optionsBuilder.Options);
    }
}
```
Obs: Coloquei a string de conexão do banco de dados direto no parâmetro do método. **NÃO** é uma boa prática... **UMA FORMA** de melhorar seria assim, na mesma classe modifique para que fique assim:
```cs
namespace Gameapp.Infrastructure.Data;

public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
        
        string currentDirectory = Directory.GetCurrentDirectory();
        string parentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? string.Empty;
        string basePath = args.Length > 0 ? Path.Combine(parentDirectory, args[0]) : currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("GameDatabase");
        optionsBuilder.UseNpgsql(connectionString);

        return new GameContext(optionsBuilder.Options);
    }
}
```

Esse trecho:
```cs
var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();
```
**SetBasePath** precisa de uma biblioteca: 
> Microsoft.Extensions.Configuration.FileExtensions

**AddJsonFile** precisa de outra biblioteca: 
> Microsoft.Extensions.Configuration.Json

Adicionando a **Connection String**, no arquivo de **appsettings.json** do **Project Gameapp.Api**:
**appsettings.json**
```json
{
  "ConnectionStrings": {
    "GameDatabase": "Host=localhost; Database=gamedb; Username=postgres; Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Até aqui, da para criar as Migrations e assim criar o banco de dados com a tabela de acordo com nosso Model.
Usando o **Package Manager Console**, utilize esses comandos:
Você precisa entrar no diretório do projeto que possui o **Context** do banco de dados, ou você também pode adicionar no comando '--project NomeDoProjeto'. Fazendo do primeiro jeito:
> cd Infrastructure

Criar as **Migrations**:
> dotnet ef migrations add InitialCreate

Criar **o banco de dados** de acordo com a **Migration** (o primeiro é sem argumento, em que alguns casos são suficiente para criar o banco de dados):
> dotnet ef database update
> dotnet ef database update -- ".\\Gameapp.Api"

Comando de 'database update', '--' e o caminho '.\\Gameapp.Api' é um identificador que estou **passando um argumento**, note que no parâmetro da classe possui '**string [] args**':

### Startup

Agora criando o arquivo **Startup.cs** e modificando o **Program.cs**, porque ao navegar pela internet buscando algumas soluções você vai se deparar com essas coisas diferentes visualmente. Lembrando que isso é no **Project Gameapp.Api**.
Criando o arquivo:
**Startup.cs**
```cs
namespace Gameapp.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
                .AddApplication()
                .AddInfrastructure(Configuration);

        services.AddControllers();

        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Game API",
                Version = "v1"
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(swaggerUiOptions => swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Game API"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}

```
Esse trecho:
```cs
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddApplication()
        .AddInfrastructure(Configuration);
```
Não vai deixar você compilar a solução, porque são métodos que vamos criar ainda no projeto **Gameapp.Application** e **Gameapp.Infrastructure**, essas práticas é de **Injeção de Dependência**, agorinha criaremos.

Modificar o arquivo **Program.cs** que já é criado na criação do Project da Api:
**Program.cs**
```cs
namespace Gameapp.Api;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}

```

No projeto **Gameapp.Infrastructure**, vamos criar o arquivo **DependencyInjection.cs**:
**DependencyInjection.cs**
```cs
namespace Gameapp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("GameDatabase");

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<GameContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IItemRepository, ItemRepository>();

        return services;
    }
}
```
Esse trecho abaixo da connectionString é importante pois estamos utilizando propriedades que vão ser armazenadas do tipo DateTime, e o PostgreSQL nessa versão reclama de UTC etc. A solução para isto:
```cs
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```
O **IItemRepository** e **ItemRepository**, são arquivos que vamos criar ainda, uma é interface (inicia o nome com 'I' e depois o nome do repositório) e o outro é o repositório.
```cs
services.AddScoped<IItemRepository, ItemRepository>();
```

E agora no projeto **Gameapp.Application**, vamos criar o arquivo **DependencyInjection.cs**:
**DependencyInjection.cs**
```cs
namespace Gameapp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
```
O AutoMapper, é uma biblioteca que utilizaremos para converter Dto para Model e de Model para Dto, adicionaremos ela daqui a pouco após criar o Dto do Item. O trecho que aparece:
```cs
services.AddAutoMapper(typeof(AutoMapperProfile));
```

Esses dois arquivos em projetos separados, **necessitam de um pacote para funcionar**, você pode instalar no Project **Gameapp.Application** e a **Infrastructure** fazer referência de lá:
> Microsoft.Extensions.DependencyInjection.Abstractions

### AutoMapper

Criando o arquivo **Dto**: 
**ItemDto.cs**
```cs
namespace Gameapp.Application.Dtos;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EnumItemType ItemType { get; set; }
}
```
Obs: Particularmente não quero exibir informações das propriedades que possuem os DateTime do Model. Por isso tirei, mas é aquela coisa... cada caso um caso, não é uma regra.

Agora vamos instalar a biblioteca **AutoMapper** e fazer suas configurações para funcionar.
Instalar a biblioteca no **Project Gameapp.Application**:
> AutoMapper.Extensions.Microsoft.DependencyInjection

Criando o arquivo de **Profile** na pasta **Mappings**, para que possamos usar o **AutoMapper**: **AutoMapperProfile.cs**
```cs
namespace Gameapp.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, Item>();
    }
}
```
Precisaria colocar esse trecho no arquivo de **DependencyInjection** do **Application**, porém já está lá. Esse trecho:
```cs
services.AddAutoMapper(typeof(AutoMapperProfile));
```

### Repositório

Criando o arquivo de **interface do repositório**: 
**IItemRepository.cs**
```cs
namespace Gameapp.Application.Interfaces.Repositories;

public interface IItemRepository
{
    Task<Item?> Get(int id);
    Task<IEnumerable<Item>> GetAll();
    Task<Item> Add(Item item);
    Task<Item?> Update(Item item);
    Task<bool> Delete(int id);
    bool ItemExists(int id);
}
```

Criando o **repositório** que vai herdar dessa interface, essa interface vai forçar essa classe implementar exatamente esses métodos, respeitando o tipo de retorno e seus parâmetros: **ItemRepository.cs**
```cs
Confira no Github, excedeu os caracteres kkkkkkkk.
```
[Gameapp - Github](https://github.com/GMessias/Gameapp)

Já foi colocado no arquivo, porém ao criar essas interfaces e repositórios precisa ser colocado no **DependencyInjection.cs** de acordo com o Projeto, nesse caso coloquei no **Gameapp.Infrastructure**.
```cs
services.AddScoped<IItemRepository, ItemRepository>();
```

### Serviço

Criando o arquivo de **interface do serviço**: 
**IItemService.cs**
```cs
namespace Gameapp.Application.Interfaces.Services;

public interface IItemService
{
    Task<ItemDto?> Get(int id);
    Task<IEnumerable<ItemDto>?> GetAll();
    Task<ItemDto> Add(ItemDto itemDto);
    Task<ItemDto?> Update(int id, ItemDto itemDto);
    Task<bool> Delete(int id);
}
```

Criando o arquivo de **serviço** que vai herdar dessa interface: 
**ItemService.cs**
```cs
Confira no Github, excedeu os caracteres kkkkkkkk.
```
[Gameapp - Github](https://github.com/GMessias/Gameapp)

Também já está no arquivo, mas segue o trecho para colocar no **DependencyInjection.cs** de **Gameapp.Application**.
```cs
services.AddScoped<IItemService, ItemService>();
```

### Controller

Criando o arquivo de **Controller**: 
**ItemController.cs**
```cs
namespace Gameapp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> Get(int id)
    {
        ItemDto? itemDto = await _itemService.Get(id);

        if (itemDto == null)
        {
            return NotFound();
        }

        return Ok(itemDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll()
    {
        IEnumerable<ItemDto>? itemList = await _itemService.GetAll();

        return Ok(itemList);
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> Add(ItemDto itemDto)
    {
        ItemDto newItem = await _itemService.Add(itemDto);

        return CreatedAtAction(nameof(Get), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ItemDto itemDto)
    {
        if (id != itemDto.Id)
        {
            return BadRequest();
        }

        ItemDto? newItem = await _itemService.Update(id, itemDto);

        if (newItem == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool success = await _itemService.Delete(id);

        if (success)
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }
}
```

Bom, com tudo isso você consegue fazer funcionar. Falta algumas coisas para dar polida, por exemplo fazer as classes de Testes(Nunit por exemplo) e fazer Validações (usando FluentValidation por exemplo), mas até aqui é só para iniciar com algumas práticas e ter alguns recursos para ir entendendo. Escrevendo isso eu queria explicar algumas linhas mas acho que ficaria muito grande este documento.

### Conteúdos

Colocarei alguns materiais e que acho importante até este ponto para incrementar  o conteúdo.
* Arquitetura: [Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
* Sobre o design-time e factory do EF Core para criar o banco de dados: [Design-time DbContext Creation - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)
* Nos DependencyInjection.cs utilizei para classe de serviços e repositórios o AddScoped, porém há mais tipos, é conhecido por Service lifetimes: [Dependency injection - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
* Swagger/Swashbuckle: [Get started with Swashbuckle and ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio)
* Controller e suas ActionResult, vejo usarem muito o Ok() sendo que seria bom usarem os outros, por exemplo Ok gera um 200, CreatedAtAction gera 201 created, e já vi vários códigos no HttpPost que colocam o Ok. De qualquer maneira, uma boa olhada aqui é legal: [Controller action return types in ASP.NET Core web API | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-7.0)

### Agradecimentos

É isto, obrigado pela atenção. Espero que eu tenha te ajudado pelo menos 1%.

[Github](https://github.com/GMessias) e meu [Linkedin](https://www.linkedin.com/in/gmessiasp/).