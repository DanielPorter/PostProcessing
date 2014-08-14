-- Table: public."07-31-14 15.58.12"

DROP TABLE public."LaserPoints";
DROP SEQUENCE laserpoints_gid_seq;
CREATE SEQUENCE laserpoints_gid_seq START 1;
CREATE TABLE public."LaserPoints"
(
  gid integer NOT NULL DEFAULT nextval('"laserpoints_gid_seq"'::regclass),
  density double precision,
  height double precision,
  skirtht double precision,
  trededge double precision,
  brededge double precision,
  tnir double precision,
  bnir double precision,
  tred double precision,
  bred double precision,
  tndre double precision,
  bndre double precision,
  tndvi double precision,
  bndvi double precision,
  "time" character varying(254),
  latitude character varying(254),
  longitude character varying(254),
  altitude double precision,
  "left" character varying(254),
  distance double precision,
  geom geometry(Point),
  CONSTRAINT "LaserPoints_pkey" PRIMARY KEY (gid )
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."LaserPoints"
  OWNER TO postgres;

-- Index: public."07-31-14 15.58.12_geom_gist"

-- DROP INDEX public."07-31-14 15.58.12_geom_gist";

CREATE INDEX "LaserPoints_geom_gist"
  ON public."LaserPoints"
  USING gist
  (geom );

