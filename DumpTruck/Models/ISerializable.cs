using System.Collections.Generic;

namespace DumpTruck.Models;

public interface ISerializable
{
    /// <summary>
    /// Returns object name for the serialized form
    /// </summary>
    /// <returns></returns>
    public string DumpName();
    
    /// <summary>
    /// Serialize attrs into string
    /// </summary>
    /// <returns></returns>
    public string DumpAttrs();

    /// <summary>
    /// Returns list of the serializable children
    /// </summary>
    /// <returns></returns>
    public virtual List<ISerializable>? GetSerializableChildren() => null;

    public string DumpChildren();
    
    public string ToString();

    public void DumpToFile(System.IO.StreamWriter file);
}
