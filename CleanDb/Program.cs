using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CleanDb
{
  class Program
  {
    static void Main(string[] args)
    {

      var result = Parser.Default.ParseArguments<Options>(args).WithParsed(options => 
      {
        var connectionString = DeletectConnectionString(options);
        if(connectionString == null)
        {
          ExitWithError(
            "Missing connection string in {0} at {1}[{2}]",
            options.ConfigPath, options.UseAppSettings ? "AppSettings" : "ConnectionStrings",
            options.ConnectionName
          );
        }

        var limitDate = BuildDateWithTimeOffset(options);

        var db = CreateDbAdapter(connectionString);
        var delete = db.CreateDelete();
        delete.From = options.TableName;
        delete.Where.Add(options.ColumnName, limitDate, Sdx.Db.Sql.Comparison.LessThan);

        var command = delete.Build();

        using(var conn = db.CreateConnection())
        {
          conn.Open();
          using(conn.BeginTransaction())
          {
            try
            {
              var count = conn.ExecuteNonQuery(command);
              conn.Commit();

              var format = @"
Delete {0} records.

SQL: {1}
@0:  {2}
";
              Console.WriteLine(string.Format(format, count, command.CommandText, limitDate.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            catch (Exception ex)
            {
              ExitWithError(
                "Fail to delete: {0}",
                Sdx.Diagnostics.Debug.BuildLogString(ex)
              );
              throw;
            }
          }
        }
        
      });
    }

    private static DateTime BuildDateWithTimeOffset(Options options)
    {
      var regex = new Regex(@"^([0-9]+)(h|d|m)$");
      var match = regex.Match(options.TimeOffset);
      
      if(!match.Success)
      {
        var attr = (BaseAttribute)options.GetType().GetProperty("TimeOffset").GetCustomAttributes(typeof(BaseAttribute), false).First();
        ExitWithError(
          "Illega format time span `{0}`, help: {1}",
          options.TimeOffset,
          attr.HelpText
        );
      }

      var number = Int32.Parse(match.Groups[1].Value);
      switch(match.Groups[2].Value)
      {
        case "h":
          return DateTime.Now.AddHours(-number);
        case "d":
          return DateTime.Now.AddDays(-number);
        case "m":
          return DateTime.Now.AddMonths(-number);
        default:
          throw new Exception("正規表現に一致してるのでここには来ないはず");
      }
    }

    private static Sdx.Db.Adapter.Base CreateDbAdapter(string connectionString)
    {
      var db = new Sdx.Db.Adapter.SqlServer();
      db.ConnectionString = connectionString;
      return db;
    }

    private static void ExitWithError(string message, params string[] values)
    {
      Console.Error.WriteLine("Error: " + string.Format(message, values));
      Environment.Exit(1);
    }

    private static string DeletectConnectionString(Options options)
    {
      //configを読み込む
      ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();

      fileMap.ExeConfigFilename = options.ConfigPath;
      System.Configuration.Configuration config =
          ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

      string connectionString = null;
      if (options.UseAppSettings)
      {
        var kv = config.AppSettings.Settings[options.ConnectionName];
        if (kv != null)
        {
          connectionString = kv.Value;
        }
      }
      else
      {
        var kv = config.ConnectionStrings.ConnectionStrings[options.ConnectionName];
        if (kv != null)
        {
          connectionString = kv.ConnectionString;
        }
      }

      return connectionString;
    }
  }
}
