using System.Data;
using Dapper;
using DbForward.Constants;
using DbForward.Logging;
using Microsoft.Extensions.Logging;

namespace DbForward.Utilities;

public static class Operation
{
    public static async Task<OperationResponse> TryExecuteCommandAsync(
        IDbConnection connection,
        string command,
        object? parameters,
        IDbTransaction? transaction,
        ILogger logger,
        LogLevel logLevel,
        OperationResponse errorResponse = OperationResponse.Aborted)
    {
        try
        {
            await connection.ExecuteAsync(command, parameters, transaction);

            return LogResponse(
                OperationResponse.Successful,
                command,
                logger,
                logLevel);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error caught while executing operation");
            return LogResponse(errorResponse, command, logger, logLevel);
        }
    }
    
    public static async Task<OperationResponse> TryExecuteAsync(
        Func<Task<OperationResponse>> asyncOperation,
        string logStatement,
        ILogger logger,
        LogLevel logLevel,
        OperationResponse errorResponse = OperationResponse.Aborted)
    {
        try
        {
            return LogResponse(
                await asyncOperation(),
                logStatement,
                logger,
                logLevel);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error caught while executing operation");
            return LogResponse(errorResponse, logStatement, logger, logLevel);
        }
    }

    private static OperationResponse LogResponse(OperationResponse response, 
        string logStatement, 
        ILogger logger, 
        LogLevel logLevel)
    {
        logger.Log(
            response == OperationResponse.Successful ? logLevel : LogLevel.Error,
            "Operation {verb}:\n{statement}",
            response == OperationResponse.Successful ? "executed" : "attempted",
            new VerboseToken(logStatement));

        return response;
    }
}