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
using Common.Config;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Subscribe.Rabbit
{
	public class ChannelProvider : IDisposable
	{
		private readonly IConnection _connection;
		private bool _disposed = false;

		public ChannelProvider(IConnection connection)
		{
			_connection = connection;
		}

		~ChannelProvider()
		{
			Dispose(false);
		}

		public virtual IModel CreateChannel(QueueConfig queueConfig)
		{
			var queueArgs = new Dictionary<string, object> { { "queue-mode", "lazy" } };

			var newChannel = _connection.CreateModel();

			try
			{
				newChannel.ExchangeDeclare(queueConfig.ExchangeName, queueConfig.ExchangeType, true);
				newChannel.QueueDeclare(queueConfig.QueueName, true, false, false, queueArgs);

				//If header exchange
				if (queueConfig.ExchangeType == ExchangeType.Headers)
				{
					var headerOptions = new Dictionary<string, object>();
					foreach (var (key, value) in queueConfig.Headers)
						headerOptions.Add(key, value);

					newChannel.QueueBind(queueConfig.QueueName, queueConfig.ExchangeName, queueConfig.RoutingKey, headerOptions);
				}
				else
					newChannel.QueueBind(queueConfig.QueueName, queueConfig.ExchangeName, queueConfig.RoutingKey);

				newChannel.BasicQos(0, 32, false);
				newChannel.ConfirmSelect();
				_connection.AutoClose = true;
			}
			catch
			{
				throw;
			}

			return newChannel;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
					_connection.Dispose();

				_disposed = true;
			}
		}
	} // end class
} // end namespace
