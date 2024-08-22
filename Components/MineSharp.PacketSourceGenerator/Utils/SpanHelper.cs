using System;
using System.Collections.Generic;

namespace MineSharp.PacketSourceGenerator.Utils;

public static class SpanHelper
{
	public static bool ContainsOnlyAllowedItems<T>(this ReadOnlySpan<T> items, ReadOnlySpan<T> allowedItems, IEqualityComparer<T>? equalityComparer = null)
	{
		if (items.Length == 0)
		{
			return true;
		}

		equalityComparer ??= EqualityComparer<T>.Default;

		for (var i = 0; i < items.Length; i++)
		{
			var item = items[i];
			var isAllowed = false;
			for (var j = 0; j < allowedItems.Length; j++)
			{
				var allowedItem = allowedItems[j];
				if (equalityComparer.Equals(item, allowedItem))
				{
					isAllowed = true;
					break;
				}
			}

			if (!isAllowed)
			{
				return false;
			}
		}

		return true;
	}
}
