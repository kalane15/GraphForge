using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Schemas;
using GraphForge.Api.Models;
using GraphForge.Api.Services.GraphService;
using GraphForge.Contracts;
using GraphForge.Validation;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Services.SchemasService;

public class SchemasService : ISchemasService
{
    private readonly AppDbContext _db;
    private readonly ISchemaDtoValidatorService _schemaDtoValidator;

    public SchemasService(AppDbContext db, ISchemaDtoValidatorService validator)
    {
        _db = db;
        _schemaDtoValidator = validator;
    }

    public async Task<SchemasListResponse> GetSchemasList(Guid userId, Guid projectId)
    {
        await EnsureProjectBelongsToUser(userId, projectId);

        var schemas = await _db.Schemas
            .Where(s => s.ProjectId == projectId)
            .OrderBy(s => s.SchemaTypeName)
            .Select(s => new SchemaResponse
                (
                s.Id,
                s.SchemaTypeName,
                s.Fields
                .Select(f => new SchemaFieldDefinitionResponse(f.Id, f.Name, f.Type))
                .ToList()
                )
            )
            .ToListAsync();

        return new SchemasListResponse(schemas);
    }

    public async Task<SchemaResponse> CreateSchema(Guid userId, Guid projectId, SchemaCreateRequest request)
    {
        await EnsureProjectBelongsToUser(userId, projectId);


        SchemaDto dto = SchemaRequestToDToMapper.ToDto(request);
        _schemaDtoValidator.ValidateSchema(dto);


        var schemaId = Guid.NewGuid();

        var schema = new Schema
        {
            ProjectId = projectId,
            SchemaTypeName = request.SchemaTypeName,
            Fields = request.Fields.Select(f => new SchemaField
            {
                Name = f.Name,
                Type = f.Type
            }).ToList()
        };

        _db.Schemas.Add(schema);
        await _db.SaveChangesAsync();

        List <SchemaFieldDefinitionResponse> fieldsDefinions = schema.Fields
            .Select(f => new SchemaFieldDefinitionResponse(f.Id, f.Name, f.Type)).ToList();

        return new SchemaResponse(schema.Id, schema.SchemaTypeName, fieldsDefinions);
    }

    public async Task UpdateSchema(Guid userId, Guid projectId, Guid schemaId, SchemaDataRequest request)
    {
        await EnsureProjectBelongsToUser(userId, projectId);


        SchemaDto dto = SchemaRequestToDToMapper.ToDto(schemaId, request);
        _schemaDtoValidator.ValidateSchema(dto);


        Schema? schema = await _db.Schemas
            .Include(schema => schema.Fields)
            .FirstOrDefaultAsync(
            (schema) =>
                schema.Id == schemaId &&
                schema.ProjectId == projectId &&
                schema.Project.OwnerId == userId
            );

        if (schema is null)
        {
            throw new NotFoundException("Schema not found");
        }

        // Synchronize the stored fields with the full field list from the update request, in which some fields may have been added or removed

        schema.SchemaTypeName = request.SchemaTypeName;
        var existingFields = schema.Fields.ToDictionary(field => field.Id);
        var requestFieldIds = request.Fields
            .Where(field => field.Id.HasValue)
            .Select(field => field.Id!.Value)
            .ToHashSet();

        schema.Fields.RemoveAll(field => !requestFieldIds.Contains(field.Id));

        foreach (SchemaFieldUpdateRequest fieldRequest in request.Fields)
        {
            if (fieldRequest.Id.HasValue &&
                existingFields.TryGetValue(fieldRequest.Id.Value, out SchemaField? existingField))
            {
                existingField.Name = fieldRequest.Name;
                existingField.Type = fieldRequest.Type;
                continue;
            }

            schema.Fields.Add(new SchemaField
            {
                Name = fieldRequest.Name,
                Type = fieldRequest.Type,
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteSchema(Guid userId, Guid projectId, Guid schemaId)
    {
        Schema? schema = await _db.Schemas.FirstOrDefaultAsync(
            (schema) =>
                schema.Id == schemaId &&
                schema.ProjectId == projectId &&
                schema.Project.OwnerId == userId
        );

        if (schema is null)
        {
            throw new NotFoundException("Schema not found");
        }

        _db.Schemas.Remove(schema);
        await _db.SaveChangesAsync();
    }

    private async Task EnsureProjectBelongsToUser(Guid userId, Guid projectId)
    {
        bool isProjectBelongsToUser = await _db.Projects.AnyAsync((project) =>
            project.Id == projectId &&
            project.OwnerId == userId
        );

        if (!isProjectBelongsToUser)
        {
            throw new IncorrectProjectOwnerException("Project does not belong to the user");
        }
    }
}
