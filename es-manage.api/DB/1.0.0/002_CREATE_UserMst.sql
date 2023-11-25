CREATE TABLE IF NOT EXISTS public."usermst" (
    "id" UUID PRIMARY KEY,
    "username" VARCHAR(25) NOT NULL,
    "displayname" VARCHAR(50),
    "password" VARCHAR(250) NOT NULL,
    "passwordsalt" VARCHAR(250),
    "createdon" TIMESTAMP,
    "createdby" VARCHAR(25),
    "modifiedon" TIMESTAMP,
    "modifiedby" VARCHAR(25),
    "deletedat" TIMESTAMP,
    "roleid" VARCHAR(25),
    CONSTRAINT fk_user_role FOREIGN KEY ("roleid") REFERENCES "role"("id") ON DELETE SET NULL
);

ALTER TABLE IF EXISTS public."usermst"
    OWNER to es;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

INSERT INTO public."usermst" (
    "id",
    "username",
    "displayname",
    "password",
    "passwordsalt",
    "createdon",
    "createdby",
    "modifiedon",
    "modifiedby",
    "deletedat",
    "roleid"
) VALUES (
    uuid_generate_v4(),
    'admin',
    'Default Administrator',
    'QZelWwn5LIpZ1AJDhRIzSEpkW7Efy1MZAVk3nW2K0fJZ5jcmj8/hQa8QXrFFwPrysYn/2wIu9YioL0KOOBRPkA==',
    'gTkmXTZrOzbv79+g0vWuNGA7cJ+EBDvyK0asZ5lCA7mi9WnWIdWP4nGWr952QE2AszbWW2+4RvTx6n0FDebXoA==',
    NOW(),
    'System',
    null,
    null,
    null,
    '1'
);