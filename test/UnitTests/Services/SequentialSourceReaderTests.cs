using DbForward.Services;
using Shouldly;

namespace UnitTests.Services;

public class SequentialSourceReaderTests
{
    private readonly UnitSourceReader unit = new();
    
    [Fact]
    public async Task Read_Header()
    {
        using var reader = new StringReader(
            """
            -- [head]
            -- [id: 5e366d33-5209-4fc5-bac6-1529bc5e95d4]
            -- [dbVersion: 20241113-120000]
            -- [metadata.author: testy]
            -- [/head]
            """
        );

        var header = await unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None);
        
        header.MigrationId.ShouldBe(Guid.Parse("5e366d33-5209-4fc5-bac6-1529bc5e95d4"));
        header.DbVersion.ShouldBe("20241113-120000");
        header.Metadata["author"].ShouldBe("testy");
    }

    [Fact]
    public async Task Read_Header_With_No_Id_Throws()
    {
        using var reader = new StringReader(
            """
            -- [head]
            -- [dbVersion: 20241113-202500]
            -- [metadata.author: testy]
            -- [/head]
            """
        );

        await Should.ThrowAsync<Exception>(() => unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None));
    }
    
    [Fact]
    public async Task Read_Header_With_No_Version_Throws()
    {
        using var reader = new StringReader(
            """
            -- [head]
            -- [id: 5e366d33-5209-4fc5-bac6-1529bc5e95d4]
            -- [metadata.author: testy]
            -- [/head]
            """
        );

        await Should.ThrowAsync<Exception>(() => unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None));
    }
    
    [Fact]
    public async Task Read_Header_With_Duplicate_Metadata_Keys_Throws()
    {
        using var reader = new StringReader(
            """
            -- [head]
            -- [id: 5e366d33-5209-4fc5-bac6-1529bc5e95d4]
            -- [dbVersion: 20241113-203000]
            -- [metadata.author: testy]
            -- [metadata.author: mctesterson]
            -- [/head]
            """
        );

        await Should.ThrowAsync<Exception>(() => unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None));
    }
    
    [Fact]
    public async Task Read_Header_With_Invalid_Close_Tag_Throws()
    {
        using var reader = new StringReader(
            "-- [/head]"
        );

        await Should.ThrowAsync<Exception>(() => unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None));
    }
    
    [Fact]
    public async Task Read_Header_With_Invalid_Open_Tag_Throws()
    {
        using var reader = new StringReader(
            """
            -- [head]
            -- [id: 5e366d33-5209-4fc5-bac6-1529bc5e95d4]
            -- [dbVersion: 2024-11-13.00005]
            -- [head]
            -- [metadata.author: testy]
            -- [/head]
            """
        );

        await Should.ThrowAsync<Exception>(() => unit.ReadHeaderAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None));
    }

    [Fact]
    public async Task Reads_Terminated_Statement()
    {
        using var reader = new StringReader(
            """
            -- [up]
            select * from dual;
            -- [/up]
            """
        );

        var result = await unit.ReadMigrationDirectivesAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None);
        
        result.Single().Text.Trim().ShouldBe("select * from dual;");
    }

    [Fact]
    public async Task Reads_Partial_Statements()
    {
        using var reader = new StringReader(
            """
            -- [up]
            select * from dual
            where 1=1;
            -- [/up]
            """
        );

        var result = await unit.ReadMigrationDirectivesAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None);

        result.Single().Text.Trim().ShouldBe(
            """
            select * from dual
            where 1=1;
            """);
    }

    [Fact]
    public async Task Reads_Custom_Directive()
    {
        using var reader = new StringReader(
            """
            -- [up]
            -- [custom]
            -- [/up]
            """);
        
        var result = await unit.ReadMigrationDirectivesAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None);

        result.Single().ShouldBeOfType<CustomDirective>();
    }

    [Fact]
    public async Task Reads_Rollback_Directives()
    {
        using var reader = new StringReader(
            """
            -- [down]
            drop table if exists accounts;
            -- [go]
            drop schema if exists core;
            -- [/down]
            """);

        var result = await unit.ReadRollbackDirectivesAsync(reader, "path", TestData.EmptyDictionary,
            CancellationToken.None);

        result[0].Text.ShouldBe(
            """
            drop table if exists accounts;
            
            """);

        result[1].Text.ShouldBe(
            """
            drop schema if exists core;

            """);

        result.Count.ShouldBe(2);
    }
}

internal sealed class CustomDirective() : StatementSectionDirective("")
{
}