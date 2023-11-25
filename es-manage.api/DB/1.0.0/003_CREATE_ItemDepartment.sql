CREATE TABLE IF NOT EXISTS public."itemdepartment" (
    "id" character varying(100),
    "categoryname" character varying(100),
    "itemdepartmentparentid" character varying(10) DEFAULT '0',
    "deleted" boolean DEFAULT false,
    "createdon" timestamp,
    "createdby" character varying(25),
    "modifiedon" timestamp,
    "modifiedby" character varying(25),
    PRIMARY KEY ("id", "categoryname")
);

ALTER TABLE IF EXISTS public."itemdepartment"
    OWNER to es;