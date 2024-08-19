using MineSharp.Core.Common;

namespace MineSharp.Data.Framework;

/// <summary>
/// Interface to implement indexed data, where a single entry does not
/// contain any other data besides a name and a corresponding protocol number.
/// </summary>
public interface INameAndProtocolNumberIndexedData
{
    /// <summary>
    ///     The number of data entries
    /// </summary>
    public int Count { get; }
    
    /// <summary>
    /// Return the protocol number associated with the given identifier.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetProtocolId(Identifier name);
    
    /// <summary>
    /// Return the <see cref="Identifier"/> associated with the given protocol number.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Identifier GetName(int id);
}

/// <inheritdoc cref="INameAndProtocolNumberIndexedData"/>
/// <typeparam name="TEnum"></typeparam>
public interface INameAndProtocolNumberIndexedData<TEnum> : INameAndProtocolNumberIndexedData
{
    /// <summary>
    /// Return the protocol number associated with the given <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetProtocolId(TEnum type);

    /// <summary>
    /// Return <typeparam name="TEnum"></typeparam> associated with the given protocol number.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TEnum GetType(int id);
} 
