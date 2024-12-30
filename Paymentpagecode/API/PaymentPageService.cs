using System;
using System.Collections.Generic;
using System.Data;

namespace ems.master.Models
{
   public class DataAccess
{
    DBConnection objdbconn = new DBConnection();
    DataTable dt_datatable;
    IDataReader objODBCDatareader;
    int mnResult;
    string msSQL, msGetGid, msGetAPICode, lsmaster_value, lsdocumentgid, lslms_code, lsbureau_code;

    // Method to get Payment Pages with security in mind
    public void DaGetPaymentPage(MdlMstApplication360 objapplication360)
    {
        try
        {
            msSQL = "SELECT paymentpage_gid, api_code, paymentpage_name, lms_code, bureau_code, " +
                    "DATE_FORMAT(a.created_date, '%d-%m-%Y %h:%i %p') as created_date, " +
                    "CONCAT(c.user_firstname, ' ', c.user_lastname, ' / ', c.user_code) as created_by, " +
                    "CASE WHEN a.status='N' THEN 'Inactive' ELSE 'Active' END as status " +
                    "FROM ocs_mst_tpaymentpage a " +
                    "LEFT JOIN hrm_mst_temployee b ON a.created_by = b.employee_gid " +
                    "LEFT JOIN adm_mst_tuser c ON c.user_gid = b.user_gid " +
                    "ORDER BY a.paymentpage_gid DESC";

            dt_datatable = objdbconn.GetDataTable(msSQL);
            var getapplication_list = new List<application_list>();

            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dr_datarow in dt_datatable.Rows)
                {
                    getapplication_list.Add(new application_list
                    {
                        paymentpage_gid = dr_datarow["paymentpage_gid"].ToString(),
                        api_code = dr_datarow["api_code"].ToString(),
                        paymentpage_name = dr_datarow["paymentpage_name"].ToString(),
                        lms_code = dr_datarow["lms_code"].ToString(),
                        bureau_code = dr_datarow["bureau_code"].ToString(),
                        created_by = dr_datarow["created_by"].ToString(),
                        created_date = dr_datarow["created_date"].ToString(),
                        status = dr_datarow["status"].ToString(),
                    });
                }
            }

