## #2 - Mudando a arquitetura
### CRUD - PostgreSQL - MediatR - CQRS

Repositório Github:  [Gameapp - Github](https://github.com/GMessias/Gameapp)

[Começo](#Começo)
[Gameapp.Api](#Gameapp.Api)
[Gameapp.Application](#Gameapp.Application)
[Handler](#Handler)
[Infrastructure](#Infrastructure)
[UpdateHandler](#UpdateHandler)
[Final](#Final)

### Começo

Acrescentado mais um project, Domain.
Apenas **Application** adiciona a referência para **Domain**.

**Gameapp.Application.csproj**
```csproj
<ItemGroup>
  <ProjectReference Include="..\Gameapp.Domain\Gameapp.Domain.csproj" />
</ItemGroup>
```

O fluxo está diferente, antes tinham interfaces e implementações de Services e Repositories. Em que funcionava praticamente assim (a grosso modo):

> Controller -> IService -> Service -> IRepository -> Repository -> Context -> Banco de dados.

Agora está assim:

> Controller -> CommandHandler -> IRepository -> Repository -> Context -> Banco de dados.
> Controller -> QuerieHandler -> IContext -> Context -> Banco de dados.

A interface do context, serve para que a Application consiga chamar o context para as queries sem precisar passar pelos repositórios.

A entidade criada antes foi passada para o projeto **Domain**.
#### **Os arquivos apresentados não estarão em ordem de modificação e criação.**

### Gameapp.Api

Gameapp.Api, por usar a biblioteca do MediatR, eu preciso alterar nos controllers:

**ItemController.cs**
```cs
private readonly IMediator _mediator;

public ItemController(IMediator mediator)
{
    _mediator = mediator;
}
```

Para você utilizar o **MediatR** chama o método **.Send(request)** em que o parâmetro costuma ser o **Request** ou **Command** que é um objeto. Servindo tanto para **Create**, **Update**, **Delete** e **Querie**. A biblioteca vai identificar qual o **Handle** adequado para aquela situação.

```cs
[HttpGet("{id}")]
public async Task<ActionResult<Item>> GetById(Guid id)
{
    Item? item = await _mediator.Send(new GetItemByIdQuery { Id = id });

    if (item == null)
    {
        return NotFound();
    }

    return Ok(item);
}

[HttpPost]
public async Task<ActionResult<Item>> Create(CreateItemCommand command)
{
    Item item = await _mediator.Send(command);

    return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
{
    await _mediator.Send(new DeleteItemCommand { Id = id });

    return NoContent();
}
```

### Gameapp.Application

Há alguns objetos novos, eles foram adicionados no **Gameapp.Application**. Por exemplo o **CreateItemCommand.cs**
**GetItemByIdQuery.cs**

É comum elas terem propriedades iguais a entidade porém não na mesma quantidade. No nosso exemplo a diferença:

Entidade: **Item.cs**
```cs
public class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set;}
    public DateTime UpdatedDate { get; set;}
}
```

Command: **CreateItemCommand.cs**
```cs
public class CreateItemCommand : IRequest<Item>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

Querie: **GetItemByIdQuery.cs**
```cs
public class GetItemByIdQuery : IRequest<Item>
{
    public Guid Id { get; set; }
}
```

A herança **IRequest** é da biblioteca **MediatR**, e quando tem a entidade como valor de T. Indica que o retorno do **handler** será aquele objeto.

Para funcionar também é necessário colocar na classe: 

**InjectionDependency.cs**
```cs
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
```

Note que o 
>typeof(DependencyInjection)

se refere **DependencyInjection.cs** do project de **Application**, onde está meus Commands, Queries e Handlers que o MediatR é responsável de gerenciar.

Costuma-se para organizar as pastas e objetos assim para nosso caso:
* Gameapp.Application
	* Features
		* Items
			* Commands
				* CreateItem
					* CreateItemCommand.cs
					* CreateItemCommandHandler.cs
				* ...
			* Queries
				* GetItemById
					* GetItemByIdQuery.cs
					* GetItemByIdQueryHandler.cs

No começo pelo menos para mim, dava uma sensação estranha o tanto de pasta e tantas divisões. Mas acostuma e fica melhor, acredite.

### Handler

Agora exemplicando, baseando no objeto criado acima.

**CreateItemCommandHandler.cs**
```cs
public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Item>
{
    private readonly IMapper _mapper; 
    private readonly IItemRepository _itemRepository;

    public CreateItemCommandHandler(IMapper mapper, IItemRepository itemRepository)
    {
        _mapper = mapper;
        _itemRepository = itemRepository;
    }

    public async Task<Item> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Validar o request

        Item item = _mapper.Map<Item>(request);
        item.CreatedDate = DateTime.UtcNow;

        item = await _itemRepository.CreateAsync(item);

        return item;
    }
}
```

Nome do método ser **Handle**, é por causa de cumprir o contrato da interface **IRequestHandler**, CreateItemCommand é mostrando o objeto de entrada como parâmetro, mas pode ser um propriedade simples como Int, Double, Guid, String etc. E o Item como segundo parâmetro é o retorno desse handler.

**GetItemByIdQueryHandler.cs**
```cs
public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Item>
{
    private readonly IApplicationDbContext _context;

    public GetItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Item> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        Item? item = await _context.Items.FindAsync(request.Id);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        return item;
    }
}
```

A diferença do querie é não passar por repositório. Então criei o **IApplicationDbContext** numa pasta chamada **Contracts** no projeto **Gameapp.Application**.

**IApplicationDbContext.cs**
```cs
public interface IApplicationDbContext
{
    DbSet<Item> Items { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

### Infrastructure

Atualizei o meu arquivo **GameContext** na **Gameapp.Infrastructure**.

**GameContext.cs**
```cs
public class GameContext : DbContext, IApplicationDbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
```

E o **NotFoundException** é uma nova classe que criei para fazer esse tratamento de quando não achar o item na query. Criei na pasta Items, dentro da pasta **Exceptions** em **Gameapp.Application**.

**NotFoundException.cs**
```cs
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
```

Modifiquei o repositório, para que não tenha mais o método de Get, e só coloquei **operações que alteram o status do banco de dados**.

**ItemRepository.cs**
```cs
public async Task<Item> CreateAsync(Item item)
{
    await _context.Items.AddAsync(item);
    await _context.SaveChangesAsync();

    return item;
}

public async Task UpdateAsync(Item item)
{
    _context.Items.Update(item);
    await _context.SaveChangesAsync();
}

public async Task DeleteAsync(Item item)
{
    _context.Items.Remove(item);
    await _context.SaveChangesAsync();
}
```

### UpdateHandler

Uma observação, o Delete e Update precisa acessar o item no banco para verificar existência e assim continuar a operação, então para reaproveitar o código, eu reutilizo o Handle de Query do Item.

**UpdateItemCommandHandler.cs**
```cs
public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandHandler(IMapper mapper, IMediator mediator, IItemRepository itemRepository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _itemRepository = itemRepository;
    }

    public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        // Validar

        Item item = await _mediator.Send(new GetItemByIdQuery { Id = request.Id }, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        item.Name = string.IsNullOrEmpty(request.Name) ? item.Name : request.Name;
        item.Description = string.IsNullOrEmpty(request.Description) ? item.Description : request.Description;
        item.UpdatedDate = DateTime.UtcNow;

        await _itemRepository.UpdateAsync(item);

        return Unit.Value;
    }
}
```

### Final

Pontos interessantes:
* Utilizo **IMediator** para chamar o handle da query.
* Uma struct chamada de **Unit**, que representa o **void**. Ela pertence a biblioteca do MediatR.

E assim concluo. Obrigado!!