using EmpClient.DTOClasses;
using EmpClient.Service;
using EmpClient.Utilities;
using EmployeeApp.BusinessLogic;
using EmployeeApp.Utilities;
using Newtonsoft.Json;
using NLog;
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
    class EmployeeWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        EmployeeBusinessLogic businessLogic = new EmployeeBusinessLogic();

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
            set
            {
                SetProperty(ref _start, value);
            }
        }


        /// <summary>
        /// Gets the index of the last item in the employees list.
        /// </summary>
        private int _end;
        public int End
        {
            get { return _start + _itemCount < _totalItems ? _start + _itemCount : _totalItems; ; }
            set
            {
                SetProperty(ref _end, value);
            }
        }


        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        private int _totalItems = 0;
        public int TotalItems
        {
            get { return _totalItems; }
            set
            {
                SetProperty(ref _totalItems, value);
            }
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
            set
            {
                if (_responseMessage != value)
                {
                    _responseMessage = value;
                    NotifyPropertyChanged("ResponseMessage");
                }
            }
        }
        #endregion

        #region [Create Employee Properties]

        private string _name;

        public string name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }


        private string _email;

        public string email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
            }
        }

        private string _gender;

        public string gender
        {
            get { return _gender; }
            set
            {
                SetProperty(ref _gender, value);
            }
        }

        private string _status;

        public string status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
            }
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
            set
            {
                if (_showPostMessage != value)
                {
                    _showPostMessage = value;
                    NotifyPropertyChanged("ShowPostMessage");
                }
            }
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
        public EmployeeWindowViewModel()
        {
            LoggerHelper.logger.Info("Employee View Model");
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
                //businessLogic.GetEmployeeDetails(pageNumber);
                Employees = businessLogic.GetEmployeeDetails(pageNumber).Item1;
                IsLoadData = true;
                if (pageNumber == 1)
                {
                    Start = businessLogic.GetEmployeeDetails(pageNumber).Item2;
                    End = businessLogic.GetEmployeeDetails(pageNumber).Item3;
                    TotalItems = businessLogic.GetEmployeeDetails(pageNumber).Item4;
                }
                NotifyPropertyChanged("Start");
                NotifyPropertyChanged("End");
                NotifyPropertyChanged("TotalItems");
            }
            catch (Exception ex)
            {
                ResponseMessage = "Employee data cannot be fetched at the moment!";
                LoggerHelper.logger.Error("Error in GetEmployeeDetails" + ex.Message);
            }
            finally
            {
                ClearContents();
            }
        }

        /// <summary>
        /// Fetches employee details for the passed id
        /// </summary>
        private void SearchEmployeeDetailsById()
        {
            List<Employee> empList = new List<Employee>();
            try
            {
                if (SearchEmployeeId.HasValue)
                {
                    Employees = businessLogic.SearchEmployeeDetailsById(SearchEmployeeId.Value);
                    IsLoadData = true;
                    ResponseMessage = "Employee found in the records!!";
                }
                else
                {
                    ResponseMessage = "SearchById accepts number only!!";
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.logger.Error("Error in SearchEmployeeDetailsById" + ex.Message);
            }
            finally
            {
                ShowPostMessage = null;
                ClearContents();
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
                ShowPostMessage = businessLogic.CreateNewEmployee(newEmployee);
            }
            catch (Exception ex)
            {
                LoggerHelper.logger.Error("Error in CreateNewEmployee" + ex.Message);
            }
            finally
            {
                ResponseMessage = null;
                ClearContents();
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
                ResponseMessage = businessLogic.UpdateEmployeeDetails(employee);
            }
            catch (Exception ex)
            {
                LoggerHelper.logger.Error("Error in UpdateEmployeeDetails" + ex.Message);
            }
            finally
            {
                ClearContents();
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
                    ResponseMessage = businessLogic.DeleteEmployeeDetails(employee.id);
                }
                else if (DeleteEmployeeId.HasValue)
                {
                    ResponseMessage = businessLogic.DeleteEmployeeDetails(DeleteEmployeeId.Value);
                }
                else
                {
                    ResponseMessage = "Enter valid input!!";
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.logger.Error("Error in DeleteEmployeeDetails" + ex.Message);
            }
            finally
            {
                ShowPostMessage = null;
                ClearContents();
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
                businessLogic.ExportToCSV();
                ResponseMessage = "Downloading completed. . .";
            }
            catch (Exception ex)
            {
                ResponseMessage = "Employee details couldn't be exported due to the error" + ex.Message;
                LoggerHelper.logger.Error("Error in ExportToCSV" + ex.Message);
            }
            finally
            {
                ClearContents();
            }
        }
        #endregion

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
            name = null;
            email = null;
            gender = null;
            status = null;
            NotifyPropertyChanged("SearchEmployeeId");
            NotifyPropertyChanged("DeleteEmployeeId");
            NotifyPropertyChanged("name");
            NotifyPropertyChanged("email");
            NotifyPropertyChanged("gender");
            NotifyPropertyChanged("status");
        }

    }
}
