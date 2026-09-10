DO $$
BEGIN
    IF to_regclass('public.schemas') IS NOT NULL
       AND NOT EXISTS (
           SELECT 1
           FROM pg_constraint
           WHERE conname = 'uq_schemas_project_id'
             AND conrelid = 'public.schemas'::regclass
       ) THEN
        ALTER TABLE schemas
            ADD CONSTRAINT uq_schemas_project_id UNIQUE (project_id, id);
    END IF;

    IF to_regclass('public.schemas') IS NOT NULL
       AND NOT EXISTS (
           SELECT 1
           FROM pg_constraint
           WHERE conname = 'uq_schemas_project_schema_type_name'
             AND conrelid = 'public.schemas'::regclass
       ) THEN
        ALTER TABLE schemas
            ADD CONSTRAINT uq_schemas_project_schema_type_name UNIQUE (project_id, schema_type_name);
    END IF;
END $$;
