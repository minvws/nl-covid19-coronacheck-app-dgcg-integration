-- Table: public.TestType

-- DROP TABLE public."TestType";

CREATE TABLE public."TestType"
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    uuid uuid NOT NULL,
    name character varying(50) COLLATE pg_catalog."default",
    CONSTRAINT "TestType_pkey" PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE public."TestType"
    OWNER to postgres;
