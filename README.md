![JRL Logo](http://jimmyloforti.com/_common/images/jrl_logo2.png)

## Application Name: Rabbit_Example ##

* Description: This solution is intended to be a simple, yet feature rich, example of how to
publish and subscribe to a dynamic message broker, exchange and queueing provider, such as RabbitMQ.

* When this program is executed it inserts a message into an exchange and is picked up by the subscriber from the queue.
The feedback to look for is on the rabbit UI.  You should see a blip on the queues message rate preview if working correctly.

### Primary Features ###

* RabbitMQ publishing and subscribing:
	* UI plugin support.
	* Supports an array of queues and multiple exchange/queue types.
	* Multithreaded message dispatchers for subscribing.
* Primary project is a core console/web hybrid:
	* It will run as a console in release, but as a web service when debugging.
	* Console app is required to handle multithreaded message dispatchers for subscribing.

-----------------------------------------------------------------------------------------------

### Why Use RabbitMQ ###

* An introduction to RabbitMQ, and how the underlying message broker technology works
, can be found [here](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)

* Let's be honest.  I'd rather write my own custom data structure from scratch using a uniquely defined rule set that fits the context perfectly.
But if you tell your scrum master that you want to change your dynamic queueing story from a 5 point-story to a 55-pointer,
then you might hear that annoying, cliche: "Don't re-invent the wheel". Code re-use is important, but sometimes it inhibits creative advances.
Nonetheless, when you need a quick solution, RabbitMQ is a safe bet.

-----------------------------------------------------------------------------------------------

### Getting Started ###

1. This is the rundown on how to [get rabbit up and running](https://www.rabbitmq.com/install-windows.html#installer) on your local machine:

2. On the page linked above, you will find that erlang is required to run rabbit with a server of "localhost".
	a. Erlang is super light and [simple to install](https://www.erlang.org/downloads)

3. Once you've installed Erlang, go ahead and install rabbit.  The installer is in the link found in step 1.

4. Now that everything is installed, a few things need to be configured in rabbit.
Let's open the CLI tool that's included with rabbit and enable the UI tool.

	* Press: Windows key + S
	* Type: rabbit
	* Run as administrator: RabbitMQ Command Prompt

	* Run the following command on the CLI:
	rabbitmq-plugins enable rabbitmq_management

	Further details on plugin management can be found [here](https://www.rabbitmq.com/management.html)

	* More info on rabbit's CLI can be found [here](https://www.rabbitmq.com/cli.html) 
	and [here](https://www.rabbitmq.com/management-cli.html)

5. Rabbit is ready to rock!!!  You can view the UI by putting this link in your browser:
http://localhost:15672/#/

	* __Default Username:__ guest
	* __Default Password:__ guest

