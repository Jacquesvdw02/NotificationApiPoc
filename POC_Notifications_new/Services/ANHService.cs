using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using POC_Notifications_new.Models;

namespace POC_Notifications_new.Services
{
    public class ANHService
    {
        NotificationHubClient hub;
        public ANHService()
        {
            hub = NotificationHubClient.CreateClientFromConnectionString(
            "Endpoint=sb://ntfns-pinniesV2-dev.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=DGF1esNYdetVnbhIo73mvwM18JnstXA01jGNTQtdBr8=",
            "ntf-pinniesV2-dev");
        }

        async public Task<RegistrationDescription> RegisterDevice(string FCM, string tagName)
        {
            IEnumerable<string> userTags = new List<string>() { tagName };
            RegistrationDescription registration = await hub.CreateFcmNativeRegistrationAsync(FCM, userTags);
            return registration;
        }

        async public Task<NotificationOutcome> SendPushNotification(string tagName)
        {
            IEnumerable<string> userTags = new List<string>() { tagName };
            string notificationPayload = @"
            {
	            ""notification"": {
		            ""title"": ""SWAGGER NOTIFICATION"",
		            ""body"": ""This is a sample notification delivered by Azure Notification Hubs.""
	            },
	            ""data"": {
		            ""property1"": ""value1"",
		            ""property2"": 42
	            }
            }";
            var result = await hub.SendFcmNativeNotificationAsync(notificationPayload, userTags);
            return result;
        }

        async public Task<List<RegistrationInfo>> GetAllClients()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);
            List<RegistrationInfo> registrationInfoList = new List<RegistrationInfo>();

            foreach (var registration in registrations)
            {
                var registrationInfo = new RegistrationInfo
                {
                    RegistrationId = registration.RegistrationId,
                    FcmToken = registration.PnsHandle,
                    UserTags = registration.Tags?.ToList()
                };

                registrationInfoList.Add(registrationInfo);
            }

            return registrationInfoList;
        }

        async public Task<List<RegistrationInfo>> GetDeviceByTagName(string tagName)
        {
            var registrations = await hub.GetRegistrationsByTagAsync(tagName, int.MaxValue);

            List<RegistrationInfo> registrationInfoList = new List<RegistrationInfo>();

            if (registrations != null && registrations.Any())
            {
                foreach (var registration in registrations)
                {
                    var registrationInfo = new RegistrationInfo
                    {
                        RegistrationId = registration.RegistrationId,
                        FcmToken = registration.PnsHandle,
                        UserTags = registration.Tags?.ToList()
                    };

                    registrationInfoList.Add(registrationInfo);
                }
                return registrationInfoList;
            }
            else
            {
                return null;
            }
        }

        async public Task<object> ClearAllDevicesWithNoTags()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);

            foreach (var registration in registrations)
            {
                if (registration.Tags == null || !registration.Tags.Any())
                {
                    try
                    {
                        await hub.DeleteRegistrationAsync(registration.RegistrationId);
                    }
                    catch (Exception ex)
                    {
                        return new { status = "error", exception = ex.Message };
                    }
                }
            }

            return new { status = "success" };
        }

        async public Task<object> ClearAllDevices()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);

            foreach (var registration in registrations)
            {
                try
                {
                    await hub.DeleteRegistrationAsync(registration.RegistrationId);
                }
                catch (Exception ex)
                {
                    return new { status = "error", exception = ex.Message };
                }
            }

            return new { status = "success" };
        }

        async public Task<List<RegistrationInfo>> UpdateDeviceTokenByTag(string tagName, string newFCMToken)
        {
            var registrations = await hub.GetRegistrationsByTagAsync(tagName, int.MaxValue);

            List<RegistrationInfo> updatedRegistrations = new List<RegistrationInfo>();

            if (registrations != null && registrations.Any())
            {
                foreach (var registration in registrations)
                {
                    try
                    {
                        IEnumerable<string> userTags = new List<string>() { tagName };
                        RegistrationDescription newRegistration = await hub.CreateFcmNativeRegistrationAsync(newFCMToken, userTags);

                        // Delete the old registration
                        await hub.DeleteRegistrationAsync(registration.RegistrationId);

                        var updatedRegistrationInfo = new RegistrationInfo
                        {
                            RegistrationId = newRegistration.RegistrationId,
                            FcmToken = newRegistration.PnsHandle,
                            UserTags = newRegistration.Tags?.ToList()
                        };

                        updatedRegistrations.Add(updatedRegistrationInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating FCM token for device with tag '{tagName}': {ex.Message}");
                    }
                }
            }

            return updatedRegistrations;
        }



    }
}
