-- Table: public.AppConfig

-- DROP TABLE public."AppConfig";

CREATE TABLE public."AppConfig"
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    content text COLLATE pg_catalog."default" NOT NULL,
    type character varying(20) COLLATE pg_catalog."default"
)

TABLESPACE pg_default;

ALTER TABLE public."AppConfig"
    OWNER to postgres;
