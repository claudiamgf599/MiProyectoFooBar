# Arquitectura del Proyecto FooBar — Comparación .NET vs Java

> Guía de estudio para desarrolladores Java que se están migrando a .NET

---

## Tabla de Contenido

1. [Resumen General](#resumen-general)
2. [Estructura de Capas](#estructura-de-capas)
3. [Patrón: Puertos y Adaptadores (Hexagonal/Clean Architecture)](#patrón-puertos-y-adaptadores)
4. [Patrón: CQRS](#patrón-cqrs)
5. [MediatR vs Java Equivalente](#mediatr-vs-java-equivalente)
6. [Entity Framework Core vs JPA/Hibernate](#entity-framework-core-vs-jpahibernate)
7. [Dapper vs Java Equivalente](#dapper-vs-java-equivalente)
8. [Minimal APIs vs Spring MVC](#minimal-apis-vs-spring-mvc)
9. [Inyección de Dependencias](#inyección-de-dependencias)
10. [FluentValidation vs Bean Validation](#fluentvalidation-vs-bean-validation)
11. [Serilog vs SLF4J/Logback](#serilog-vs-slf4jlogback)
12. [Health Checks vs Spring Boot Actuator](#health-checks-vs-spring-boot-actuator)
13. [Automapper vs ModelMapper/MapStruct](#automapper-vs-modelmappermapstruct)
14. [Migraciones vs Flyway/Liquibase](#migraciones-vs-flywayliquibase)
15. [Testeos vs JUnit/Mockito](#testeos-vs-junitmockito)
16. [Docker/Kubernetes](#dockerkubernetes)
17. [Glosario .NET → Java](#glosario-net--java)

---

## Resumen General

FooBar es una plantilla de microservicio REST construida con **.NET 8** que implementa tres patrones arquitectónicos combinados:

| Patrón | Propósito |
|--------|-----------|
| **Puertos y Adaptadores** (Hexagonal/Clean/Onion) | Aislar el dominio de la tecnología |
| **CQRS** | Separar consultas (read) de comandos (write) |
| **Clean Architecture** | Dependencias apuntan siempre hacia adentro |

### Equivalente en el ecosistema Java

| Concepto | .NET (FooBar) | Java equivalente |
|----------|---------------|------------------|
| Framework web | ASP.NET Core (Minimal APIs) | Spring Boot (Spring MVC) |
| ORM (Write) | Entity Framework Core | JPA/Hibernate |
| Micro-ORM (Read) | Dapper | jOOQ / MyBatis |
| CQRS handlers | MediatR | Implementación manual o MicroProfile Contexts & Dependency Injection |
| Validación | FluentValidation | Jakarta Bean Validation (Hibernate Validator) |
| Logging | Serilog | SLF4J + Logback |
| Health checks | Microsoft.Extensions.Diagnostics.HealthChecks | Spring Boot Actuator |
| Mapper | AutoMapper | MapStruct / ModelMapper |
| DI Container | Built-in (IServiceCollection) | Spring Container / Micronaut |
| Testing | xUnit + NSubstitute | JUnit 5 + Mockito |
| CI/CD | Azure Pipelines | GitHub Actions / GitLab CI |

---

## Estructura de Capas

```
FooBar/
├── FooBar.Api/                          ← Capa de Presentación (Adaptadores de Entrada)
├── FooBar.Api.Tests/                    ← Pruebas de Integración
├── FooBar.Application/                  ← Capa de Aplicación (Orquestación, CQRS Handlers)
├── FooBar.Architecture.Tests/           ← Pruebas de Arquitectura (reglas de capas)
├── FooBar.Domain/                       ← Capa de Dominio (Entidades, Value Objects, Servicios)
├── FooBar.Domain.Tests/                 ← Pruebas Unitarias de Dominio
└── FooBar.Infrastructure/               ← Capa de Infraestructura (Adaptadores de Salida)
```

### Comparación con estructura Java (Spring Boot + Clean Architecture)

```
FooBar/
├── FooBar.Api/                          ← src/main/java/com/foobar/presentation/
│   ├── controllers/                     ← @RestController (Spring MVC)
│   ├── filters/                         ← Filter/Interceptor
│   └── middleware/                      ← ExceptionHandler (global)
├── FooBar.Application/                  ← src/main/java/com/foobar/application/
│   ├── command/                         ← Command objects + CommandHandler
│   ├── query/                           ← Query objects + QueryHandler
│   └── ports/                           ← Interfaces de dominio expuestas
├── FooBar.Domain/                       ← src/main/java/com/foobar/domain/
│   ├── common/                          ← Entidad base, validadores
│   ├── invoices/                        ← Aggregate: Invoice, ProductInvoice
│   ├── customers/                       ← Aggregate: Customer
│   ├── products/                        ← Aggregate: Product
│   └── exceptions/                      ← Domain exceptions
└── FooBar.Infrastructure/               ← src/main/java/com/foobar/infrastructure/
    ├── adapters/                        ← Repository implementations
    ├── datasource/                      ← DbContext (EF Core) / EntityManager
    ├── mappers/                         ← AutoMapper / MapStruct
    └── config/                          ← @Configuration classes
```

---

## Patrón: Puertos y Adaptadores

### ¿Qué es?

La **capa de Dominio** está en el centro y **no conoce** nada externo. Las interfaces (puertos) se definen en Dominio, y las implementaciones (adaptadores) van en Infraestructura.

### En FooBar

```
┌─────────────────────────────────────────────────────┐
│                  FooBar.Api                         │  ← Adaptadores de ENTRADA (HTTP)
│  (HTTP endpoints → MediatR → Commands/Queries)     │
└────────────────────┬────────────────────────────────┘
                     │ llama a
┌────────────────────▼────────────────────────────────┐
│           FooBar.Application                        │  ← Puertos + Handlers
│  • Interfaces (puertos): IInvoiceRepository         │
│  • Commands: InsertInvoiceCommand                   │
│  • Queries: GetInvoiceByIdQuery                     │
│  • Handlers: InsertInvoiceHandler                   │
└────────────────────┬────────────────────────────────┘
                     │ usa
┌────────────────────▼────────────────────────────────┐
│              FooBar.Domain                          │  ← CORAZÓN (sin dependencias externas)
│  • Entities: Invoice, Customer, Product             │
│  • Value Objects: InvoiceState, TypeCustomer        │
│  • Domain Services: CancelInvoiceService            │
│  • Puertos (interfaces): IInvoiceRepository         │
│  • Excepciones de dominio: CoreBusinessException    │
└────────────────────┬────────────────────────────────┘
                     │ implementa
┌────────────────────▼────────────────────────────────┐
│          FooBar.Infrastructure                      │  ← Adaptadores de SALIDA
│  • EF Core: DataContext, GenericRepository          │
│  • Dapper: InvoiceSimpleQueryRepository             │
│  • UnitOfWork                                       │
└─────────────────────────────────────────────────────┘
```

### Equivalente Java (Spring Boot)

```java
// PUERTO (interface) — en capa domain
public interface InvoiceRepository {
    Invoice findById(Guid id);
    void save(Invoice invoice);
}

// ENTIDAD DE DOMINIO — en capa domain
public class Invoice {
    private Guid id;
    private Customer customer;
    private List<ProductInvoice> products;
    
    public void cancel() {
        if (this.state == InvoiceState.CANCELED) {
            throw new CoreBusinessException("Ya está cancelada");
        }
        this.state = InvoiceState.CANCELED;
    }
}

// ADAPTADOR — en capa infrastructure
@Repository
public class InvoiceRepositoryJpa implements InvoiceRepository {
    private final InvoiceEntityJpa entityRepository;
    
    @Override
    public Invoice findById(Guid id) {
        return entityRepository.findById(id).map(toDomain).orElse(null);
    }
}

// COMMAND HANDLER — en capa application
@Service
public class InsertInvoiceHandler implements CommandHandler<InsertInvoiceCommand, Guid> {
    private final InvoiceRepository repository;
    private final UnitOfWork unitOfWork;
    
    @Override
    public Guid handle(InsertInvoiceCommand cmd) {
        var invoice = factory.create(cmd);
        repository.save(invoice);
        unitOfWork.commit();
        return invoice.getId();
    }
}
```

### Regla de oro (igual en .NET y Java)

> **Las flechas de dependencia apuntan SIEMPRE hacia adentro.**
> - `Api` → depende de → `Application`
> - `Application` → depende de → `Domain`
> - `Infrastructure` → depende de → `Domain` + `Application`
> - `Domain` → NO depende de NADA del proyecto

En .NET esto se logra con `<ProjectReference>` en los `.csproj`. En Java, con módulos (JPMS) o convenciones de paquetes.

---

## Patrón: CQRS

### ¿Qué es?

**Command Query Responsibility Segregation**: separar los modelos de lectura (Query) de los de escritura (Command).

### En FooBar

| Tipo | Ejemplo | Tecnología |
|------|---------|------------|
| **Command** (escribe) | `InsertInvoiceCommand`, `CancelInvoiceCommand` | EF Core (SaveChanges) |
| **Query** (lee) | `GetInvoiceByIdQuery`, `GetAllCancelQuery` | Dapper (SQL directo) |

#### Ejemplo de Command (escritura)

```csharp
// Command: el mensaje
public record InsertInvoiceCommand(
    Guid CustomerId,
    IEnumerable<ProductInvoiceCommand> ProductsInvoice
) : IRequest<Guid>;

// Handler: la lógica de aplicación
internal class InsertInvoiceHandler(
    InvoiceFactory factory,
    InsertInvoiceService domainService,
    IUnitOfWork unitOfWork
) : IRequestHandler<InsertInvoiceCommand, Guid>
{
    public async Task<Guid> Handle(InsertInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await factory.CreateAsync(request);
        var invoiceId = await domainService.ExecuteAsync(invoice);
        await unitOfWork.SaveAsync();
        return invoiceId;
    }
}
```

#### Ejemplo de Query (lectura)

```csharp
// Query: el mensaje de lectura
public record GetInvoiceByIdQuery(Guid id) : IRequest<InvoiceDto>;

// Handler: usa repositorio específico + AutoMapper
internal class GetInvoiceByIdHandler(
    IInvoiceRepository invoiceRepository,
    IMapper mapper
) : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(
            request.id, 
            "ProductsInvoice,Customer,ProductsInvoice.Product"
        );
        return mapper.Map<InvoiceDto>(invoice);
    }
}
```

#### Query optimizada con Dapper

```csharp
// Para lecturas simples, no se necesita mapear toda la entidad
[Repository]
public class InvoiceSimpleQueryRepository(DataContext dataContext) 
    : IInvoiceSimpleQueryRepository
{
    private IDbConnection DbConnection => dataContext.Database.GetDbConnection();

    public async Task<IEnumerable<SummaryInvoiceDto>> GetAllCancelAsync()
    {
        return await DbConnection.QueryAsync<SummaryInvoiceDto>(
            @"SELECT id, ValueTotal, State FROM Invoice WHERE State = @State",
            new { State = InvoiceState.Canceled }
        );
    }
}
```

### Equivalente Java

| .NET | Java |
|------|------|
| `IRequest<TResponse>` | Interfaz genérica `Command<TRequest, TResponse>` / `Query<TRequest, TResponse>` |
| `IRequestHandler<TRequest, TResponse>` | `@Service` clase con método `handle(TRequest)` |
| `MediatR` (registro por reflexión) | `@Service` + `@Autowired` manual o librería como **Cqrs-ee** |
| `IRequest<Guid>` | `Command<Void, Guid>` |

En Java, CQRS se implementa usualmente de forma manual:

```java
// Command
public record InsertInvoiceCommand(Guid customerId, List<ProductInvoiceCommand> products) {}

// Handler
@Service
public class InsertInvoiceHandler {
    private final InvoiceRepository repository;
    private final UnitOfWork unitOfWork;
    
    public Guid handle(InsertInvoiceCommand cmd) {
        var invoice = factory.create(cmd);
        repository.save(invoice);
        unitOfWork.commit();
        return invoice.getId();
    }
}

// Query
public record GetInvoiceByIdQuery(Guid id) {}

// Query Handler (puede usar jOOQ/Dapper-equivalente)
@Service
public class GetInvoiceByIdQueryHandler {
    private final JdbcTemplate jdbcTemplate;
    
    public InvoiceDto handle(GetInvoiceByIdQuery q) {
        return jdbcTemplate.queryForObject(
            "SELECT * FROM invoice WHERE id = ?",
            new BeanPropertyRowMapper<>(InvoiceDto.class),
            q.id()
        );
    }
}
```

### ¿Por qué CQRS?

| Beneficio | Explicación |
|-----------|-------------|
| **Separación de responsabilidades** | El modelo de lectura puede ser un DTO plano; el de escritura es una entidad rica |
| **Optimización** | Lecturas usan Dapper (SQL directo, rápido); escrituras usan EF Core (tracker de cambios) |
| **Escalabilidad** | En sistemas grandes, se pueden escalar read/write por separado |
| **Consistencia del dominio** | Las entidades de dominio mantienen su lógica de negocio sin "ensuciarse" con DTOs de presentación |

---

## MediatR vs Java Equivalente

### MediatR en .NET

MediatR es una librería que implementa el patrón **Mediator**. Los handlers se registran automáticamente por reflexión:

```csharp
// En Program.cs — registro automático
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("FooBar.Application"));
});
```

Esto escanea todo el ensamblado `FooBar.Application`, encuentra todas las clases que implementan `IRequestHandler<TRequest, TResponse>` y las registra en el DI container.

### Equivalente en Java

Java no tiene un equivalente directo de MediatR en el ecosistema Spring estándar. Opciones:

| Opción | Descripción |
|--------|-------------|
| **Manual** | `@Service` + `@Autowired` — lo más común en Spring Boot |
| **Spring Events** | `ApplicationEventPublisher` / `@EventListener` — para eventos, no para CQRS |
| **MicroProfile (Jakarta EE)** | Tiene CDI que hace inyección similar |
| **Librerías de terceros** | `cqrs-es`, `eventuate-commons` |

```java
// Equivalente manual en Spring Boot
@Service
public class InsertInvoiceHandler {
    @Autowired private InvoiceRepository repository;
    @Autowired private UnitOfWork unitOfWork;
    
    public Guid handle(InsertInvoiceCommand cmd) { ... }
}

// Controller lo invoca directamente
@RestController
public class InvoiceController {
    @Autowired private InsertInvoiceHandler handler;
    
    @PostMapping("/api/invoice")
    public ResponseEntity<Guid> create(@Valid @RequestBody InsertInvoiceCommand cmd) {
        return ResponseEntity.ok(handler.handle(cmd));
    }
}
```

### Comparación directa

| Característica | MediatR (.NET) | Spring Manual (Java) |
|---------------|----------------|---------------------|
| Registro de handlers | Automático por reflexión | Manual (@Service + @Autowired) |
| Desacoplamiento | Alto (API no conoce handlers) | Medio (controller conoce handler) |
| Testing | Fácil (mockear IMediator) | Fácil (mockear handler) |
| Curva de aprendizaje | Media | Baja |
| Performance | Ligera sobrecarga por reflexión | Directo, sin overhead |

---

## Entity Framework Core vs JPA/Hibernate

### EF Core en FooBar

```csharp
// DataContext — equivalente a @EntityManagement + @Configuration
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Auto-mapea todas las configuraciones de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        
        // Registra entidades explícitamente
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<Invoice>();
        modelBuilder.Entity<ProductInvoice>();

        // Shadow Properties: propiedades que existen en BD pero no en la entidad de dominio
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(DomainEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Crea columnas CreatedOn y LastModifiedOn automáticamente
                modelBuilder.Entity(entityType.Name)
                    .Property<DateTime>("CreatedOn");
                modelBuilder.Entity(entityType.Name)
                    .Property<DateTime>("LastModifiedOn");
            }
        }
    }
}
```

### Registro de conexión

```csharp
// Program.cs
builder.Services.AddDbContext<DataContext>(opts =>
{
    var useInMemory = config.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        opts.UseInMemoryDatabase("InMemoryDb");  // Para tests
    }
    else
    {
        opts.UseSqlServer(config.GetConnectionString("db"));  // Producción
    }
});
```

### Equivalente en Java (JPA/Hibernate)

```java
// Application.java / application.properties
@SpringBootApplication
public class FooBarApplication {
    public static void main(String[] args) {
        SpringApplication.run(FooBarApplication.class, args);
    }
}

// application.properties
spring.datasource.url=jdbc:sqlserver://localhost:1433;databaseName=foobar
spring.datasource.username=sa
spring.datasource.password=secret
spring.jpa.hibernate.ddl-auto=update
```

```java
// Entity equivalente
@Entity
@Table(name = "invoice")
public class InvoiceEntity {
    @Id
    private Guid id;
    
    @ManyToOne
    @JoinColumn(name = "customer_id")
    private CustomerEntity customer;
    
    @OneToMany(mappedBy = "invoice", cascade = CascadeType.ALL)
    private List<ProductInvoiceEntity> productsInvoice;
    
    @Column(name = "value_total")
    private BigDecimal valueTotal;
    
    @Enumerated(EnumType.STRING)
    @Column(name = "state")
    private InvoiceState state;
    
    // Shadow properties equivalent
    @Column(name = "created_on", updatable = false)
    private LocalDateTime createdOn;
    
    @Column(name = "last_modified_on")
    private LocalDateTime lastModifiedOn;
    
    @PrePersist
    protected void onCreate() {
        this.createdOn = LocalDateTime.now();
        this.lastModifiedOn = LocalDateTime.now();
    }
    
    @PreUpdate
    protected void onUpdate() {
        this.lastModifiedOn = LocalDateTime.now();
    }
}
```

### Comparación directa

| Característica | EF Core (.NET) | JPA/Hibernate (Java) |
|---------------|----------------|---------------------|
| Declaración de entidades | Clases C# con convenciones o Fluent API | `@Entity`, `@Table`, `@Column` |
| Mapeo relacional | Fluent API (`HasMany`, `WithMany`) | `@ManyToOne`, `@OneToMany` |
| Shadow Properties | `Property<DateTime>("CreatedOn")` en OnModelCreating | `@PrePersist`, `@PreUpdate` callbacks |
| Change Tracking | Automático (`ChangeTracker`) | Automatic (`EntityManager`) |
| Migrations | `dotnet ef migrations add` | Flyway o Liquibase (plugins externos) |
| In-Memory DB | `UseInMemoryDatabase()` | H2 database (`spring.datasource.url=jdbc:h2:mem:`) |
| Query language | LINQ (`query.Where(x => x.Id == id)`) | JPQL / Criteria API / Spring Data Methods |
| Micro-ORM integrado | No (usa Dapper por separado) | No (usa jOOQ/MyBatis por separado) |
| N+1 problem | `Include()`, `ThenInclude()` | `@EntityGraph`, `JOIN FETCH` |

---

## Dapper vs Java Equivalente

### Dapper en FooBar

Dapper es un **micro-ORM** ligero que mapea resultados SQL directamente a objetos:

```csharp
public class InvoiceSimpleQueryRepository(DataContext dataContext) 
    : IInvoiceSimpleQueryRepository
{
    private IDbConnection DbConnection => dataContext.Database.GetDbConnection();

    public async Task<IEnumerable<SummaryInvoiceDto>> GetAllCancelAsync()
    {
        // SQL directo → DTO plano, sin tracking, sin overhead
        return await DbConnection.QueryAsync<SummaryInvoiceDto>(
            @"SELECT id, ValueTotal, State FROM Invoice WHERE State = @State",
            new { State = InvoiceState.Canceled }
        );
    }
}
```

### Equivalente en Java

| Opción | Descripción |
|--------|-------------|
| **JdbcTemplate** (Spring) | Lo más cercano a Dapper |
| **jOOQ** | Code-first SQL, más poderoso |
| **MyBatis** | XML/annotation-based SQL mapping |

```java
// Equivalente con JdbcTemplate
@Repository
public class InvoiceSimpleQueryRepository {
    private final JdbcTemplate jdbcTemplate;
    
    public List<SummaryInvoiceDto> getAllCancelAsync() {
        String sql = "SELECT id, value_total, state FROM invoice WHERE state = ?";
        return jdbcTemplate.query(sql, 
            new BeanPropertyRowMapper<>(SummaryInvoiceDto.class),
            InvoiceState.CANCELED.name()
        );
    }
}
```

### ¿Por qué usar Dapper/JdbcTemplate en lugar de EF Core/JPA para lecturas?

| Criterio | EF Core / JPA | Dapper / JdbcTemplate |
|----------|---------------|----------------------|
| Performance | Más lento (change tracking, lazy loading) | Más rápido (mapeo directo) |
| Flexibilidad | LINQ / JPQL (no SQL nativo) | SQL nativo completo |
| Complejidad | Bajo (no escribir SQL) | Medio (escribir SQL) |
| Uso ideal | Escrituras, consultas complejas con relaciones | Lecturas simples, reportes, queries optimizadas |

---

## Minimal APIs vs Spring MVC

### Minimal APIs en FooBar

```csharp
// Program.cs — endpoints definidos como métodos estáticos
public static class InvoiceApi
{
    public static RouteGroupBuilder MapInvoice(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/", async (IMediator mediator, [Validate] InsertInvoiceCommand invoice) =>
        {
            var invoiceId = await mediator.Send(invoice);
            return Results.Ok(invoiceId);
        })
        .Produces(StatusCodes.Status201Created)
        .WithSummary("Create new invoice")
        .WithOpenApi();

        routes.MapGet("/{id}", async (IMediator mediator, Guid id) =>
        {
            return Results.Ok(await mediator.Send(new GetInvoiceByIdQuery(id)));
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get invoice by ID");

        return routes;
    }
}

// Registro en Program.cs
app.MapGroup("/api/invoice")
    .MapInvoice()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Invoices");
```

### Equivalente en Spring Boot (Java)

```java
@RestController
@RequestMapping("/api/invoice")
@Tag(name = "Invoices")
public class InvoiceController {

    @Autowired private InsertInvoiceHandler insertHandler;
    @Autowired private GetInvoiceByIdHandler getByIdHandler;
    @Autowired private CancelInvoiceHandler cancelHandler;
    @Autowired private GetAllCancelHandler getAllCancelHandler;

    @PostMapping
    @Operation(summary = "Create new invoice")
    public ResponseEntity<Guid> create(@Valid @RequestBody InsertInvoiceCommand cmd) {
        return ResponseEntity.ok(insertHandler.handle(cmd));
    }

    @GetMapping("/{id}")
    @Operation(summary = "Get invoice by ID")
    public ResponseEntity<InvoiceDto> getById(@PathVariable Guid id) {
        return ResponseEntity.ok(getByIdHandler.handle(new GetInvoiceByIdQuery(id)));
    }

    @PostMapping("/{id}/cancel")
    @Operation(summary = "Cancel an invoice")
    public ResponseEntity<Void> cancel(@PathVariable Guid id) {
        cancelHandler.handle(new CancelInvoiceCommand(id));
        return ResponseEntity.ok().build();
    }

    @GetMapping("/cancels")
    @Operation(summary = "Get canceled invoices")
    public ResponseEntity<List<SummaryInvoiceDto>> getAllCancels() {
        return ResponseEntity.ok(getAllCancelHandler.handle(new GetAllCancelQuery()));
    }
}
```

### Comparación directa

| Característica | Minimal APIs (.NET 8) | Spring MVC (Java) |
|---------------|----------------------|-------------------|
| Verbosidad | Baja (lambda expressions) | Alta (clases, annotations) |
| Routing | `MapPost`, `MapGet` con extension methods | `@PostMapping`, `@GetMapping` |
| Dependency injection | Parámetros del lambda resueltos por DI | `@Autowired` fields o constructor |
| OpenAPI/Swagger | `WithOpenApi()`, `WithSummary()` | `@Operation`, `@Tag` (Swagger annotations) |
| Grupos de endpoints | `MapGroup()` | `@RequestMapping` en controller |
| Filtros de validación | `AddEndpointFilterFactory()` | `@Valid` + `@ControllerAdvice` |
| Estilo | Functional (funcional) | Orientado a objetos |
| Curva de aprendizaje | Baja para simples, media para complejos | Media-alta por boilerplate |

### ¿Minimal APIs o Controllers?

En .NET 8, **ambos son válidos**. Minimal APIs son más concisos; Controllers son más estructurados para proyectos grandes. En Java, Spring MVC con Controllers es el estándar (aunque Spring también tiene WebFlux functional routing).

---

## Inyección de Dependencias

### DI en .NET (FooBar)

.NET tiene un DI container **integrado** en el framework:

```csharp
// Program.cs — registro de servicios
builder.Services.AddDbContext<DataContext>(opts => { ... });
builder.Services.AddMediatR(cfg => { ... });
builder.Services.AddAutoMapper(Assembly.Load("FooBar.Application"));
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddHealthChecks().AddDbContextCheck<DataContext>();

// Inyección por constructor (primary constructor syntax en C# 12)
internal class InsertInvoiceHandler(
    InvoiceFactory factory,
    InsertInvoiceService domainService,
    IUnitOfWork unitOfWork
) : IRequestHandler<InsertInvoiceCommand, Guid> { ... }

[Repository]  // Atributo personalizado para auto-registro
public class InvoiceRepository(IRepository<Invoice> invoiceRepository) 
    : IInvoiceRepository { ... }
```

### DI en Java (Spring Boot)

Spring tiene el container DI **más maduro** del ecosistema:

```java
// Registro de beans (generalmente automático con @Component)
@Service
public class InvoiceRepository implements IInvoiceRepository { ... }

// Inyección por constructor (recomendado en Spring)
@Service
public class InsertInvoiceHandler {
    private final InvoiceFactory factory;
    private final InsertInvoiceService domainService;
    private final IUnitOfWork unitOfWork;
    
    // Constructor injection (preferido sobre @Autowired field)
    public InsertInvoiceHandler(
        InvoiceFactory factory,
        InsertInvoiceService domainService,
        IUnitOfWork unitOfWork
    ) {
        this.factory = factory;
        this.domainService = domainService;
        this.unitOfWork = unitOfWork;
    }
}
```

### Comparación directa

| Característica | .NET DI | Spring DI |
|---------------|---------|-----------|
| Container | Integrado en framework (`IServiceCollection`) | Spring Context |
| Scopes | `Transient`, `Scoped`, `Singleton` | `@Scope("prototype")`, `@Scope("request")`, `@Singleton`/default |
| Registro | Manual en `Program.cs` | Automático (`@Component`, `@Service`) o manual (`@Bean`) |
| Resolución | Constructor injection (C# 12 primary constructors) | Constructor injection |
| Life-cycle por defecto | Scoped (en web) | Singleton |
| Atributos para DI | `[DomainService]`, `[Repository]` (custom) | `@Service`, `@Repository` |
| Testing | Fácil mockear interfaces | Fácil mockear interfaces (Mockito) |

---

## FluentValidation vs Bean Validation

### FluentValidation en FooBar

```csharp
// Validación fluida, fuera de las entidades
public class InsertInvoiceCommandValidator : AbstractValidator<InsertInvoiceCommand>
{
    public InsertInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ProductsInvoice).NotEmpty();
    }
}

// Se registra automáticamente:
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Se aplica en el endpoint:
[Validate]  // Custom attribute
InsertInvoiceCommand invoice
```

### Equivalente en Java (Jakarta Bean Validation)

```java
// Validación con annotations en el command
public record InsertInvoiceCommand(
    @NotNull(message = "Customer ID is required")
    Guid customerId,
    
    @NotEmpty(message = "Products are required")
    @Valid
    List<ProductInvoiceCommand> productsInvoice
) {}

// Se aplica en el controller:
@PostMapping
public ResponseEntity<Guid> create(@Valid @RequestBody InsertInvoiceCommand cmd) {
    // @Valid activa la validación automáticamente
}
```

### Comparación directa

| Característica | FluentValidation (.NET) | Bean Validation (Java) |
|---------------|------------------------|----------------------|
| Estilo | Fluent API (`RuleFor`) | Annotations (`@NotNull`) |
| Ubicación | Clase separada | En el DTO/Record |
| Validaciones complejas | Más expresiva | Requiere custom constraints |
| Integración con endpoints | `AddEndpointFilterFactory` | `@Valid` en controller |
| Validación condicional | `When()`, `Unless()` | `@ConditionalConstraint` (limitado) |
| Popularidad | Muy usado en .NET | Estándar en Java (Jakarta) |

---

## Serilog vs SLF4J/Logback

### Serilog en FooBar

```csharp
// Program.cs — configuración centralizada
builder.Host.UseSerilog((_, loggerconfiguration) =>
    loggerconfiguration
        .WriteTo.Console()        // Consola
        .WriteTo.File("logs.txt", LogEventLevel.Information));  // Archivo
```

Uso en código:

```csharp
private static readonly ILogger Log = Serilog.Log.ForContext<InsertInvoiceHandler>();

public async Task<Guid> Handle(InsertInvoiceCommand request, CancellationToken ct)
{
    Log.Information("Inserting invoice for customer {CustomerId}", request.CustomerId);
    // ... lógica
}
```

### Equivalente en Java (SLF4J + Logback)

```java
// application.properties
logging.level.root=INFO
logging.file.name=logs/app.log
logging.pattern.console=%d{yyyy-MM-dd HH:mm:ss} - %msg%n

// En el código
private static final Logger log = LoggerFactory.getLogger(InsertInvoiceHandler.class);

public Guid handle(InsertInvoiceCommand cmd) {
    log.info("Inserting invoice for customer {}", cmd.customerId());
    // ... lógica
}
```

### Comparación directa

| Característica | Serilog (.NET) | SLF4J + Logback (Java) |
|---------------|----------------|----------------------|
| Estructura | JSON by default, enriquecido | Texto plano por defecto |
| Enrichers | `ForContext<T>()` | MDC (Mapped Diagnostic Context) |
| Sink plugins | Console, File, Seq, ElasticSearch, Azure | Console, File, Logstash, ElasticSearch |
| Query logs | Serilog query language | Log files + external tools |
| Performance | Alto (asíncrono por defecto) | Alto (asíncrono configurable) |

---

## Health Checks vs Spring Boot Actuator

### Health Checks en FooBar

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>()  // Verifica conexión a BD
    .ForwardToPrometheus();            // Expone métricas

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});
```

### Equivalente en Java (Spring Boot Actuator)

```java
// application.properties
management.endpoints.web.exposure.include=health,info
management.endpoint.health.show-details=always

// Health indicator personalizado
@Component
public class CustomHealthIndicator implements HealthIndicator {
    @Override
    public Health health() {
        try {
            // verificar dependencia
            return Health.up().build();
        } catch (Exception e) {
            return Health.down(e).build();
        }
    }
}
```

### Comparación directa

| Característica | .NET HealthChecks | Spring Boot Actuator |
|---------------|-------------------|---------------------|
| Endpoint por defecto | `/healthz` (configurable) | `/actuator/health` |
| Status codes | Personalizable | 200 OK, 503 DOWN |
| Integración con k8s | Sí (readiness/liveness) | Sí (readiness/liveness) |
| Métricas | Prometheus integration | Prometheus + Micrometer |
| Check personalizados | `AddDbContextCheck<T>()` | `HealthIndicator` interface |

---

## Automapper vs MapStruct/ModelMapper

### AutoMapper en FooBar

```csharp
// Profile de mapeo
public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>();
    }
}

// Registro en Program.cs
builder.Services.AddAutoMapper(Assembly.Load("FooBar.Application"));

// Uso en handler
var invoiceDto = mapper.Map<InvoiceDto>(invoice);
```

### Equivalente en Java

| Librería | Descripción |
|----------|-------------|
| **MapStruct** | Compile-time, recomendada (genera código) |
| **ModelMapper** | Runtime, reflection-based |

```java
// MapStruct — interface (se genera implementación en compile-time)
@Mapper(componentModel = "spring")
public interface InvoiceMapper {
    InvoiceMapper INSTANCE = Mappers.getMapper(InvoiceMapper.class);
    
    @Mapping(source = "customer.name", target = "customerName")
    @Mapping(source = "productsInvoice", target = "products")
    InvoiceDto toDto(Invoice entity);
    
    List<InvoiceDto> toDtoList(List<Invoice> entities);
}

// Inyección y uso
@Service
public class GetInvoiceByIdHandler {
    private final InvoiceMapper mapper;
    
    public InvoiceDto handle(GetInvoiceByIdQuery q) {
        return mapper.toDto(invoice);  // Sin reflection overhead
    }
}
```

### Comparación directa

| Característica | AutoMapper (.NET) | MapStruct (Java) |
|---------------|-------------------|------------------|
| Implementación | Runtime (reflection) | Compile-time (genera código) |
| Performance | Bueno | Excelente (sin reflection) |
| Configuración | Profiles con `CreateMap` | Annotations en interface |
| Mapeo anidado | Automático | Requiere `@Mapping` explícito |
| Popularidad | Muy popular en .NET | Estándar en Java |

---

## Migraciones vs Flyway/Liquibase

### EF Core Migrations en .NET

```bash
# Crear migración
dotnet ef migrations add InitialCreate --project FooBar.Infrastructure

# Aplicar migraciones
dotnet ef database update --project FooBar.Infrastructure

# Ver migraciones disponibles
dotnet ef migrations list --project FooBar.Infrastructure
```

Las migraciones se almacenan en la carpeta `FooBar.Infrastructure/Migrations/`:

```
FooBar.Infrastructure/
└── Migrations/
    ├── 20240101000000_InitialCreate.cs
    ├── 20240115000000_AddInvoiceState.cs
    └── DataContextModelSnapshot.cs
```

### Equivalente en Java

| Herramienta | Descripción |
|-------------|-------------|
| **Flyway** | SQL-based, simple, muy popular |
| **Liquibase** | XML/YAML/JSON, versionado de esquemas |
| **Hibernate auto-ddl** | `spring.jpa.hibernate.ddl-auto=update` (solo dev) |

```bash
# Flyway
flyway migrate          # aplicar migraciones
flyway info             # ver estado
```

```sql
-- Flyway migration: V1__Initial_Create.sql
CREATE TABLE invoice (
    id UUID PRIMARY KEY,
    customer_id UUID NOT NULL,
    value_total DECIMAL(10,2),
    state VARCHAR(20) NOT NULL,
    created_on TIMESTAMP NOT NULL,
    last_modified_on TIMESTAMP
);
```

### Comparación directa

| Característica | EF Core Migrations | Flyway | Liquibase |
|---------------|-------------------|--------|-----------|
| Tipo | Code-first (C#) | SQL scripts | XML/YAML/JSON |
| Generación | `dotnet ef migrations add` | Manual | Generado o manual |
| Rollback | `.Undo()` method | No nativo (crear nueva migración) | Soportado |
| Versionado | `__EFMigrationsHistory` table | `flyway_schema_history` | `changelog` table |
| Multi-DB | Sí (SQL Server, PostgreSQL, SQLite) | Sí | Sí |

---

## Testeos vs JUnit/Mockito

### Testing en FooBar

```
FooBar.Domain.Tests/          ← Unit Tests de Dominio
FooBar.Api.Tests/             ← Integration Tests de API
FooBar.Architecture.Tests/    ← Pruebas de Arquitectura
```

#### Unit Test (xUnit + NSubstitute)

```csharp
// xUnit test
public class CancelInvoiceServiceTests
{
    [Fact]
    public void Cancel_ShouldThrow_WhenCustomerIsCommon()
    {
        // Arrange
        var customer = Substitute.For<Customer>();
        customer.IsCommon().Returns(true);
        var invoice = new Invoice { Customer = customer, State = InvoiceState.Active };
        
        // Act + Assert
        Assert.Throws<CoreBusinessException>(() => invoice.Cancel());
    }
}
```

#### Integration Test (xUnit + WebApplicationFactory)

```csharp
public class InvoiceApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public InvoiceApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Post_Invoice_ReturnsCreated()
    {
        var response = await _client.PostAsync(
            "/api/invoice",
            new StringContent(JsonSerializer.Serialize(command), 
                Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

### Equivalente en Java (JUnit 5 + Mockito)

```java
// Unit Test (JUnit 5 + Mockito)
class CancelInvoiceServiceTests {
    
    @Test
    void cancel_ShouldThrow_WhenCustomerIsCommon() {
        // Arrange
        var customer = mock(Customer.class);
        when(customer.isCommon()).thenReturn(true);
        var invoice = new Invoice();
        invoice.setCustomer(customer);
        invoice.setState(InvoiceState.ACTIVE);
        
        // Act + Assert
        assertThrows(CoreBusinessException.class, invoice::cancel);
    }
}

// Integration Test (Spring Boot Test)
@SpringBootTest
@AutoConfigureMockMvc
class InvoiceApiIntegrationTest {
    
    @Autowired private MockMvc mockMvc;
    
    @Test
    void post_Invoice_ReturnsCreated() throws Exception {
        mockMvc.perform(post("/api/invoice")
                .contentType(MediaType.APPLICATION_JSON)
                .content(json(command)))
            .andExpect(status().isCreated());
    }
}
```

### Comparación directa

| Característica | .NET | Java |
|---------------|------|------|
| Framework de tests | **xUnit** (o NUnit, MSTest) | **JUnit 5** |
| Mocking | **NSubstitute** (o Moq) | **Mockito** |
| Assertions | FluentAssertions | AssertJ |
| Integration Tests | `WebApplicationFactory<T>` | `@SpringBootTest` + `MockMvc` |
| Architecture Tests | Shouldly + reflection | ArchUnit |
| Cobertura | Coverlet | JaCoCo |
| Mutation Testing | **Stryker** | **PITest** |

---

## Docker/Kubernetes

### Docker en FooBar

```dockerfile
# FooBar.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FooBar.Api/FooBar.Api.csproj", "FooBar.Api/"]
COPY ["FooBar.Application/FooBar.Application.csproj", "FooBar.Application/"]
COPY ["FooBar.Domain/FooBar.Domain.csproj", "FooBar.Domain/"]
COPY ["FooBar.Infrastructure/FooBar.Infrastructure.csproj", "FooBar.Infrastructure/"]

RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "FooBar.Api.dll"]
```

### Comparación con Java

| Aspecto | .NET (FooBar) | Java (Spring Boot) |
|---------|--------------|-------------------|
| Base image | `mcr.microsoft.com/dotnet/aspnet:8.0` | `eclipse-temurin:17-jre` |
| Build tool | `dotnet publish` | `mvn package` / `gradle build` |
| JAR/WAR | DLLs en carpeta `/app` | `target/*.jar` (executable JAR) |
| Multi-stage | Sí (SDK → Runtime) | Sí (Maven/Gradle → JRE) |
| K8s manifest | Similar | Similar (no cambia por lenguaje) |

---

## Glosario .NET → Java

| Término .NET | Equivalente Java |
|-------------|-----------------|
| `project` (.csproj) | `module` / `artifact` |
| `assembly` (.dll) | `JAR` |
| `namespace` | `package` |
| `class` | `class` |
| `record` (C# 9+) | `record` (Java 16+) |
| `interface` | `interface` |
| `async/await` | `CompletableFuture` / `Reactive` / `suspend` (Kotlin) |
| `Task<T>` | `CompletableFuture<T>` / `Mono<T>` / `Publisher<T>` |
| `LINQ` | `Stream API` / `Sequence` (Kotlin) |
| `DependencyInjection` | `Spring Container` / `CDI` |
| `IServiceCollection` | `ApplicationContext` |
| `Program.cs` | `Application.java` (main method) |
| `appsettings.json` | `application.properties` / `application.yml` |
| `Middleware` | `Filter` / `Interceptor` |
| `Attribute` | `Annotation` |
| `NuGet` | `Maven` / `Gradle` |
| `dotnet new` | `archetype` (Maven) / `start.spring.io` |
| `dotnet restore` | `mvn dependency:resolve` |
| `dotnet build` | `mvn compile` |
| `dotnet test` | `mvn test` |
| `dotnet publish` | `mvn package` |
| `Entity Framework Core` | `JPA/Hibernate` |
| `DbContext` | `EntityManager` |
| `DbSet<T>` | `Repository<T>` (Spring Data) |
| `Migration` | `Flyway` / `Liquibase` |
| `Minimal API` | `@RestController` |
| `RouteGroupBuilder` | `@RequestMapping` |
| `MediatR` | Manual `@Service` |
| `FluentValidation` | `Jakarta Validation` (Bean Validation) |
| `Serilog` | `SLF4J` + `Logback` |
| `xUnit` | `JUnit 5` |
| `NSubstitute` | `Mockito` |
| `FluentAssertions` | `AssertJ` |
| `coverlet` | `JaCoCo` |
| `Stryker` | `PITest` |
| `WebApplicationFactory` | `@SpringBootTest` |
| `ILogger` | `Logger` (SLF4J) |
| `ValueTuple` | `record` / POJO |
| `Nullable reference types` | `@Nullable` / `@NonNull` (annotations) |
| `ImplicitUsings` | No equivalente (imports explícitos) |
| `Nullable enable` | `@Nullable` annotations |

---

## Diagrama Arquitectónico Comparativo

```
┌──────────────────────────────────────────────────────────────────┐
│                    FLUJO DE UNA PETICIÓN HTTP                     │
└──────────────────────────────────────────────────────────────────┘

.NET (FooBar)                              Java (Spring Boot)
═══════════════════                        ═════════════════════

HTTP Request                                 HTTP Request
    │                                            │
    ▼                                            ▼
┌─────────────┐                                ┌──────────────────┐
│ Minimal API │  (static method)               │ @RestController  │
│ InvoiceApi  │                                │ InvoiceController│
└──────┬──────┘                                └────────┬─────────┘
       │                                              │
       ▼                                              ▼
┌─────────────┐                                ┌──────────────────┐
│  [Validate] │  (Endpoint Filter)             │  @Valid          │
│  Fluent     │                                │  Bean Validation │
│ Validation  │                                └────────┬─────────┘
└──────┬──────┘                                       │
       │                                              ▼
       ▼                                    ┌──────────────────┐
┌─────────────┐                            │ Handler (Service)│
│   MediatR   │  (Mediator pattern)        │ (manual or lib)  │
│  Send()     │───────────────────────────►│                  │
└──────┬──────┘                            └────────┬─────────┘
       │                                            │
       ▼                                            ▼
┌─────────────┐                                ┌──────────────────┐
│ Application │  (Command/Query Handler)       │ Application Layer│
│   Layer     │                                │   (Service)      │
└──────┬──────┘                                └────────┬─────────┘
       │                                              │
       ▼                                            ▼
┌─────────────┐                                ┌──────────────────┐
│   Domain    │  (Entity + Domain Logic)       │   Domain Layer   │
│   Layer     │                                │  (Entity + Service)│
└──────┬──────┘                                └────────┬─────────┘
       │                                              │
       ▼                                            ▼
┌─────────────┐                                ┌──────────────────┐
│ Infrastructure │                          │ Infrastructure   │
│   (EF Core +  │                            │   (JPA +         │
│    Dapper)   │                            │    jOOQ)         │
└─────────────┘                                └──────────────────┘
       │                                              │
       ▼                                            ▼
   Database                                        Database
```

---

## Resumen: Conceptos Clave para el Migrante Java

### Lo que se siente igual

- **Inyección de dependencias** → Spring Container es más maduro, pero el concepto es idéntico
- **Separación de capas** → Mismo concepto que Clean Architecture en Java
- **CQRS** → En Java se hace manual; en .NET MediatR lo automatiza
- **Repository pattern** → Igual en ambos mundos
- **Domain-driven design** → Conceptos universales

### Lo que es diferente

| Concepto | Diferencia clave |
|----------|-----------------|
| **Minimal APIs** | No existe en Java Spring estándar (usar `@RestController`) |
| **MediatR** | En Java se implementa manualmente con `@Service` |
| **ImplicitUsings** | Java requiere imports explícitos siempre |
| **Records** | Similar a Java records (inmutables, data classes) |
| **Nullable reference types** | En Java se usa `@Nullable`/`@NonNull` annotations |
| **Migrations** | EF Core las genera desde código; en Java se usan SQL scripts (Flyway) |
| **DI por defecto** | .NET registra Scoped; Spring registra Singleton |
| **async/await** | En Java se usa `CompletableFuture` o Reactor (Mono/Flux) |
| **LINQ** | Similar a Java Stream API, pero LINQ es más rico |
| **Value types** | .NET tiene `struct`; Java solo tiene primitives (`int`, `double`) |

### Recomendaciones para aprender .NET viniendo de Java

1. **Empieza con Minimal APIs** → Son más concisos que Spring MVC, más fáciles de entender
2. **Entiende el DI container** → Es más simple que Spring, pero menos potente
3. **MediatR** → Aprende el patrón Mediator; en Java ya lo usas sin saberlo
4. **EF Core** → Es como JPA pero con LINQ (más intuitivo que JPQL)
5. **Records** → Muy similares a Java records (desde Java 16)
6. **Pattern matching** | .NET tiene pattern matching más rico que Java
7. **Span<T>** | Concepto de memoria que no existe en Java (performance)

---

*Documento generado para estudio personal. Proyecto de referencia: FooBar (.NET 8, Clean Architecture, CQRS).*