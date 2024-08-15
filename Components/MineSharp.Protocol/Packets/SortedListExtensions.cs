namespace MineSharp.Protocol.Packets;

internal static class SortedListExtensions
{
    /// <summary>
    /// Finds the next lower bound in the <see cref="SortedList{TKey, TValue}"/>.
    /// 
    /// Taken from: <see href="https://stackoverflow.com/a/66651020/8353937"/> and slightly modified.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="list"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue? LowerBound<TKey, TValue>(this SortedList<TKey, TValue> list, TKey key)
        where TKey : notnull
    {
        var comparer = list.Comparer;

        #region Short-Circuits

        if (list.Count == 0) //empty list
            return default;
        if (list.TryGetValue(key, out var value))
            return value; //"The function should return the first element equal"

        var keys = list.Keys;
        if (comparer.Compare(keys[keys.Count - 1], key) < 0)
            return default; // if all elements are smaller, return default

        if (comparer.Compare(keys[0], key) > 0)
            return list[keys[0]]; //if the first element is greater, return it

        #endregion Short-Circuits

        int range = list.Count - 1; //the range between of first and last element to check
        int itemIndex = 1;          //the default value of index is 1 since 0 was already checked above
        while (range > 0)
        {
            int halfRange = range / 2;               //cut the search range in half
            int indexTmp = itemIndex + halfRange;    //set the current item to compare
            if (comparer.Compare(keys[indexTmp], key) < 0)
            {
                //the current item is less than the given key
                itemIndex = indexTmp + 1;   //set the current item to the item after the compared item
                range = (range - halfRange - 1); //move the remaining range to the right
            }
            else
            {
                //the current item is greater than the given key (we have already checked equal above in short-circuits)
                range = halfRange; //move the remaining range to the left
            }
        }
        return list[keys[itemIndex]];
    }
}
