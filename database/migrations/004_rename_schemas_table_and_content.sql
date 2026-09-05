BEGIN;

DO $$
BEGIN
    IF to_regclass('public.node_schemas') IS NOT NULL
       AND to_regclass('public.schemas') IS NULL THEN
        ALTER TABLE node_schemas RENAME TO schemas;
    END IF;

    IF to_regclass('public.schemas') IS NOT NULL
       AND EXISTS (
           SELECT 1
           FROM pg_attribute
           WHERE attrelid = 'public.schemas'::regclass
             AND attname = 'schema'
             AND NOT attisdropped
       ) THEN
        ALTER TABLE schemas RENAME COLUMN schema TO content;
    END IF;

    IF to_regclass('public.schemas') IS NOT NULL
       AND EXISTS (
           SELECT 1
           FROM pg_constraint
           WHERE conname = 'fk_node_schemas_project'
             AND conrelid = 'public.schemas'::regclass
       ) THEN
        ALTER TABLE schemas RENAME CONSTRAINT fk_node_schemas_project TO fk_schemas_project;
    END IF;
END $$;

COMMIT;
