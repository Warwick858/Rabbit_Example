// ******************************************************************************************************************
//  This file is part of Rabbit_Example.
//
//  Rabbit_Example - simple example of rabbit publishing and subscribing.
//  Copyright(C)  2019  James LoForti
//  Contact Info: jamesloforti@gmail.com
//
//  Rabbit_Example is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.If not, see<https://www.gnu.org/licenses/>.
//									     ____.           .____             _____  _______   
//									    |    |           |    |    ____   /  |  | \   _  \  
//									    |    |   ______  |    |   /  _ \ /   |  |_/  /_\  \ 
//									/\__|    |  /_____/  |    |__(  <_> )    ^   /\  \_/   \
//									\________|           |_______ \____/\____   |  \_____  /
//									                             \/          |__|        \/ 
//
// ******************************************************************************************************************
//
using Common.AppSettings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Subscribe.Config;
using Subscribe.Rabbit;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Api
{
	public class Startup
	{
		private AppSettings _appSettings;

		private IConfigurationRoot Config { get; }
		private MessageDispatcher _dispatcher;

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();
			Config = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			_appSettings = Config.GetSection("AppSettings").Get<AppSettings>();
			services.AddSingleton(a => _appSettings);

			var rabbitConfig = new RabbitConfig();
			Config.GetSection("RabbitSettings").Bind(rabbitConfig);

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			services.AddSingleton(h => httpClient);

			var connectionFactory = new ConnectionFactory
			{
				AutomaticRecoveryEnabled = true,
				NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
				TopologyRecoveryEnabled = true,
				UserName = DecryptCypherText(RabbitConfig.UserName),
				Password = DecryptCypherText(RabbitConfig.Password),
				HostName = RabbitConfig.Server
			};

			var rabbitConnection = connectionFactory.CreateConnection();
			services.AddSingleton(rabbitConnection);
			services.AddSingleton<RabbitChannelCreator>();
			services.AddSingleton<MessageDispatcher>();

			if (Debugger.IsAttached)
			{
				var provider = services.BuildServiceProvider();
				_dispatcher = provider.GetService<MessageDispatcher>();
				_dispatcher.StartDispatchers(RabbitConfig.DispatcherCount);
			}
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifeTime, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			lifeTime.ApplicationStopping.Register(OnShutDown);

			app.UseMvc();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
		}

		private string DecryptCypherText(string cypherText)
		{


			return cypherText;
		}

		private void OnShutDown()
		{
			_dispatcher.StopDispatchers();
		}
	}
}
