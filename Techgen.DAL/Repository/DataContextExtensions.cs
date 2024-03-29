﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.DAL.Abstract;

namespace Techgen.DAL.Repository
{
    public static class DataContextExtensions
    {
        public static IEnumerable<T> GetDataFromSqlCommand<T>(this IDataContext context, string command, Dictionary<string, object> parameters) where T : class
        {
            // Get parameters from dictionary
            SqlParameter[] sqlParameters = parameters.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();

            // Create execute string
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"EXECUTE {command}");
            strBuilder.Append(string.Join(",", sqlParameters.Select(s => $" @{s.ParameterName}")));

            // Check if context has such dbset or dbquery
            var properties = context.GetType().GetProperties();

            if (properties.Any(x => x.PropertyType == typeof(DbSet<T>)))
                return context.Set<T>().FromSqlRaw(strBuilder.ToString(), sqlParameters);
            else
                throw new Exception("Invalid type parameter: no such DbSet/DbQuery parameter in DataContext");
        }
    }
}
