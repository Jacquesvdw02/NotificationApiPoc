using Microsoft.AspNetCore.Mvc;
using POC_Notifications_new.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace POC_Notifications_new.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        ANHService _anh = new ANHService();
        [HttpGet]
        public void GetAll()
        {
            _anh.GetAllClients();
        }

        [HttpGet("{tagName}")]
        public void GetByTagName(string tagName)
        {
            _anh.GetDeviceByTagName(tagName);
        }

        [HttpPost]
        [Route("RegisterDevice")]
        public object PostDevice(string FCM, string tagName)
        {
            _anh.RegisterDevice(FCM, tagName);
            return new { status = "success" };
        }

        [HttpPost]
        [Route("SendNotification")]
        public void PostNotification()
        {
            _anh.SendPushNotification();
            //return new { status = "success" };
        }

        [HttpPut]
        public void Put(string tagName, string newFCM)
        {
            _anh.UpdateDeviceTokenByTag(tagName, newFCM);
        }

        [HttpDelete()]
        public void Delete()
        {
            _anh.ClearAllDevices();
        }

        [HttpDelete()]
        [Route("NoTags")]
        public void DeleteNoTags()
        {
            _anh.ClearAllDevicesWithNoTags();
        }
    }
}
