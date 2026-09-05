BEGIN;

DO $$
BEGIN
    IF to_regclass('public.schemas') IS NOT NULL
       AND EXISTS (
           SELECT 1
           FROM pg_attribute
           WHERE attrelid = 'public.schemas'::regclass
             AND attname = 'name'
             AND NOT attisdropped
       )
       AND NOT EXISTS (
           SELECT 1
           FROM pg_attribute
           WHERE attrelid = 'public.schemas'::regclass
             AND attname = 'schema_type_name'
             AND NOT attisdropped
       ) THEN
        ALTER TABLE schemas RENAME COLUMN name TO schema_type_name;
    END IF;
END $$;

COMMIT;
