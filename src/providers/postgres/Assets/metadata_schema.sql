create schema __schema__;

create table __schema__.log(
    log_id              uuid not null,
    migration_id        uuid not null,
    apply_date          timestamp not null,
    source_path         text not null,
    source_file         text not null,
    sha                 text not null,
    error_message       text null,
    constraint pk_log primary key(log_id)
);

create table __schema__.migrations(
    migration_id        uuid not null,
    log_id              uuid not null,
    constraint pk_migrations primary key(migration_id),
    constraint fk_migrations_log foreign key(log_id) references __schema__.log(log_id)
);

create table __schema__.metadata(
    migration_id        uuid not null,
    key                 text not null,
    value               text not null,
    constraint pk_metadata primary key(migration_id, key),
    constraint fk_metadata_migrations foreign key(migration_id) references __schema__.migrations(migration_id)
);

create table __schema__.blobs(
    migration_id        uuid not null,
    content_bytes       byteea not null,
    compression         text not null,
    encoding            text not null,
    constraint pk_blobs primary key(migration_id),
    constraint fk_blobs_migrations foreign key(migration_id) references __schema__.migrations(migration_id)
);