using System;
using System.Text.Json.Serialization;

namespace EmpClient.DTOClasses
{
    public class Employee
    {
        #region Properties
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        #endregion

        #region Constructor
        public Employee()
        {
        }
        #endregion
    }
}