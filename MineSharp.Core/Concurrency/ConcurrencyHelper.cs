using System.Runtime.CompilerServices;

namespace MineSharp.Core.Concurrency;

public record struct EnsureOnlyRunOnceAsyncResult(Task Task, bool FirstInvocation)
{
    public TaskAwaiter GetAwaiter() => Task.GetAwaiter();
}
public record struct EnsureOnlyRunOnceAsyncResult<TResult>(Task<TResult> Task, bool FirstInvocation)
{
    public TaskAwaiter<TResult> GetAwaiter() => Task.GetAwaiter();
}

public static class ConcurrencyHelper
{
    #region EnsureOnlyRunOnce

    /// <inheritdoc cref="EnsureOnlyRunOnceAsync{TResult}(Func{Task{TResult}}, ref Task{TResult}?)" />
    public static EnsureOnlyRunOnceAsyncResult EnsureOnlyRunOnceAsync(Func<Task> action, ref Task? taskStore)
    {
        var ret = EnsureOnlyRunOnceAsync(async () =>
        {
            // we need to do the typical async/await because otherwise the exception would be lost and the task will not be faulted
            await action();
            // return value does mean nothing. Is just used to call the other method.
            return true;
        },
        // this ref type conversion is safe because all Tasks with return type are also normal Tasks
        ref Unsafe.As<Task?, Task<bool>?>(ref taskStore));

        return new(ret.Task, ret.FirstInvocation);
    }

    /// <summary>
    ///     Ensures that the given async action is only run once while still returning the task of that first invocation for further calls.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="action">The async action to execute once.</param>
    /// <param name="taskStore">A reference to a variable holding the task of the first and only execution.</param>
    /// <returns></returns>
    public static EnsureOnlyRunOnceAsyncResult<TResult> EnsureOnlyRunOnceAsync<TResult>(Func<Task<TResult>> action, ref Task<TResult>? taskStore)
    {
        var storedTask = taskStore;
        if (storedTask is not null)
        {
            return new(storedTask, false);
        }

        // we can use using here because this method is the only place where the cts is used
        using var cts = new CancellationTokenSource();
        var newTaskWrapped = new Task<Task<TResult>>(action, cancellationToken: cts.Token);
        // TODO: Check if Unwrap does forward OCE. If not use UnwrapSlow
        var newTask = newTaskWrapped.Unwrap();
        //var newTask = UnwrapSlow(newTaskWrapped);

        var actualStoredTask = Interlocked.CompareExchange(ref taskStore, newTask, null);
        if (ReferenceEquals(actualStoredTask, null))
        {
            // we set the stored task to the new task
            // and start the new task

            // important: start newTaskWrapped not newTask
            newTaskWrapped.Start();
            return new(newTask, true);
        }

        // some other task was faster and set the task
        // cancel the new task
        cts.Cancel();
        // and return the already started task
        return new(actualStoredTask, false);
    }

    private static async Task<TResult> UnwrapSlow<TResult>(Task<Task<TResult>> task)
    {
        return await await task;
    }
    #endregion
}
