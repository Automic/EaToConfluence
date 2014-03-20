using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Diagnostics;

namespace NancyEaWebApi
{
    public class EaDiagnosticsProvider : IDiagnosticsProvider
    {
        public string Name
        {
            get { return "EA cache diagnostics"; }
        }

        public string Description
        {
            get { return "Allows to clear the EA repository cache."; }
        }

        public object DiagnosticObject
        {
            get { return this; }
        }

        [Description("Clears the EA cachces")]
        [Template("<h1>Done</h1>")]
        public void ClearCache()
        {
            Console.WriteLine("clear ea caches");
            IndexModule.ClearCache();
        }
    }
}
