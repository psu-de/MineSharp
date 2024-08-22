using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class TaskHelper
{
	// created with inspiration from https://devblogs.microsoft.com/pfxteam/processing-tasks-as-they-complete/
	public static async IAsyncEnumerable<TInput> AsTheyComplete<TInput, TTask>(IEnumerable<TInput> enumerable, Func<TInput, TTask> taskSelector)
		where TTask : Task
	{
		var inputList = enumerable.ToList();

		var buckets = new TaskCompletionSource<TInput>[inputList.Count];
		for (var i = 0; i < buckets.Length; i++)
		{
			buckets[i] = new TaskCompletionSource<TInput>(TaskCreationOptions.RunContinuationsAsynchronously);
		}

		int nextTaskIndex = -1;
		Action<Task, object> continuation = (completedTask, input) =>
		{
			var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
			bucket.TrySetResult((TInput)input);
		};

		foreach (var input in inputList)
		{
			var inputTask = taskSelector(input);
			_ = inputTask.ContinueWith(continuation, input, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}

		foreach (var bucket in buckets)
		{
			yield return await bucket.Task;
		}
	}

}
