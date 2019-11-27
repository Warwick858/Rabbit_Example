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
using Common;
using Common.AppSettings;
using Common.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish;
using Publish.Rabbit;
using RabbitMQ.Client;
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
		private MessageDispatcher _dispatcher;

		//private IConfigurationRoot Config { get; }
		//public Startup(IHostingEnvironment env)
		//{
		//	var builder = new ConfigurationBuilder()
		//		.SetBasePath(env.ContentRootPath)
		//		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		//		.AddEnvironmentVariables();
		//	Config = builder.Build();
		//}

		private IConfiguration Config { get; }

		public Startup(IConfiguration configuration)
		{
			Config = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			//services.AddHttpContextAccessor();

			_appSettings = Config.GetSection("AppSettings").Get<AppSettings>();
			services.AddSingleton(a => _appSettings);

			var rabbitConfig = new RabbitConfig();
			Config.GetSection("RabbitSettings").Bind(rabbitConfig);
			RabbitConfig.UserName = DecryptCypherText(RabbitConfig.UserName); // always decrypt credentials at runtime
			RabbitConfig.Password = DecryptCypherText(RabbitConfig.Password); // always decrypt credentials at runtime

			//var httpClient = new HttpClient();
			//httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			//services.AddSingleton(h => httpClient);

			//var connectionFactory = new ConnectionFactory
			//{
			//	AutomaticRecoveryEnabled = true,
			//	NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
			//	TopologyRecoveryEnabled = true,
			//	UserName = RabbitConfig.UserName,
			//	Password = RabbitConfig.Password,
			//	HostName = RabbitConfig.Server
			//};

			var rabbitConnection = ConnectionProvider.CreateConnection();
			services.AddSingleton(rabbitConnection);
			//Shared
			services.AddSingleton<ChannelProvider>();
			services.AddSingleton<ConnectionProvider>();
			//Publish
			services.AddSingleton<Marshaller>();
			services.AddSingleton<MessageSender>();
			services.AddSingleton<Sender>();
			var senderProvider = new SenderProvider();
			services.AddSingleton(senderProvider);
			services.AddSingleton<SenderWrapper>();
			//Subscribe
			services.AddTransient<MessageProcessor>();
			services.AddSingleton<MessageDispatcher>();

			if (Debugger.IsAttached)
			{
				var temp = services.BuildServiceProvider();
				_dispatcher = temp.GetService<MessageDispatcher>();
				_dispatcher.StartDispatchers(RabbitConfig.DispatcherCount);
			}

			//Emulate pub/sub eventing
			var processor = new PublishGenerator(senderProvider);
			processor.StartPubSubLoop();
		} // end ConfigureServices

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifeTime)
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

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Running!!!");
			});
		} // end Configure

		private string DecryptCypherText(string cypherText)
		{


			return cypherText;
		}

		private void OnShutDown()
		{
			_dispatcher.StopDispatchers();
		}
	} // end class
} // end namespace
