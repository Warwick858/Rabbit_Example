![JRL Logo](http://jimmyloforti.com/_common/images/jrl_logo2.png)

## Application Name: Rabbit_Example ##

* Description: This solution is intended to be a simple, yet feature rich, example of how to
publish and subscribe to a dynamic message broker exchange and queueing provider such as RabbitMQ.

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
, can be found [here](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html "title" target="_blank")

<a href="https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html" target="_blank">here</a>

* Let's be honest.  I'd rather write my own custom data structure from scratch using a uniquely defined rule set that fits the context perfectly.
But if you tell your scrum master that you want to change your dynamic queueing story from a 5 point-story to a 55-pointer,
then you might hear that annoying, cliche: "Don't re-invent the wheel". Code re-use is important, but sometimes it inhibits creative advances.
Nonetheless, when you need a quick solution, RabbitMQ is a safe bet.

-----------------------------------------------------------------------------------------------

### Getting Started ###

1. This is the rundown on how to get rabbit up and running on your local machine:
[https://www.rabbitmq.com/install-windows.html#installer]
(https://www.rabbitmq.com/install-windows.html#installer)

2. On the page linked above, you will find that erlang is required to run rabbit with a server of "localhost".
	a. Erlang is super light and simple to install, which can be found here:
https://www.erlang.org/downloads
[Erlang](https://www.erlang.org/downloads)

3. Once you've installed Erlang, go ahead and install rabbit.  The installer is in the link found in step 1.

4. Now that everything is installed, a few things need to be configured in rabbit.
Let's open the CLI tool that's included with rabbit and enable the UI tool.

	* Press: Windows key + S
	* Type: rabbit
	* Run as administrator: RabbitMQ Command Prompt

	* Run the following command on the CLI:
	rabbitmq-plugins enable rabbitmq_management

	Further details on plugin management can be found here:
	https://www.rabbitmq.com/management.html
	[Rabbit Plugin Mgmt](https://www.rabbitmq.com/management.html)

	* More info on rabbit's CLI can be found here:
	https://www.rabbitmq.com/cli.html
	[Rabbit CLI](https://www.rabbitmq.com/cli.html)
	https://www.rabbitmq.com/management-cli.html
	[Rabbit CLI Mgmt](https://www.rabbitmq.com/management-cli.html)

5. Rabbit is ready to rock!!!  You can view the UI by putting this link in your browser:
http://localhost:15672/#/

	* __Default Username:__ guest
	* __Default Password:__ guest

-----------------------------------------------------------------------------------------------

### Project Comment Feed ###
_General feed of thoughts and ideas as the project progressed. Begins with oldest comments_


* AppSettings and RabbitConfig utilize static properties.  I'm under the impression that the dotnet "Core" way of doing this is to use dependency injection.  On the other hand, best programming practices dictate these classes be static, uninstantiated, and uninherited from.  So, when using dotnet core, should statics be DI'd instead, thus ignoring these established programming paradigms. The classic argument over who should drive, the framework or the user. ("I fight for the USER!!!" -Tron quote...ignore me, I'm a nerd.) Thoughts???
* I did not extend all my classes with interfaces, so if you want to use core DI the _right_ way, and want to alleviate your testing efforts, extend all classes with interfaces by default.  Am I wrong here???
* Made the instructions on the readme more comprehensive.  Makes sense.



