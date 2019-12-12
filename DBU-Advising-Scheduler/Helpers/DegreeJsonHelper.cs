using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json;
using DBU_Advising_Scheduler.Models;

namespace DBU_Advising_Scheduler.Helpers
{
    public class DegreeJsonHelper
    {

        public string GetFields(Type modelType)
        {
            return string.Join(",",
                modelType.GetProperties()
                         .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                         .Select(jp => jp.PropertyName));
        }
    }
}