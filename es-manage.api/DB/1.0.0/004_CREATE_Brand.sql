CREATE TABLE IF NOT EXISTS public."brand" (
    "id" character varying(100),
    "name" character varying(255),
    "deleted" boolean DEFAULT false,
    "createdon" timestamp,
    "createdby" character varying(25),
    "modifiedon" timestamp,
    "modifiedby" character varying(25),
    PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public."brand"
    OWNER to es;