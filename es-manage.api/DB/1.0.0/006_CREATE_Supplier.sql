CREATE TABLE IF NOT EXISTS public."supplier" (
    "id" character varying(100),
    "suppliername" character varying(255),
    "deleted" boolean DEFAULT false,
    "createdon" timestamp,
    "createdby" character varying(25),
    "modifiedon" timestamp NULL,
    "modifiedby" character varying(25) NULL,
    PRIMARY KEY ("id")
);

ALTER TABLE IF EXISTS public."supplier"
    OWNER to es;