using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Db4Wd.Postgres.Schema.V1;

internal static class Install
{
    internal static async Task ExecuteAsync(NpgsqlConnection connection,
       ILogger logger,
       CancellationToken cancellationToken)
    {
       logger.LogDebug("Starting transaction scope");
       await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
       
        const string schema = Constants.SchemaName;
        
        const string sql =
            $"""
             create schema {schema};
             
             create sequence {schema}.id_sequence
                as integer
                increment by 1
                start with 1;
                
             create table {schema}.log(
                logid               uuid not null,
                migrationid         uuid not null,
                dbversionid         int not null,
                dateapplied         timestamp not null,
                operation           text not null,
                result              text not null,
                error               text null,
                statementcount      integer not null,
                transactioncount    integer not null,
                rowsaffected        bigint not null,
                agent               text not null,
                host                text not null,
                primary key(logid)
             );
             
             create index log_migrationid_idx
             on {schema}.log using btree(migrationid);
             
             create table {schema}.migrations(
                migrationid         uuid not null,
                logid               uuid not null,
                primary key(migrationid),
                foreign key(logid) references {schema}.log(logid)
             );
             
             create table {schema}.blobs(
                migrationid         uuid not null,
                path                text not null,
                filename            text not null,
                sha                 text not null,
                binarylength        bigint not null,
                compression         text not null,
                encoding            text not null,
                primary key(migrationid),
                foreign key(migrationid) references {schema}.migrations(migrationid)
             );
             
             create table {schema}.metadata(
               migrationid          uuid not null,
               key                  text not null,
               value                text not null,
               primary key(migrationid, key),
               foreign key(migrationid) references {schema}.migrations(migrationid)
             );
             
             create index metadata_key_idx
             on {schema}.metadata using btree(key);
             
             create table {schema}.locks(
               leasetype            text not null,
               lockid               uuid not null,
               dateacquired         timestamp not null,
               agent                text not null,
               host                 text not null,
               primary key(leasetype)
             );
             
             create table {schema}.versions(
               major       int not null,
               minor       int not null,
               revision    int not null,
               dateapplied timestamp not null,
               primary key(major, minor, revision)
             );
             
             insert into {schema}.versions(major, minor, revision, dateapplied)
             values(1, 0, 0, CURRENT_TIMESTAMP);
             """;

        logger.LogDebug("Executing schema DDL");

        if (logger.IsEnabled(LogLevel.Trace))
        {
           logger.LogTrace("\n{ddl}", sql);
        }
        
        await connection.ExecuteAsync(sql, commandTimeout: 5, transaction: transaction);
        
        logger.LogDebug("Committing transaction");
        await transaction.CommitAsync(cancellationToken);
    }
}