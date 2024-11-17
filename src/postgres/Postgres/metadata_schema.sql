-- [up]
create schema if not exists __schema__;
-- [go]
create function __schema__.partial_uuid(id uuid) returns uuid
    language sql
    immutable
    return ('00000000-0000-0000-0000-0000' || substr(id::text, 29))::uuid;

create function __schema__.is_uuid_match(x uuid, y uuid) returns boolean
    language sql
    immutable
    return ((x = y) or (pgfwd_meta.partial_uuid(x) = pgfwd_meta.partial_uuid(y)));
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
select mv.migrationid,
       __schema__.partial_uuid(mv.migrationid) as partialid,
       mv.dbversion,
       mv.logid,
       mv.sourcepath,
       mv.sourcefile,
       mv.sha       
from __schema__.migrations mv
where not exists (
    select *
    from __schema__.metadata md
    where md.migrationid = mv.migrationid
      and md.key = 'pgfwd/internalMigration'
      and md.value = 'true'
);
-- [go]
create view __schema__.log_view as
select l.logid,
       l.migrationid,
       __schema__.partial_uuid(l.migrationid) as partialid,
       l.dateapplied,
       l.operation,
       l.agent,
       l.host
from __schema__.log l
where not exists (
    select *
    from __schema__.metadata md
    where md.migrationid = l.migrationid
      and md.key = 'pgfwd/internalMigration'
      and md.value = 'true'
);
-- [/up]

-- [down]
drop schema __schema__ cascade;    
-- [/down]