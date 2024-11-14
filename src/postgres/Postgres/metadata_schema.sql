-- [up]
create schema if not exists __schema__;
-- [go]

create table __schema__.log(
    logid           uuid not null,
    migrationid     uuid not null,
    dateapplied     timestamp not null,
    operation       text not null,
    agent           text not null,
    host            text not null,
    constraint pk__log primary key(logid)
);
-- [go]
create table __schema__.migrations(
    migrationid     uuid not null,
    dbversion       text not null,
    logid           uuid not null,
    sourcepath      text not null,
    sourcefile      text not null,
    sha             text not null,
    constraint pk__migrations primary key(migrationid),
    constraint fk__migrations_log foreign key(logid) references __schema__.log(logid),
    constraint uq__dbversion unique(dbversion)
);
-- [go]
create table __schema__.blobs(
    migrationid     uuid not null,
    compression     text not null,
    encoding        text not null,
    content         bytea not null,
    contentlength   bigint not null,
    constraint pk__blobs primary key(migrationid),
    constraint fk__blobs_migrations foreign key(migrationid) 
        references __schema__.migrations(migrationid)
        on delete cascade
);
-- [go]
create table __schema__.metadata(
    migrationid     uuid not null,
    key             text not null,
    value           text not null,
    constraint pk__metadata primary key(migrationid, key),
    constraint fk__metadata_migrations foreign key(migrationid)
        references __schema__.migrations(migrationid)
        on delete cascade                                
);
-- [go]
create table __schema__.metrics(
    migrationid     uuid not null,
    category        text not null,
    key             text not null,
    value           text not null,
    constraint pk__metrics primary key(migrationid, key),
    constraint fk__metrics_migrations foreign key(migrationid)
        references __schema__.migrations(migrationid)
        on delete cascade
);
-- [go]
create view __schema__.migrations_view as
select src.*
from pgfwd_meta.migrations src
where not exists (
    select *
    from __schema__.metadata md
    where md.migrationid = src.migrationid
      and md.key = 'pgfwd/internalMigration'
      and md.value = 'true'
);    
-- [/up]

-- [down]
drop schema __schema__ cascade;    
-- [/down]