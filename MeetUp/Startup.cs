using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Validators;
using MeetUp.Authorization;
using MeetUp.Entity;
using MeetUp.Filters;
using MeetUp.Identity;
using MeetUp.Models;
using MeetUp.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MeetUp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtOptions = new JwtOptions();
            Configuration.GetSection("jwt").Bind(jwtOptions);
            services.AddSingleton(jwtOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.JwtIssuer,
                    ValidAudience = jwtOptions.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtKey))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "English"));
                options.AddPolicy("AtLeast18", builder => builder.AddRequirements(new MinimumAgeRequirement(18)));
            });

            services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)))
                .AddFluentValidation();

            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();

            services.AddDbContext<MeetupContext>();

            services.AddScoped<MeetupSeeder>();

            services.AddAutoMapper(this.GetType().Assembly);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetupAPI", Version = "v1" });
            });

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();

            services.AddScoped<IAuthorizationHandler, ResourceOperationHandler>();

            services.AddScoped<TimeTrackFilter>();

            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                // the url of the front end client
                .WithOrigins("http://localhost:3000"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MeetupSeeder meetupSeeder)
        {
            app.UseStaticFiles();
            app.UseCors("FrontEndClient");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeetupAPI v1");
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            meetupSeeder.Seed();
        }
    }
}
