using System.Web.Http;
using ems.master.Models;

namespace ems.master.Controllers
{
  
    public class PaymentPageController : ApiController
    {
        DataAccess objDataAccess = new DataAccess();

        [HttpGet]
        [Route("api/PaymentPage/GetAll")]
        public IHttpActionResult GetAll()
        {
            MdlMstApplication360 objapplication360 = new MdlMstApplication360();
            objDataAccess.DaGetPaymentPage(objapplication360);
            return Ok(objapplication360);
        }

        [HttpPost]
        [Route("api/PaymentPage/Create")]
        public IHttpActionResult Create(application360 values, string employee_gid)
        {
            objDataAccess.DaCreatePaymentPage(values, employee_gid);
            return Ok(values);
        }

        [HttpGet]
        [Route("api/PaymentPage/Edit")]
        public IHttpActionResult Edit(string paymentpage_gid)
        {
            application360 values = new application360();
            objDataAccess.DaEditPaymentPage(paymentpage_gid, values);
            return Ok(values);
        }

        [HttpPost]
        [Route("api/PaymentPage/Update")]
        public IHttpActionResult Update(application360 values, string employee_gid)
        {
            objDataAccess.DaUpdatePaymentPage(employee_gid, values);
            return Ok(values);
        }

        [HttpPost]
        [Route("api/PaymentPage/SetInactive")]
        public IHttpActionResult SetInactive(application360 values, string employee_gid)
        {
            objDataAccess.DaInactivePaymentPage(values, employee_gid);
            return Ok(values);
        }

        [HttpDelete]
        [Route("api/PaymentPage/Delete")]
        public IHttpActionResult Delete(string paymentpage_gid, string employee_gid)
        {
            result objResult = new result();
            objDataAccess.DaDeletePaymentPage(paymentpage_gid, employee_gid, objResult);
            return Ok(objResult);
        }

        [HttpGet]
        [Route("api/PaymentPage/GetInactiveLog")]
        public IHttpActionResult GetInactiveLog(string paymentpage_gid)
        {
            MdlMstApplication360 values = new MdlMstApplication360();
            objDataAccess.DaPaymentPageInactiveLogview(paymentpage_gid, values);
            return Ok(values);
        }
    }
}
