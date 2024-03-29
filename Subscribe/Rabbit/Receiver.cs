﻿// ******************************************************************************************************************
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
using RabbitMQ.Client.Events;

namespace Subscribe.Rabbit
{
	public class Receiver<T>
	{
		private BasicDeliverEventArgs Envelope { get; set; }
		private IModel ReceivingChannel { get; set; }
		public bool IsMessageRejected { get; set; }
		public T Message { get; set; }
		public string RejectReason { get; set; }

		public Receiver(T message, BasicDeliverEventArgs envelope, IModel channel)
		{
			Message = message;
			Envelope = envelope;
			ReceivingChannel = channel;
		}

		public void Ack()
		{
			ReceivingChannel.BasicAck(Envelope.DeliveryTag, false);
		}

		public void Reject()
		{
			ReceivingChannel.BasicReject(Envelope.DeliveryTag, false);
		}
	} // end class
} // end namespace
