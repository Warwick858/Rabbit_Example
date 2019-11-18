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
using RabbitMQ.Client;
using Subscribe.Config;
using Subscribe.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Subscribe.Rabbit
{
	public class RabbitChannelCreator : IRabbitChannelCreator
	{
		private readonly IConnection _connection;

		public RabbitChannelCreator(IConnection connection)
		{
			_connection = connection;
		}

		public virtual IModel CreateChannel(QueueConfig queueConfig)
		{
			var queueArgs = new Dictionary<string, object> { { "queue-mode", "lazy" } };

			var newChannel = _connection.CreateModel();

			try
			{
				newChannel.ExchangeDeclare(queueConfig.ExchangeName, queueConfig.ExchangeType, true);
				newChannel.QueueDeclare(queueConfig.QueueName, true, false, false, queueArgs);

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
			}
			catch
			{
				throw;
			}

			return newChannel;
		}
	}
}
