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
using Common.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Subscribe.Rabbit
{
	public class MessageReceiver : IObservable<Receiver<BunnyModel>>
	{
		private readonly IModel _channel;
		private readonly JsonSerializerSettings _jsonSettings;
		private readonly string _queue;

		public MessageReceiver(IModel channel, string queue)
		{
			_channel = channel;
			_queue = queue;
			_jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
		}

		public IDisposable Subscribe(IObserver<Receiver<BunnyModel>> observer)
		{
			var consumer = new EventingBasicConsumer(_channel);
			var observableDisposable = Observable.FromEventPattern<BasicDeliverEventArgs>(consumer, "Received")
				.Select(CreateIncomingMessage).Subscribe(observer);
			_channel.BasicConsume(_queue, false, consumer);

			return Disposable.Create(() =>
			{
				observableDisposable.Dispose();
				if (!_channel.IsOpen)
					return;

				_channel.Close();
			});
		}

		private Receiver<BunnyModel> CreateIncomingMessage(EventPattern<BasicDeliverEventArgs> receivedEvent)
		{
			Receiver<BunnyModel> newMsg = null;
			var envelope = receivedEvent.EventArgs;

			if (envelope != null)
			{
				try
				{
					newMsg = new Receiver<BunnyModel>(
						JsonConvert.DeserializeObject<BunnyModel>(Encoding.UTF8.GetString(envelope.Body), _jsonSettings), envelope, _channel);
				}
				catch
				{
					_channel.BasicNack(envelope.DeliveryTag, false, false);
				}
			} // end if
			else
			{
				_channel.BasicNack(envelope.DeliveryTag, false, false);
			}

			return newMsg;
		}
	} // end class
} // end namespace
