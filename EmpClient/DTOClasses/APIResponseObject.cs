using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EmpClient.DTOClasses
{
    public class APIResponseObject
    {
        #region Properties
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
        #endregion
    }
    public class Pagination
    {
        #region Properties
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("pages")]
        public int Pages { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        #endregion
    }

    public class Meta
    {
        #region Properties
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }
        #endregion
    }
    public class APIMessage
    {
        #region Properties
        public string field { get; set; }
        public string message { get; set; }
        #endregion
    }

    public class APISuccessResponseObject : APIResponseObject
    {
        #region Properties
        public Employee data { get; set; }
        #endregion
    }

    public class APISuccessResponseObjectWhenGet : APIResponseObject
    {
        #region Properties
        public List<Employee> data { get; set; }
        #endregion
    }

    public class APIErrorResponseObject : APIResponseObject
    {
        #region Properties
        public APIMessage data { get; set; }
        #endregion
    }

    public class APIErrorResponseObjectMultipleErrorMessages : APIResponseObject
    {
        #region Properties
        public List<APIMessage> data
        {
            get; set;
            #endregion
        }
    }
}
