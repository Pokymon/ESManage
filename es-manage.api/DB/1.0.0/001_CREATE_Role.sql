CREATE TABLE IF NOT EXISTS public."role" (
    "id" VARCHAR(100) PRIMARY KEY,
    "rolename" VARCHAR(50) NOT NULL,
    "createdon" TIMESTAMP,
    "createdby" VARCHAR(25),
    "modifiedon" TIMESTAMP,
    "modifiedby" VARCHAR(25),
    "deleted" BOOLEAN DEFAULT FALSE
);

-- Mengubah pemilik tabel "role" ke pengguna 'es'
ALTER TABLE public."role" OWNER TO es;

-- Menambahkan nilai awal untuk peran "Administrator"
-- Jika sudah ada baris dengan "id" = 1, kita akan mengabaikannya.
-- Asumsi di sini adalah bahwa "id" = 1 selalu milik "Administrator".
INSERT INTO public."role" ("id", "rolename", "createdby", "createdon", "deleted")
VALUES ('1', 'Administrator', 'System', NOW(), FALSE)
ON CONFLICT ("id") DO NOTHING;

-- Jika sudah ada peran dengan nama "Administrator" namun id berbeda, ubah nama tersebut.
UPDATE public."role" SET "rolename" = 'OldRole' WHERE "rolename" = 'Administrator' AND "id" <> '1';

-- Membuat fungsi trigger untuk mencegah penghapusan atau pengeditan baris dengan "rolename" 'Administrator'
CREATE OR REPLACE FUNCTION prevent_administrator_alteration()
RETURNS TRIGGER AS $$
BEGIN
    -- Jika ada percobaan menghapus atau mengubah "rolename" 'Administrator', lempar kesalahan.
    IF (TG_OP = 'DELETE' AND OLD."rolename" = 'Administrator') OR
       (TG_OP = 'UPDATE' AND NEW."rolename" <> 'Administrator' AND OLD."rolename" = 'Administrator') THEN
        RAISE EXCEPTION 'Tidak dapat menghapus atau mengubah nama peran "Administrator".';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Membuat trigger yang menggunakan fungsi "prevent_administrator_alteration"
CREATE TRIGGER trigger_prevent_administrator_alteration
BEFORE DELETE OR UPDATE OF "rolename" ON public."role"
FOR EACH ROW WHEN (OLD."rolename" = 'Administrator')
EXECUTE FUNCTION prevent_administrator_alteration();

INSERT INTO public."role" ("id", "rolename", "createdby", "createdon", "deleted")
VALUES ('2', 'User', 'System', NOW(), FALSE)
ON CONFLICT ("id") DO NOTHING;