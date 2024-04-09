# AsyncDemo
Various demos, tips and tricks for using async in C#

[![Build and deploy ASP.Net Core app to Azure Web App - asyncdemo](https://github.com/markhazleton/AsyncDemo/actions/workflows/main_asyncdemo.yml/badge.svg)](https://github.com/markhazleton/AsyncDemo/actions/workflows/main_asyncdemo.yml)

Sample code for making and canceling Async Calls. 

Article on Cancellation Tokens 
[https://markhazleton.com/cancellation-token.html](https://markhazleton.com/cancellation-token.html)

Article on Async and Decorator Pattern
[https://markhazleton.com/decorator-pattern-http-client.html](https://markhazleton.com/decorator-pattern-http-client.html)

Sample source code for the following blog post 
[https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/](https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/)

Another async blog post from John Thiriet
[https://johnthiriet.com/mvvm-going-async-with-async-command/](https://johnthiriet.com/mvvm-going-async-with-async-command/)

# Hosting
Current Hosted at [https://asyncdemoweb.azurewebsites.net/](https://asyncdemoweb.azurewebsites.net/)
# Other Links

[https://devblogs.microsoft.com/pfxteam/await-and-ui-and-deadlocks-oh-my/?WT.mc_id=friends-0000-jamont](https://devblogs.microsoft.com/pfxteam/await-and-ui-and-deadlocks-oh-my/?WT.mc_id=friends-0000-jamont)

[https://montemagno.com/c-sharp-developers-stop-calling-dot-result/](https://montemagno.com/c-sharp-developers-stop-calling-dot-result/)

[https://codereview.stackexchange.com/questions/113108/async-task-with-timeout](https://codereview.stackexchange.com/questions/113108/async-task-with-timeout)

Souce of CancelAsyncWithToken project:

[https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-async-tasks-after-a-period-of-time](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-async-tasks-after-a-period-of-time)

Article on Crafting a Task.Timeout
[https://devblogs.microsoft.com/pfxteam/crafting-a-task-timeoutafter-method/](https://devblogs.microsoft.com/pfxteam/crafting-a-task-timeoutafter-method/)

Related Stack Overflow Question
[https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout](https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout)

Blog post on surfacing retry count for diagnostics
[https://www.stevejgordon.co.uk/polly-using-context-to-obtain-retry-count-diagnostics](https://www.stevejgordon.co.uk/polly-using-context-to-obtain-retry-count-diagnostics)

Blog post on Polly Retry and Circuit Breaker Pattern
[https://medium.com/@therealjordanlee/retry-circuit-breaker-patterns-in-c-with-polly-9aa24c5fe23a](https://medium.com/@therealjordanlee/retry-circuit-breaker-patterns-in-c-with-polly-9aa24c5fe23a)


# Polly Project
[http://www.thepollyproject.org/](http://www.thepollyproject.org/)
Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.

