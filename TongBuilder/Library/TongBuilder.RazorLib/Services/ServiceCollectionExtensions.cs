﻿using AntDesign;
using AntDesignProApp.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TongBuilder.RazorLib.Services.Auth;
using TongBuilder.RazorLib.Services.Settings;


namespace TongBuilder.RazorLib.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
           
            services.Configure<LayoutSettings>(configuration.GetSection("LayoutSettings"));

            services.AddAntDesign();  

            services.AddScoped<ModalService>();
            services.AddScoped<MessageService>();

            //services.AddScoped<IChartService, ChartService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IProfileService, ProfileService>();

            services.AddHttpClient();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, TongAuthenticationStateProvider>();
            
            services.AddCascadingAuthenticationState();
            //services.AddBlazoredLocalStorage();
            

        }
    }

}
