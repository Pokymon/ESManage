CREATE TABLE IF NOT EXISTS public."itemsupplier" (
    "id" character varying(100),
    "itemid" character varying(100),
    "supplierid" character varying(100),
    "receiptdate" timestamp,
    "returndate" timestamp,
    "deleted" boolean DEFAULT false,
    "createdon" timestamp,
    "createdby" character varying(25),
    "modifiedon" timestamp NULL,
    "modifiedby" character varying(255) NULL,
    PRIMARY KEY ("id"),
    FOREIGN KEY ("itemid") REFERENCES public."item" ("id"),
    FOREIGN KEY ("supplierid") REFERENCES public."supplier" ("id") ON DELETE SET NULL
);

ALTER TABLE public."itemsupplier"
    OWNER to es;