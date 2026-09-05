BEGIN;

UPDATE graphs
SET content = '{"nodes":[],"edges":[]}'::jsonb
WHERE content IS NULL OR content = '{}'::jsonb;

ALTER TABLE graphs
    ALTER COLUMN content SET DEFAULT '{"nodes":[],"edges":[]}'::jsonb,
    ALTER COLUMN content SET NOT NULL;

COMMIT;
