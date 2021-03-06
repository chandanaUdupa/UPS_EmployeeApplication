using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace EmpClient.Utilities
{
    public static class CSVUtlity
    {
        /// <summary>
        /// Converts dtDataTable to csv file in the specified file location
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <param name="strFilePath"></param>
        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(strFilePath, false))
                {
                    //headers    
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        sw.Write(dtDataTable.Columns[i]);
                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                    foreach (DataRow dr in dtDataTable.Rows)
                    {
                        for (int i = 0; i < dtDataTable.Columns.Count; i++)
                        {
                            if (!Convert.IsDBNull(dr[i]))
                            {
                                string value = dr[i].ToString();
                                if (value.Contains(','))
                                {
                                    value = String.Format("\"{0}\"", value);
                                    sw.Write(value);
                                }
                                else
                                {
                                    sw.Write(dr[i].ToString());
                                }
                            }
                            if (i < dtDataTable.Columns.Count - 1)
                            {
                                sw.Write(",");
                            }
                        }
                        sw.Write(sw.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
