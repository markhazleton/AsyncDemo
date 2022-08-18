// See https://aka.ms/new-console-template for more information
using AsyncDemo;

Console.WriteLine($"Program Start:  Thread:{Environment.CurrentManagedThreadId}");

var asyncMock = new AsyncMockService();

await asyncMock.ExecuteTaskAsync(true).ConfigureAwait(true);
Console.WriteLine($"Program After Configure Await True:  Thread:{Environment.CurrentManagedThreadId}");

await asyncMock.ExecuteTaskAsync(false).ConfigureAwait(false);
Console.WriteLine($"Program After Configure Await False:  Thread:{Environment.CurrentManagedThreadId}");

//await asyncMock.ExecuteTaskWithTimeoutAsync(TimeSpan.FromSeconds(2));

//await asyncMock.ExecuteTaskWithTimeoutAsync(TimeSpan.FromSeconds(10));

//await asyncMock.ExecuteManuallyCancellableTaskAsync();

//await asyncMock.ExecuteManuallyCancellableTaskAsync();

//await asyncMock.CancelANonCancellableTaskAsync();




