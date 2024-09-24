# Sharkable.AutoCrud
automatic generate crud operations api for Sharkable based minimal api framework

### Note this is a Sharkable based extension and WIP, just for testing users

## Usage
```csharp
//in your sharkable based program for aot mode
builder.Services.AddShark([typeof(Program).Assembly, typeof(Sharkable.AutoCrudSqlSugar).Assembly], opt=>{
    opt.Format = Sharkable.EndpointFormat.Tolower;
    opt.ConfigureAutoCrud(s =>
    {
        s.IsAutoCloseConnection = true;
        s.DbType = DbType.Sqlite;
        s.ConnectionString = "DataSource=testsample.db";
    });
});
```
