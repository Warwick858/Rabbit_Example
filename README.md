![JRL Logo](http://jimmyloforti.com/_common/images/jrl_logo2.png)

## Application Name: Rabbit_Example
### Description: RabbitMQ pub/sub example.

Implementation includes pub/sub handling with header exchanges/queues.
Additionally, subscriber supports multithreaded message dispatchers.






### Project Comment Feed (comments to my comments are welcome :) so comment) ###
_this feed was written as the project progressed. begins with the oldest comments_


* AppSettings and RabbitConfig utilize static properties.  I'm under the impression that the dotnet "Core" way of doing this is to use dependency injection.  On the other hand, best programming practices dictate these classes be static, uninstantiated, and uninherited from.  So, when using dotnet core, should statics be DI'd instead, thus ignoring these established programming paradigms. The classic argument over who should drive, the framework or the user. ("I fight for the USER!!!" -Tron quote...ignore me, I'm a nerd.) Thoughts???
* I did not extend all my classes with interfaces, so if you want to use core DI the _right_ way, and want to alleviate your testing efforts, extend all classes with interfaces by default.  Am I wrong here???



