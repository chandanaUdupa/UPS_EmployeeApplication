using EmpClient.DTOClasses;
using EmpClient.Service;
using EmpClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace EmployeeApp.BusinessLogic
{
    class EmployeeBusinessLogic
    {

        /// <summary>
        /// Fetches employee details
        /// </summary>
        public (List<Employee>, int, int, int) GetEmployeeDetails(int pageNumber)
        {
            List<Employee> employeeList = new List<Employee>();
            int start = 0;
            int end = 0;
            int totalItems = 0;
            try
            {
                var employeeDetails = WebAPI.GetEmployeesData(pageNumber);
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                    APISuccessResponseObjectWhenGet responseObj = JsonConvert.DeserializeObject<APISuccessResponseObjectWhenGet>(res);
                    employeeList = responseObj.data;
                    start = ((pageNumber - 1) * responseObj.Meta.Pagination.Limit);
                    end = (pageNumber * responseObj.Meta.Pagination.Limit);
                    totalItems = responseObj.Meta.Pagination.Total;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (employeeList, start, end, totalItems);
        }


        /// <summary>
        /// Fetches employee details for the passed id
        /// </summary>
        public List<Employee> SearchEmployeeDetailsById(int employeeId)
        {
            List<Employee> empList = new List<Employee>();
            try
            {
                var employeeDetails = WebAPI.SearchEmployeesData(employeeId);
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                    Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                    APISuccessResponseObject responseObj = JsonConvert.DeserializeObject<APISuccessResponseObject>(res);

                    empList.Add(responseObj.data);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return empList;
        }


        /// <summary>
        /// Adds new employee
        /// </summary>
        public string CreateNewEmployee(Employee newEmployee)
        {
            string responseMessage = string.Empty;
            try
            {
                var employeeDetails = WebAPI.CreateEmployeeData(newEmployee);

                string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                responseMessage = DeserializeResponseObjectHelper(res, types, "Create", newEmployee);
            }
            catch (Exception)
            {
                throw;
            }
            return responseMessage;
        }



        /// <summary>
        /// Updates employee's record
        /// </summary>
        /// <param name="employee"></param>
        public string UpdateEmployeeDetails(Employee employee)
        {
            string responseMessage = string.Empty;
            try
            {
                var employeeDetails = WebAPI.UpdateEmployeeData(employee.id, employee);
                string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                responseMessage = DeserializeResponseObjectHelper(res, types, "Update", employee);
            }
            catch (Exception)
            {
                throw;
            }
            return responseMessage;
        }

        /// <summary>
        /// Deletes employee's record
        /// </summary>
        /// <param name="employee"></param>
        public string DeleteEmployeeDetails(int employeeId)
        {
            string responseMessage = string.Empty;
            try
            {
                var employeeDetails = WebAPI.DeleteEmployeeData(employeeId);
                string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                responseMessage = DeserializeResponseObjectHelper(res, types, "Delete");
            }
            catch (Exception)
            {
                throw;
            }
            return responseMessage;
        }

        /// <summary>
        /// Exports the entire employee data to CSV file
        /// in the same directory as the project
        /// </summary>
        public bool ExportToCSV()
        {
            bool isExportedSuccesfully = false;
            try
            {
                List<Employee> exportToCSVList = new List<Employee>();
                var employeeDetails = WebAPI.GetEmployeesData();
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;

                    APISuccessResponseObjectWhenGet responseObj = JsonConvert.DeserializeObject<APISuccessResponseObjectWhenGet>(res);
                    exportToCSVList = responseObj.data;
                    for (int pageNumber = 2; pageNumber <= responseObj.Meta.Pagination.Pages; pageNumber++)
                    {
                        var paginatedEmployeeDetails = WebAPI.GetEmployeesData(pageNumber);
                        if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string paginatedResult = paginatedEmployeeDetails.Result.Content.ReadAsStringAsync().Result;
                            APISuccessResponseObjectWhenGet paginatedResponseObj = JsonConvert.DeserializeObject<APISuccessResponseObjectWhenGet>(paginatedResult);
                            exportToCSVList.AddRange(paginatedResponseObj.data);
                        }
                    }
                    DataTable EmployeeTable = GetTable(exportToCSVList);
                    string workingDirectory = Environment.CurrentDirectory;
                    string startupPath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                    CSVUtlity.ToCSV(EmployeeTable, startupPath + "\\" + ConfigurationManager.AppSettings["ExportFileName"]);
                    isExportedSuccesfully = true;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return isExportedSuccesfully;
        }


        /// <summary>
        /// Create DataTable to extract employee details to CSV
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        static DataTable GetTable(List<Employee> employees)
        {
            DataTable table = new DataTable();
            Type type = typeof(Employee);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                table.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (Employee emp in employees)
            {
                table.Rows.Add(properties[0].GetValue(emp, null), properties[1].GetValue(emp, null), properties[2].GetValue(emp, null), properties[3].GetValue(emp, null), properties[4].GetValue(emp, null), properties[5].GetValue(emp, null), properties[6].GetValue(emp, null));
            }

            return table;
        }

        public static string DeserializeResponseObjectHelper(string res, Type[] types, string operation, Employee employee = null)
        {
            object obj = null;
            string resMessage = String.Empty;

            if (JsonDeserializer.TryDeserialize(res, out obj, types))
            {
                if (obj is APISuccessResponseObject)
                {
                    var responseObj = obj as APISuccessResponseObject;
                    switch (operation)
                    {
                        case "Create":
                            if (responseObj.Code == (int)System.Net.HttpStatusCode.Created)
                            {
                                resMessage = employee.name + ConfigurationManager.AppSettings["CreateEmployeeSuccessMessage"] + responseObj.data.id;
                            }
                            else
                            {
                                resMessage = employee.name + ConfigurationManager.AppSettings["CreateEmployeeFailureMessage"];
                            }
                            break;
                        case "Update":
                            if (responseObj.Code == (int)System.Net.HttpStatusCode.OK)
                            {
                                resMessage = employee.name + ConfigurationManager.AppSettings["UpdateEmployeeSuccessMessage"] + responseObj.data;
                            }
                            else
                            {
                                resMessage = employee.name + ConfigurationManager.AppSettings["UpdateEmployeeFailureMessage"];
                            }
                            break;
                        case "Delete":
                            if (responseObj.Code == (int)System.Net.HttpStatusCode.NoContent)
                            {
                                resMessage = ConfigurationManager.AppSettings["DeleteEmployeeSuccessMessage"];
                            }
                            else
                            {
                                resMessage = ConfigurationManager.AppSettings["CreateEmployeeFailureMessage"];
                            }
                            break;
                    }
                }
                else if (obj is APIErrorResponseObjectMultipleErrorMessages)
                {
                    var responseObj = obj as APIErrorResponseObjectMultipleErrorMessages;
                    resMessage = string.Empty;
                    foreach (var failureMessage in responseObj.data)
                    {
                        resMessage += failureMessage.field + " " + failureMessage.message;
                    }
                }
                else
                {
                    var responseObj = obj as APIErrorResponseObject;
                    resMessage = responseObj.data.field + " " + responseObj.data.message;
                }
            }
            return resMessage;
        }

    }
}
