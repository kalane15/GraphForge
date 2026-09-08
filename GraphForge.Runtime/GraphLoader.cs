using GraphForge.Contracts;
using System;
using System.IO;
using System.Text.Json;

namespace GraphForge.Runtime;

public static class GraphLoader
{
    public static Graph LoadGraphFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        string json = File.ReadAllText(path);
        GraphDto? graphDto = JsonSerializer.Deserialize<GraphDto>(json);

        if (graphDto == null)
        {
            throw new InvalidDataException();
        }

        return GraphMapper.FromDto(graphDto);
    }
}
