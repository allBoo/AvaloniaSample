using System;
using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Models;

public abstract class Serializable : ISerializable
{
    public class UnserializeException : Exception
    {
        public UnserializeException(string? message) : 
            base(message) {}
    }
    
    public class UnserializeStringException : UnserializeException
    {
        public UnserializeStringException(string line, string? message) : 
            base($"Unable to parse string: {line} -> {message}") {}
    }

    /// <summary>
    /// Разделитель для записи информации по объекту в файл
    /// </summary>
    protected static readonly char _separator = ';';
    
    /// <summary>
    /// Разделитель для записи информации по объекту в файл
    /// </summary>
    protected static readonly char _classSeparator = ':';

    protected static readonly char _classSeparatorSafe = '\ufe55';
    protected static readonly char _separatorSafe = '\uff1b';

    /// <summary>
    /// Returns object name for the serialized form
    /// </summary>
    /// <returns></returns>
    public virtual string DumpName() => GetType().Name;

    /// <summary>
    /// Serialize attrs into string
    /// </summary>
    /// <returns></returns>
    public virtual object[] DumpAttrs() => Array.Empty<object>();

    /// <summary>
    /// Returns list of the serializable children
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<ISerializable>? DumpChildren() => null;
    
    /// <summary>
    /// Adds new child from serialized string
    /// </summary>
    /// <param name="name"></param>
    /// <param name="attrs"></param>
    public virtual void AddChild(string name, string[] attrs)
    {}
    
    /// <summary>
    /// Dumps all children returned from GetSerializableChildren method into string
    /// </summary>
    /// <returns></returns>
    private string ChildrenToString()
    {
        var children = DumpChildren();
        return children != null ? string.Join("", children.ToList().ConvertAll(el => el.ToString())) : "";
    }

    /// <summary>
    /// Dumps all attrs returned from DumpAttrs method into string
    /// </summary>
    /// <returns></returns>
    private string AttrsToString()
    {
        var stringArgs = DumpAttrs().Select(el => _safeString(el.ToString() ?? ""));
        return string.Join(_separator, stringArgs);
    }
    
    /// <summary>
    /// Dumps object representation into string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{_safeString(DumpName())}{_classSeparator}{AttrsToString()}{Environment.NewLine}{ChildrenToString()}";
    }

    private static string _safeString(string origString)
    {
        return origString.Replace(_separator, _separatorSafe).
            Replace(_classSeparator, _classSeparatorSafe);
    }

    private static string _loadSafeString(string origString)
    {
        return origString.Replace(_separatorSafe, _separator).
            Replace(_classSeparatorSafe, _classSeparator);
    }

    /// <summary>
    /// Dumps object representation into file
    /// </summary>
    /// <param name="file"></param>
    public void DumpToFile(System.IO.StreamWriter file)
    {
        file.WriteLine($"{DumpName()}{_classSeparator}{AttrsToString()}");
        
        var children = DumpChildren();
        if (children != null)
        {
            foreach (var child in children)
            {
                child.DumpToFile(file);
            }
        }
    }

    public static T? LoadFromFile<T>(System.IO.StreamReader file, string headerToken) where T : Serializable
    {
        T? instance = null;
        string? line;
        
        while ((line = file.ReadLine()) != null)
        {
            var chunks = line.Split(_classSeparator);
            if (chunks.Length != 2)
            {
                throw new UnserializeStringException(line, "No class attrs");
            }

            var className = _loadSafeString(chunks[0]);
            var classVars = chunks[1].Split(_separator).Select(_loadSafeString).ToArray();

            if (className == headerToken)
            {
                if (instance == null)
                {
                    instance = (T) Activator.CreateInstance(typeof(T), new object?[]{classVars});
                }
                else
                {
                    throw new UnserializeException("Instance already exists. Probably HeaderToken listed twice in the file");
                }
            }
            else if (instance != null)
            {
                instance.AddChild(className, classVars);
            }
        }
        
        return instance;
    }
}
