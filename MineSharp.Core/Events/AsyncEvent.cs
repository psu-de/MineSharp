using ConcurrentCollections;
using NLog;

namespace MineSharp.Core.Events;

/// <summary>
/// Base class for all async events
/// </summary>
/// <typeparam name="TSync">Synchronous event handler delegate</typeparam>
/// <typeparam name="TAsync">Asynchronous event handler delegate</typeparam>
public abstract class AsyncEventBase<TSync, TAsync> where TSync : Delegate where TAsync : Delegate
{
    /// <summary>
    /// List of synchronous handlers subscribed to this event
    /// </summary>
    protected readonly ConcurrentHashSet<TSync> Handlers = [];

    /// <summary>
    /// List of asynchronous handlers subscribed to this event
    /// </summary>
    protected readonly ConcurrentHashSet<TAsync> AsyncHandlers = [];

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    public bool Subscribe(TSync handler)
    {
        return Handlers.Add(handler);
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    public bool Unsubscribe(TSync handler)
    {
        return Handlers.TryRemove(handler);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    public bool Subscribe(TAsync handler)
    {
        return AsyncHandlers.Add(handler);
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    public bool Unsubscribe(TAsync handler)
    {
        return AsyncHandlers.TryRemove(handler);
    }

    /// <summary>
    /// Makes sure all handlers run to completion and catch exceptions in the handlers
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
    protected Task WaitForHandlers(Task[] tasks)
    {
        var task = Task.Run(async () =>
        {
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                foreach (var exception in tasks.Where(x => x.Exception != null))
                {
                    HandleExceptionInHandler(exception.Exception?.InnerException!);
                }
            }
        });

        return task;
    }

    /// <summary>
    /// Handle an exception thrown in a handler
    /// </summary>
    private void HandleExceptionInHandler(Exception exception)
    {
        Logger.Warn($"Error in event handler: {exception}");
    }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
}

/// <summary>
/// An event supporting synchronous and asynchronous handlers.
/// </summary>
public class AsyncEvent : AsyncEventBase<AsyncEvent.Handler, AsyncEvent.AsyncHandler>
{
    /// <summary>
    /// A synchronous event handler
    /// </summary>
    public delegate void Handler();

    /// <summary>
    /// An asynchronous event handler
    /// </summary>
    public delegate Task AsyncHandler();

    /// <summary>
    /// Dispatch this event
    /// </summary>
    /// <returns>A task that completes once all handlers have completed.</returns>
    public Task Dispatch()
    {
        var tasks = AsyncHandlers
                   .Select(handler => Task.Run(async () => await handler()))
                   .Concat(Handlers.Select(handler => Task.Run(() => handler())))
                   .ToArray();

        return WaitForHandlers(tasks);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent operator +(AsyncEvent eventBase, Handler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent operator +(AsyncEvent eventBase, AsyncHandler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent operator -(AsyncEvent eventBase, Handler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent operator -(AsyncEvent eventBase, AsyncHandler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }
}

/// <summary>
/// An event supporting synchronous and asynchronous handlers.
/// </summary>
public class AsyncEvent<T> : AsyncEventBase<AsyncEvent<T>.Handler, AsyncEvent<T>.AsyncHandler>
{
    /// <summary>
    /// A synchronous event handler
    /// </summary>
    public delegate void Handler(T arg1);

    /// <summary>
    /// An asynchronous event handler
    /// </summary>
    public delegate Task AsyncHandler(T arg1);

    /// <summary>
    /// Dispatch this event
    /// </summary>
    /// <param name="arg1">First argument for the handlers</param>
    /// <returns>A task that completes once all handlers have completed.</returns>
    public Task Dispatch(T arg1)
    {
        var tasks = AsyncHandlers
                   .Select(handler => Task.Run(async () => await handler(arg1)))
                   .Concat(Handlers.Select(handler => Task.Run(() => handler(arg1))))
                   .ToArray();

        return WaitForHandlers(tasks);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T> operator +(AsyncEvent<T> eventBase, Handler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T> operator +(AsyncEvent<T> eventBase, AsyncHandler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T> operator -(AsyncEvent<T> eventBase, Handler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T> operator -(AsyncEvent<T> eventBase, AsyncHandler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }
}

/// <summary>
/// An event supporting synchronous and asynchronous handlers.
/// </summary>
public class AsyncEvent<T1, T2> : AsyncEventBase<AsyncEvent<T1, T2>.Handler, AsyncEvent<T1, T2>.AsyncHandler>
{
    /// <summary>
    /// A synchronous event handler
    /// </summary>
    public delegate void Handler(T1 arg1, T2 arg2);

    /// <summary>
    /// An asynchronous event handler
    /// </summary>
    public delegate Task AsyncHandler(T1 arg1, T2 arg2);

    /// <summary>
    /// Dispatch this event
    /// </summary>
    /// <param name="arg1">First argument for the handlers</param>
    /// <param name="arg2">Second argument for the handlers</param>
    /// <returns>A task that completes once all handlers have completed.</returns>
    public Task Dispatch(T1 arg1, T2 arg2)
    {
        var tasks = AsyncHandlers
                   .Select(handler => Task.Run(async () => await handler(arg1, arg2)))
                   .Concat(Handlers.Select(handler => Task.Run(() => handler(arg1, arg2))))
                   .ToArray();

        return WaitForHandlers(tasks);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2> operator +(AsyncEvent<T1, T2> eventBase, Handler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2> operator +(AsyncEvent<T1, T2> eventBase, AsyncHandler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2> operator -(AsyncEvent<T1, T2> eventBase, Handler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2> operator -(AsyncEvent<T1, T2> eventBase, AsyncHandler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }
}

/// <summary>
/// An event supporting synchronous and asynchronous handlers.
/// </summary>
public class AsyncEvent<T1, T2, T3> : AsyncEventBase<AsyncEvent<T1, T2, T3>.Handler, AsyncEvent<T1, T2, T3>.AsyncHandler>
{
    /// <summary>
    /// A synchronous event handler
    /// </summary>
    public delegate void Handler(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// An asynchronous event handler
    /// </summary>
    public delegate Task AsyncHandler(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Dispatch this event
    /// </summary>
    /// <param name="arg1">First argument for the handlers</param>
    /// <param name="arg2">Second argument for the handlers</param>
    /// <param name="arg3">Third argument for the handlers</param>
    /// <returns>A task that completes once all handlers have completed.</returns>
    public Task Dispatch(T1 arg1, T2 arg2, T3 arg3)
    {
        var tasks = AsyncHandlers
                   .Select(handler => Task.Run(async () => await handler(arg1, arg2, arg3)))
                   .Concat(Handlers.Select(handler => Task.Run(() => handler(arg1, arg2, arg3))))
                   .ToArray();

        return WaitForHandlers(tasks);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3> operator +(AsyncEvent<T1, T2, T3> eventBase, Handler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3> operator +(AsyncEvent<T1, T2, T3> eventBase, AsyncHandler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3> operator -(AsyncEvent<T1, T2, T3> eventBase, Handler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3> operator -(AsyncEvent<T1, T2, T3> eventBase, AsyncHandler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }
}

/// <summary>
/// An event supporting synchronous and asynchronous handlers.
/// </summary>
public class AsyncEvent<T1, T2, T3, T4> : AsyncEventBase<AsyncEvent<T1, T2, T3, T4>.Handler, AsyncEvent<T1, T2, T3, T4>.AsyncHandler>
{
    /// <summary>
    /// A synchronous event handler
    /// </summary>
    public delegate void Handler(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// An asynchronous event handler
    /// </summary>
    public delegate Task AsyncHandler(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Dispatch this event
    /// </summary>
    /// <param name="block">Whether to block until all handlers are completed</param>
    /// <param name="arg1">First argument for the handlers</param>
    /// <param name="arg2">Second argument for the handlers</param>
    /// <param name="arg3">Third argument for the handlers</param>
    /// <param name="arg4">Fourth argument for the handlers</param>
    /// <returns>A task that completes once all handlers have completed.</returns>
    public Task Dispatch(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        var tasks = AsyncHandlers
                   .Select(handler => Task.Run(async () => await handler(arg1, arg2, arg3, arg4)))
                   .Concat(Handlers.Select(handler => Task.Run(() => handler(arg1, arg2, arg3, arg4))))
                   .ToArray();

        return WaitForHandlers(tasks);
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3, T4> operator +(AsyncEvent<T1, T2, T3, T4> eventBase, Handler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Subscribe <paramref name="handler"/> to this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3, T4> operator +(AsyncEvent<T1, T2, T3, T4> eventBase, AsyncHandler handler)
    {
        eventBase.Subscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3, T4> operator -(AsyncEvent<T1, T2, T3, T4> eventBase, Handler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }

    /// <summary>
    /// Unsubscribe <paramref name="handler"/> from this event
    /// </summary>
    /// <param name="eventBase"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static AsyncEvent<T1, T2, T3, T4> operator -(AsyncEvent<T1, T2, T3, T4> eventBase, AsyncHandler handler)
    {
        eventBase.Unsubscribe(handler);
        return eventBase;
    }
}
