using System;
using System.Collections.Generic;
using System.Linq;

public class ResolutionContext
{
    public object SourceValue { get; set; }
    public object DestinationValue { get; set; }

}

public class MapperGodObject
{

    public void Configure() { /* ... */ }
    
    public bool IsValid(object obj) => true;
    
    public object Map(object source) => source;
    
    public void LogToDatabase(string message) { /* ... */ }
}

public interface IObjectMapper
{
    bool CanMap(Type source, Type dest);
    object Map(object source);
}

public class ArrayMapper : IObjectMapper
{
    public bool CanMap(Type s, Type d) => s.IsArray;
    public object Map(object source) => ((Array)source).Clone();
}

public class MappingEngine
{
    private readonly List<IObjectMapper> _mappers = new();

    public void RegisterMapper(IObjectMapper mapper) => _mappers.Add(mapper);

    public object Execute(object source, Type targetType)
    {
        var mapper = _mappers.FirstOrDefault(m => m.CanMap(source.GetType(), targetType));
        return mapper?.Map(source) ?? throw new Exception("Mapper not found");
    }
}

public class ViolatingOCPMapper
{
    public object Map(object source, string targetType)
    {
        return targetType switch
        {
            "String" => source.ToString(),
            "Integer" => Convert.ToInt32(source),
            _ => throw new NotSupportedException()
        };
    }
}