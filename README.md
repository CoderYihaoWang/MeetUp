# Meet up
A .net core API project following the Udemy course 'ASP.NET Core API. From scratch to master'

## Key take-aways

### Database

- Use EF Core as the ORM, override `OnModelCreating` to configure the relationships among entities
- Provide a `DbContext` class, and register each entity as a `DbSet` property
- The `DbContext` object is injected in `Startup`, add connection string by using `options.UseSqlServer({connection string})`
- Add connection string to `appsettings.json`, then retrieve it by using `Configuration.GetSection("ConnectionStrings")` or `GetConnectionString`
- Only local connection string is used, so doesn't matter. Override this connection string when deploying to Azure
- Don't directly use entity objects. Create models(XXXDtos) to represent input and output data. Use AutoMapper for conversion


### Controllers 

-  Linq + automapper for CRUD. Derive `ControllerBase` to use methods like `Ok`, `Created`, `StatusCode`, `JsonResult`
- Use `ModelState` object to check the model binding process
- 

### Authentication & authorization


### Filters and utils

- For logging, can use NLogger, inject it directly on web host builder (Note `ILogger` is generic)


### Deployment


### General

- Register DI services in `Startup.ConfigureServices`
- Configure app running procedure(middlewares) in `Startup.Configure`



