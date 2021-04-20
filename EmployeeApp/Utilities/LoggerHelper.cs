using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeApp.Utilities
{
    class LoggerHelper
    {
        readonly public static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
