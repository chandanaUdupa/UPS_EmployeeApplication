using EmpClient.DTOClasses;
using EmpClient.Service;
using EmpClient.Utilities;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace EmpClient.ViewModels
{
    class MainWindowViewModel : BindableBase, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties
        private List<Employee> _employees;

        public List<Employee> Employees
        {
            get { return _employees; }
            //set { SetProperty(ref _employees, value);
            private set
            {
                if (object.ReferenceEquals(_employees, value) != true)
                {
                    _employees = value;
                    NotifyPropertyChanged("Employees");
                }
            }
        }



        private int _itemCount = 20;

        /// <summary>
        /// Gets the index of the first item in the employees list.
        /// </summary>
        private int _start = 0;
        public int Start
        {
            get { return _start + 1; }
            set { SetProperty(ref _start, value); }
        }

        /// <summary>
        /// Gets the index of the last item in the employees list.
        /// </summary>
        private int _end;
        public int End
        {
            get { return _start + _itemCount < _totalItems ? _start + _itemCount : _totalItems; ; }
            set { SetProperty(ref _end, value); }
        }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        private int _totalItems = 0;
        public int TotalItems
        {
            get { return _totalItems; }
            set { SetProperty(ref _totalItems, value); }
        }

        private int? _searchEmployeeId;
        public int? SearchEmployeeId
        {
            get { return _searchEmployeeId; }
            set { SetProperty(ref _searchEmployeeId, value); }
        }

        private int? _deleteEmployeeId;
        public int? DeleteEmployeeId
        {
            get { return _deleteEmployeeId; }
            set { SetProperty(ref _deleteEmployeeId, value); }
        }

        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref _selectedEmployee, value); }
        }

        private Employee _createEmployee;

        public Employee CreateEmployee
        {
            get { return _createEmployee; }
            set { SetProperty(ref _createEmployee, value); }
        }

        private bool _isLoadData;

        public bool IsLoadData
        {
            get { return _isLoadData; }
            set { SetProperty(ref _isLoadData, value); }
        }

        private string _responseMessage;

        public string ResponseMessage
        {
            get { return _responseMessage; }
            set { SetProperty(ref _responseMessage, value); }
        }
        #endregion

        #region [Create Employee Properties]

        private string _name;

        public string name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        private string _email;

        public string email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _gender;

        public string gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        private string _status;

        public string status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        private bool _isShowForm;

        public bool IsShowForm
        {
            get { return _isShowForm; }
            set { SetProperty(ref _isShowForm, value); }
        }

        private string _showPostMessage = "Fill the form to register an employee!";

        public string ShowPostMessage
        {
            get { return _showPostMessage; }
            set { SetProperty(ref _showPostMessage, value); }
        }
        #endregion

        #region Pagination commands

        private ICommand firstCommand;

        private ICommand previousCommand;

        private ICommand nextCommand;

        private ICommand lastCommand;

        /// <summary>
        /// Gets the command for moving to the first page of products.
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start = 0;
                            GetEmployeeDetails();
                        },
                        param =>
                        {
                            return _start - _itemCount >= 0 ? true : false;
                        }
                    );
                }
                return firstCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the previous page of products.
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start -= _itemCount;
                            GetEmployeeDetails();
                        },
                        param =>
                        {
                            return _start - _itemCount >= 0 ? true : false;
                        }
                    );
                }
                return previousCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the next page of products.
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start += _itemCount;
                            GetEmployeeDetails();
                        },
                        param =>
                        {
                            return _start + _itemCount < _totalItems ? true : false;
                        }
                    );
                }
                return nextCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the last page of products.
        /// </summary>
        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new RelayCommand
                    (
                        param =>
                        {
                            _start = (_totalItems / _itemCount - 1) * _itemCount;
                            _start += _totalItems % _itemCount == 0 ? 0 : _itemCount;
                            GetEmployeeDetails();
                        },
                        param =>
                        {
                            return _start + _itemCount < _totalItems ? true : false;
                        }
                    );
                }
                return lastCommand;
            }
        }
        #endregion

        #region ICommand 
        public DelegateCommand OnLoadCallGet { get; set; }
        public DelegateCommand GetButtonClicked { get; set; }
        public DelegateCommand ShowRegistrationForm { get; set; }
        public DelegateCommand PostButtonClick { get; set; }
        public DelegateCommand<Employee> PutButtonClicked { get; set; }
        public DelegateCommand<Employee> DeleteButtonClicked { get; set; }
        public DelegateCommand SearchButtonClicked { get; set; }
        public DelegateCommand ExportToCSVClicked { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initalize perperies & delegate commands
        /// </summary>
        public MainWindowViewModel()
        {
            GetEmployeeDetails();
            OnLoadCallGet = new DelegateCommand(GetEmployeeDetails);
            GetButtonClicked = new DelegateCommand(GetEmployeeDetails);
            PutButtonClicked = new DelegateCommand<Employee>(UpdateEmployeeDetails);
            DeleteButtonClicked = new DelegateCommand<Employee>(DeleteEmployeeDetails);
            SearchButtonClicked = new DelegateCommand(SearchEmployeeDetailsById);
            PostButtonClick = new DelegateCommand(CreateNewEmployee);
            ShowRegistrationForm = new DelegateCommand(RegisterEmployee);
            ExportToCSVClicked = new DelegateCommand(ExportToCSV);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Make visible Regiter user form
        /// </summary>
        private void RegisterEmployee()
        {
            IsShowForm = true;
        }

        /// <summary>
        /// Fetches employee details
        /// </summary>
        private void GetEmployeeDetails()
        {
            try
            {
                int pageNumber = 0;
                if (Start < 2)
                    pageNumber = 1;
                else
                    pageNumber = (Start / 20) + 1;
                var employeeDetails = WebAPI.GetEmployeesData(pageNumber);
                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;

                    APISuccessResponseObjectWhenGet responseObj = JsonConvert.DeserializeObject<APISuccessResponseObjectWhenGet>(res);
                    Employees = responseObj.data;
                    IsLoadData = true;
                    if (pageNumber == 1)
                    {
                        Start = ((pageNumber - 1) * responseObj.Meta.Pagination.Limit);
                        End = (pageNumber * responseObj.Meta.Pagination.Limit);
                        TotalItems = responseObj.Meta.Pagination.Total;
                    }
                    NotifyPropertyChanged("Start");
                    NotifyPropertyChanged("End");
                    NotifyPropertyChanged("TotalItems");
                }
                ClearContents();
            }
            catch (Exception ex)
            {
                ResponseMessage = "Employee data cannot be fetched at the moment!";
                NotifyPropertyChanged("ResponseMessage");
            }
        }

        /// <summary>
        /// Fetches employee details for the passed id
        /// </summary>
        private void SearchEmployeeDetailsById()
        {
            try
            {
                if (SearchEmployeeId.HasValue)
                {
                    List<Employee> empList = new List<Employee>();
                    var employeeDetails = WebAPI.SearchEmployeesData(SearchEmployeeId.Value);
                    if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                        Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };

                        APISuccessResponseObject responseObj = JsonConvert.DeserializeObject<APISuccessResponseObject>(res);

                        empList.Add(responseObj.data);
                        Employees = empList;
                        IsLoadData = true;
                        ResponseMessage = "Employee found in the records!!";
                    }
                    else
                    {
                        ResponseMessage = "SearchById accepts number only!!";
                    }
                }
                NotifyPropertyChanged("ResponseMessage");
                ClearContents();

            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Adds new employee
        /// </summary>
        private void CreateNewEmployee()
        {
            try
            {
                Employee newEmployee = new Employee()
                {
                    name = name,
                    email = email,
                    gender = gender,
                    status = status
                };
                var employeeDetails = WebAPI.CreateEmployeeData(newEmployee);

                string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                ShowPostMessage = DeserializeResponseObjectHelper(res, types, "Create", newEmployee);
                NotifyPropertyChanged("ShowPostMessage");
                ClearContents();

            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Updates employee's record
        /// </summary>
        /// <param name="employee"></param>
        private void UpdateEmployeeDetails(Employee employee)
        {
            try
            {
                var employeeDetails = WebAPI.UpdateEmployeeData(employee.id, employee);
                string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                ResponseMessage = DeserializeResponseObjectHelper(res, types, "Update", employee);
                ClearContents();

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes employee's record
        /// </summary>
        /// <param name="employee"></param>
        private void DeleteEmployeeDetails(Employee employee)
        {
            try
            {
                if (employee != null)
                {
                    var employeeDetails = WebAPI.DeleteEmployeeData(employee.id);
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                    Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                    ResponseMessage = DeserializeResponseObjectHelper(res, types, "Delete", employee);
                }
                else if (DeleteEmployeeId.HasValue)
                {
                    var employeeDetails = WebAPI.DeleteEmployeeData(DeleteEmployeeId.Value);
                    string res = employeeDetails.Result.Content.ReadAsStringAsync().Result;
                    Type[] types = new Type[] { typeof(APISuccessResponseObject), typeof(APIErrorResponseObject), typeof(APIErrorResponseObjectMultipleErrorMessages) };
                    ResponseMessage = DeserializeResponseObjectHelper(res, types, "Delete", employee);
                }
                else
                {
                    ResponseMessage = "Enter valid input!!";
                }
                NotifyPropertyChanged("ResponseMessage");
                ClearContents();

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Exports the entire employee data to CSV file
        /// in the same directory as the project
        /// </summary>
        private void ExportToCSV()
        {
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
                    ResponseMessage = "Downloading completed. . .";
                    NotifyPropertyChanged("ResponseMessage");
                }
                exportToCSVList = null;
                ClearContents();

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

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

        /// <summary>
        /// Notifies subscribers of changed properties.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ClearContents()
        {
            SearchEmployeeId = null;
            DeleteEmployeeId = null;
        }

        public static string DeserializeResponseObjectHelper(string res, Type[] types, string operation, Employee employee)
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
