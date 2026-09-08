using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace GraphForge.Generator;

[Generator]
public sealed class GraphForgeGenerator : IIncrementalGenerator
{
    public void Initialize(
        IncrementalGeneratorInitializationContext context)
    {
        var schemaFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".schema.json"));

        context.RegisterSourceOutput(
            schemaFiles,
            (ctx, file) =>
            {
                var json = file.GetText(ctx.CancellationToken)?.ToString();

                if (json is null)
                {
                    return;
                }

                foreach (GeneratedSource generatedSource in SchemaClassGenerator.GenerateClassesFromSchemasJson(json))
                {
                    ctx.AddSource(
                        generatedSource.SavePath,
                        SourceText.From(generatedSource.Source, Encoding.UTF8));
                }
            });
    }
}
