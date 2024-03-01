using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using POC_Notifications_new.Models;
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
        async public Task<List<RegistrationInfo>> GetAll()
        {
            var result = await _anh.GetAllClients();
            return result;
        }

        [HttpGet("{tagName}")]
        async public Task<List<RegistrationInfo>> GetByTagName(string tagName)
        {
            var result = await _anh.GetDeviceByTagName(tagName);
            return result;
        }

        [HttpPost]
        [Route("RegisterDevice")]
        async public Task<RegistrationDescription> PostDevice([FromBody] DeviceModel model)
        {
            var result = await _anh.RegisterDevice(model.FCM, model.TagName);
            return result;
        }

        [HttpPost]
        [Route("SendNotification/{tagName}")]
        async public Task<NotificationOutcome> PostNotification(string tagName)
        {
            var result = await _anh.SendPushNotification(tagName);
            return result;
        }

        [HttpPut]
        async public Task<List<RegistrationInfo>> Put([FromBody] DeviceModel model)
        {
            var result = await _anh.UpdateDeviceTokenByTag(model.TagName, model.FCM);
            return result;
        }

        [HttpDelete()]
        async public Task<object> Delete()
        {
            var result = await _anh.ClearAllDevices();
            return result;
        }

        [HttpDelete()]
        [Route("NoTags")]
        async public Task<object> DeleteNoTags()
        {
            var result = await _anh.ClearAllDevicesWithNoTags();
            return result;
        }
    }
}
