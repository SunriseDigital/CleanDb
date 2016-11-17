using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace CleanDb
{
  public class Options
  {
    [Option('a', "appsettings", HelpText = "Use connection string in AppSettings section.")]
    public bool UseAppSettings { get; set; }
   
    [Value(0, Required = true, HelpText = "Path to config file.")]
    public string ConfigPath { get; set; }

    [Value(1, Required = true, HelpText = "Connection name.")]
    public string ConnectionName { get; set; }

    [Value(2, Required = true, HelpText = "Table Name.")]
    public string TableName { get; set; }

    [Value(3, Required = true, HelpText = "Date column name.")]
    public string ColumnName { get; set; }

    [Value(4, Required = true, HelpText = "Time offset. #h: hours | #d: days | #m months")]
    public string TimeOffset { get; set; }
  }
}
