using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// (It's used for All master in Samfin) Application360 Model Class accessed by API methods from related DataAccess class and is returning relevant response to client.
/// </summary>
/// <remarks>Written by Sumala, Logapriya and Abilash</remarks>
namespace ems.master.Models
{
    public class MdlMstApplication360 : result
    {
        public List<application_list> application_list { get; set; }
    }

    public class application_list
    {
          public string paymentpage_gid { get; set; }
    public string paymentpage_name { get; set; }
    public string payment_code { get; set; }
    public string amount { get; set; }
    public string ifsc_code { get; set; }
    public string account_number { get; set; }  // Changed to account_number
    public string confirm_account_number { get; set; }  // Changed to confirm_account_number
    public string remarks { get; set; }
    public string lms_code { get; set; }
    public string bureau_code { get; set; }
    public string status { get; set; }
    public string message { get; set; }
    public string rbo_status { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
    }

   public class application360
{
    public string paymentpage_gid { get; set; }
    public string paymentpage_name { get; set; }
    public string payment_code { get; set; }
    public string amount { get; set; }
    public string ifsc_code { get; set; }
    public string account_number { get; set; }  // Changed to account_number
    public string confirm_account_number { get; set; }  // Changed to confirm_account_number
    public string remarks { get; set; }
    public string lms_code { get; set; }
    public string bureau_code { get; set; }
    public string status { get; set; }
    public string message { get; set; }
    public string rbo_status { get; set; }
}


    public class result
    {
        public string message { get; set; }
        public bool status { get; set; }
    }
}