            objapplication360.application_list = getapplication_list;
            dt_datatable.Dispose();
            objapplication360.status = true;
        }
        catch
        {
            objapplication360.status = false;
        }
    }

    // Method to create a Payment Page securely
    public void DaCreatePaymentPage(application360 values, string employee_gid)
    {
        try
        {
            // Using parameterized queries to prevent SQL Injection
            string paymentpage_name = values.paymentpage_name?.Replace("'", "\\'") ?? "";
            string msSQL = "SELECT paymentpage_name FROM ocs_mst_tpaymentpage WHERE paymentpage_name = @paymentpage_name";
            var objODBCDatareader = objdbconn.GetDataReader(msSQL, new { paymentpage_name });

            if (objODBCDatareader.HasRows)
            {
                values.status = false;
                values.message = "Payment Page Name Already Exists";
                return;
            }

            objODBCDatareader.Close();

            // Secure insertion with parameterized query
            string msGetGid = objcmnfunctions.GetMasterGID("MPPG");
            string msGetAPICode = objcmnfunctions.GetApiMasterGID("PAYP");

            msSQL = "INSERT INTO ocs_mst_tpaymentpage (paymentpage_gid, api_code, paymentpage_name, payment_code, amount, ifsc_code, account_number, confirm_account_number, remarks, lms_code, bureau_code, created_by, created_date) " +
                    "VALUES (@paymentpage_gid, @api_code, @paymentpage_name, @payment_code, @amount, @ifsc_code, @account_number, @confirm_account_number, @remarks, @lms_code, @bureau_code, @created_by, @created_date)";

            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL, new
            {
                paymentpage_gid = msGetGid,
                api_code = msGetAPICode,
                paymentpage_name,
                payment_code = values.payment_code,
                amount = values.amount,
                ifsc_code = values.ifsc_code,
                account_number = values.account_number,
                confirm_account_number = values.confirm_account_number,
                remarks = values.remarks,
                lms_code = values.lms_code,
                bureau_code = values.bureau_code,
                created_by = employee_gid,
                created_date = DateTime.Now
            });

            values.status = mnResult != 0;
            values.message = values.status ? "Payment Page Added Successfully" : "Error Occurred While Adding";
        }
        catch (Exception ex)
        {
            values.status = false;
            values.message = $"Error: {ex.Message}";
        }
    }

    // Method for editing the Payment Page with parameterized queries
    public void DaEditPaymentPage(string paymentpage_gid, application360 values)
    {
        try
        {
            msSQL = "SELECT paymentpage_gid, paymentpage_name, payment_code, amount, ifsc_code, account_number, confirm_account_number, remarks, lms_code, bureau_code, status AS Status FROM ocs_mst_tpaymentpage WHERE paymentpage_gid = @paymentpage_gid";
            var objODBCDatareader = objdbconn.GetDataReader(msSQL, new { paymentpage_gid });

            if (objODBCDatareader.Read())
            {
                values.paymentpage_gid = objODBCDatareader["paymentpage_gid"].ToString();
                values.paymentpage_name = objODBCDatareader["paymentpage_name"].ToString();
                values.payment_code = objODBCDatareader["payment_code"].ToString();
                values.amount = objODBCDatareader["amount"].ToString();
                values.ifsc_code = objODBCDatareader["ifsc_code"].ToString();
                values.account_number = objODBCDatareader["account_number"].ToString();
                values.confirm_account_number = objODBCDatareader["confirm_account_number"].ToString();
                values.remarks = objODBCDatareader["remarks"].ToString();
                values.lms_code = objODBCDatareader["lms_code"].ToString();
                values.bureau_code = objODBCDatareader["bureau_code"].ToString();
                values.rbo_status = objODBCDatareader["Status"].ToString();
            }

            objODBCDatareader.Close();
            values.status = true;
        }
        catch (Exception ex)
        {
            values.status = false;
            values.message = $"Error: {ex.Message}";
        }
    }

    public void DaUpdatePaymentPage(string employee_gid, application360 values)
{
    try
    {
        // SQL Query to update only the 'remarks' field
        msSQL = "UPDATE ocs_mst_tpaymentpage SET " +
                "remarks = @remarks, " +
                "modified_by = @modified_by, " +
                "modified_date = @modified_date " +
                "WHERE paymentpage_gid = @paymentpage_gid";

        // Execute the query with parameterized values
        mnResult = objdbconn.ExecuteNonQuerySQL(msSQL, new
        {
            remarks = values.remarks,  // Only update remarks
            modified_by = employee_gid,  // The user modifying the record
            modified_date = DateTime.Now,  // Current date and time
            paymentpage_gid = values.paymentpage_gid  // Identify the record to update
        });

        // Set the status and message based on the result of the query
        values.status = mnResult != 0;
        values.message = values.status ? "Remarks Updated Successfully" : "Error Occurred While Updating Remarks";
    }
    catch (Exception ex)
    {
        values.status = false;
        values.message = $"Error: {ex.Message}";
    }
}


    // Method for deleting Payment Page with security (parameterized queries)
    public void DaDeletePaymentPage(string paymentpage_gid, string employee_gid, result objResult)
    {
        try
        {
            msSQL = "DELETE FROM ocs_mst_tpaymentpage WHERE paymentpage_gid = @paymentpage_gid";
            mnResult = objdbconn.ExecuteNonQuerySQL(msSQL, new { paymentpage_gid });

            objResult.status = mnResult != 0;
            objResult.message = objResult.status ? "Payment Page Deleted Successfully" : "Error Occurred While Deleting";
        }
        catch (Exception ex)
        {
            objResult.status = false;
            objResult.message = $"Error: {ex.Message}";
        }
    }

    // Logging method with security (parameterized queries)
    public void DaPaymentPageInactiveLogview(string paymentpage_gid, MdlMstApplication360 objapplication360)
    {
        try
        {
            msSQL = "SELECT CONCAT(a.created_date, ' / ', c.user_firstname, ' ', c.user_lastname, ' / ', c.user_code) AS inactive_log " +
                    "FROM ocs_mst_tpaymentpage a " +
                    "LEFT JOIN hrm_mst_temployee b ON a.created_by = b.employee_gid " +
                    "LEFT JOIN adm_mst_tuser c ON c.user_gid = b.user_gid " +
                    "WHERE a.paymentpage_gid = @paymentpage_gid AND a.status = 'N'";

            dt_datatable = objdbconn.GetDataTable(msSQL, new { paymentpage_gid });
            var inactive_log_list = new List<string>();

            if (dt_datatable.Rows.Count != 0)
            {
                foreach (DataRow dr_datarow in dt_datatable.Rows)
                {
                    inactive_log_list.Add(dr_datarow["inactive_log"].ToString());
                }
            }

            objapplication360.inactive_logs = inactive_log_list;
            objapplication360.status = true;
        }
        catch (Exception ex)
        {
            objapplication360.status = false;
            objapplication360.message = $"Error: {ex.Message}";
        }
    }
}

}
