BEGIN;

ALTER TABLE IF EXISTS graph_documents
    RENAME TO graphs;

ALTER TABLE IF EXISTS graphs
    RENAME CONSTRAINT fk_graph_documents_project TO fk_graphs_project;

COMMIT;
