namespace MineSharp.Protocol.Packets;

internal static class SortedListExtensions
{
    /// <summary>
    /// Finds the next lower bound in the <see cref="SortedList{TKey, TValue}"/>.
    /// 
    /// Taken from: <see href="https://stackoverflow.com/a/66651020/8353937"/> and modified.
    /// </summary>
    public static bool TryGetLowerBound<TKey, TValue>(this SortedList<TKey, TValue> list, TKey key, out TValue? lowerBound)
        where TKey : notnull
    {
        var comparer = list.Comparer;

        #region Short-Circuits

        if (list.Count == 0)
        {
            //empty list
            lowerBound = default;
            return false;
        }

        if (list.TryGetValue(key, out lowerBound))
        {
            // "The function should return the first element equal"
            return true;
        }

        var keys = list.Keys;
        var lastKey = keys[keys.Count - 1];
        if (comparer.Compare(lastKey, key) < 0)
        {
            // if all elements are smaller, return the biggest element
            lowerBound = list[lastKey];
            return true;
        }

        if (comparer.Compare(keys[0], key) > 0)
        {
            // if the first element is greater, return not found
            lowerBound = default;
            return false;
        }

        #endregion Short-Circuits

        int range = list.Count - 1; // the range between first and last element to check
        int itemIndex = 1;          // the default value of index is 1 since 0 was already checked above
        while (range > 0)
        {
            int halfRange = range / 2;               // cut the search range in half
            int indexTmp = itemIndex + halfRange;    // set the current item to compare
            if (comparer.Compare(keys[indexTmp], key) < 0)
            {
                // the current item is lower than the given key
                itemIndex = indexTmp + 1;   // set the current item to the item after the compared item
                range = (range - halfRange - 1); // move the remaining range to the right
            }
            else
            {
                // the current item is greater than the given key (we have already checked equal above in short-circuits)
                range = halfRange; // move the remaining range to the left
            }
        }
        lowerBound = list[keys[itemIndex]];
        return true;
    }
}
