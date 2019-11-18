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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Subscribe.Config;
using Subscribe.Rabbit;

namespace Api
{
	public class RabbitService : WebHostService
	{
		private readonly MessageDispatcher _dispatcher;

		public RabbitService(IWebHost host) : base(host)
		{
			_dispatcher = host.Services.GetRequiredService<MessageDispatcher>();
		}

		protected override void OnStarting(string[] args)
		{
			_dispatcher.StartDispatchers(RabbitConfig.DispatcherCount);
			base.OnStarting(args);
		}

		protected override void OnStarted()
		{
			_dispatcher.StopDispatchers();
			base.OnStarted();
		}

		protected override void OnStopping()
		{
			base.OnStopping();
		}

		protected override void OnStopped()
		{
			base.OnStopped();
		}
	}
}
