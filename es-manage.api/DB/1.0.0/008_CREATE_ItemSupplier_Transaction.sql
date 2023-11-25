CREATE TABLE IF NOT EXISTS public."itemsupplier_transaction" (
    "id" character varying(100),
    "itemsupplierid" character varying(100),
    "transactiontype" character varying(50),
    "transactiondate" timestamp,
    "quantity" numeric(18,2),
    "notes" text NULL,
    "deleted" boolean DEFAULT false,
    "createdon" timestamp,
    "createdby" character varying(50),
    "modifiedon" timestamp NULL,
    "modifiedby" character varying(50) NULL,
    PRIMARY KEY ("id"),
    FOREIGN KEY ("itemsupplierid") REFERENCES public."itemsupplier" ("id")
);

ALTER TABLE public."itemsupplier_transaction"
    OWNER to es;