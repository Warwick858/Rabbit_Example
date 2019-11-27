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
using Common.Config;
using Common.Model;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Subscribe.Rabbit
{
	public class MessageDispatcher : IDisposable
	{
		private readonly ChannelProvider _channelProvider;
		private readonly IServiceProvider _services;
		private readonly CompositeDisposable _msgSubscriptions;
		private readonly List<IModel> _channels = new List<IModel>();
		private readonly List<QueueConfig> _queues;

		public MessageDispatcher(ChannelProvider channelProvider, IServiceProvider services)
		{
			_channelProvider = channelProvider;
			_services = services;
			_msgSubscriptions = new CompositeDisposable();
			_queues = RabbitConfig.Queues;
		}

		public void StartDispatchers(int numOfDispatchers)
		{
			foreach (var q in _queues)
				for (int i = 0; i < numOfDispatchers; i++)
					StartDispatcher(q);
		}

		public void StartDispatcher(QueueConfig queueConfig)
		{
			var channel = _channelProvider.CreateChannel(queueConfig);
			var incomingMsg = new MessageReceiver(channel, queueConfig.QueueName);
			var subscription = incomingMsg.SubscribeOn(NewThreadScheduler.Default).Retry().Subscribe(DispatchMessage, OnError);
			_msgSubscriptions.Add(subscription);
		}

		public void StopDispatchers()
		{
			if (_msgSubscriptions != null && _msgSubscriptions.Count > 0)
			{
				foreach (var s in _msgSubscriptions)
					s.Dispose();
			}

			if (_channels != null && _channels.Count > 0)
			{
				foreach (var c in _channels)
					c.Close();

				_channels.Clear();
			}
		}

		private static void OnError(Exception ex)
		{
			//Log error
		}

		public void DispatchMessage(Receiver<BunnyModel> msg)
		{
			if (msg != null)
			{
				try
				{
					var scopeFactory = _services.GetRequiredService<IServiceScopeFactory>();
					using (var scope = scopeFactory.CreateScope())
					{
						var msgProcessor = scope.ServiceProvider.GetService<MessageProcessor>();
						msgProcessor.ProcessMessage(msg?.Message);
						msg.Ack();
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}

		public void Dispose()
		{
			_msgSubscriptions?.Dispose();
		}
	} // end class
} // end namespace
