# Meet up
A .net core API project following the Udemy course 'ASP.NET Core API - from scratch to master'

## Key take-aways

### General

- Register DI services in `Startup.ConfigureServices`
- Configure app running procedure(middlewares) in `Startup.Configure`
- A general pattern: in `ConfigureServices`, use `services.AddXXX({options => ...})` to configure a service, then in `Configure`, use `app.UseXXX` to add it to the pipeline 
- `app.UseResponseCaching()` + `[ResponseCache]` for caching responses that does not change
- `app.UseStaticFiles()` to serve public static file. Files are in the `wwwroot` folder
- `app.UseCors({policy name})` to allow CORS, use `services.AddCors()` to configure
- `services.AddSwaggerGen`, `app.UseSwagger`, `app.UseSwaggerUI` to make use of swagger
- For logging, can use NLogger, inject it directly on web host builder (Note `ILogger` is generic)
- For hot reload configurations, create a controller, create an `OPTIONS` action, use `((IConfigurationRoot)_configuration).Reload()` to reload
- To add an exception filter globally, add it in `AddControllers(options => options.Filters.Add(typeof(...)))`, this supports DI
- Normally filters as attributes do not support DI, to work around this, use `[ServiceFilter(typeof(...))]`

### Database

- Use EF Core as the ORM, override `OnModelCreating` to configure the relationships among entities
- Provide a `DbContext` class, and register each entity as a `DbSet` property
- The `DbContext` object is injected in `Startup`, add connection string by using `options.UseSqlServer({connection string})`
- Add connection string to `appsettings.json`, then retrieve it by using `Configuration.GetSection("ConnectionStrings")` or `GetConnectionString`
- Only local connection string is used, so doesn't matter. Override this connection string when deploying to Azure
- Don't directly use entity objects. Create models(XXXDtos) to represent input and output data. Use `AutoMapper` for conversion

### Controllers 

-  Linq + automapper for CRUD. Derive `ControllerBase` to use methods like `Ok`, `Created`, `StatusCode`, `JsonResult`
- Use `ModelState` object to check the model binding status
- To validate input model, use validation attributes on model properties
- To customise model validation process, derive from `AbstractValidator<TDto>`, use `RuleFor` to fluently configure the requirements in the constructor. Can inject the DbContext for checking existence in database. Then, call `AddFluentValidation` after `AddController`

### Authentication & authorization

- Hash the password
- Use JWT for authentication
- To decompose code, configure jwt in appsettings.json, then map this to a JwtOptions object, inject it as a singleton
- Use `JwtSecurityToken` and `JwtSecurityTokenHandler` to generate jwt tokens to the user, through an `Ok` result
- To create a jwt token, think about attributes: issuer, expiration, key, and claims
- Use `[Authorize]` to require authentication
- Use `[Authorize]` with `Role` argument to enable role-based authorization
- Use `[Authorize]` with `Policy` argument to enable claim-based authorization, configure required claims in `services.AddAuthorization(options => options.AddPolicy({policy name}, builder => builder.RequireClaim(...)))`
- User `[Authorize]` with `Policy` argument can also enable customized authorization; to do this, create a `IAuthorizationRequirement` class and a `AuthorizationHandler<T>` class, add this requirement in `AddAuthorization`; this can be used to achieve resource-based authorization, to do this, inject the AuthorizationHandler and directly use it for authorization in action methods
- Yet another way to do authorization is by creating authorization filters, and apply it as an attribute

### Deployment

- Configure the environment variable in the Azure Web App service, such that its name is the same as the connection string name in `appsettings.json`'s `ConnectionStrings` section. This will override this value in when the app is running in cloud
- Use `{DbContext}.Database.GetPendingMigrations` to check and complete migrations



