using Microsoft.Azure.NotificationHubs;

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

        async public void RegisterDevice(string FCM, string tagName)
        {
            IEnumerable<string> userTags = new List<string>() { tagName };
            RegistrationDescription registration = await hub.CreateFcmNativeRegistrationAsync(FCM, userTags);
            Console.WriteLine(registration.Tags.First().ToString());
        }

        async public void SendPushNotification(string tagName)
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
        }

        async public void GetAllClients()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);
            foreach (var registration in registrations)
            {
                Console.WriteLine($"RegistrationId: {registration.RegistrationId}");
                Console.WriteLine($"FCM Token: {registration.PnsHandle}");

                // Display user tags
                if (registration.Tags != null)
                {
                    Console.WriteLine("User Tags:");
                    foreach (var tag in registration.Tags)
                    {
                        Console.WriteLine($"  {tag}");
                    }
                }
            }
        }

        async public void GetDeviceByTagName(string tagName)
        {
            var registrations = await hub.GetRegistrationsByTagAsync(tagName, int.MaxValue);

            if (registrations != null && registrations.Any())
            {
                foreach (var registration in registrations)
                {
                    Console.WriteLine($"RegistrationId: {registration.RegistrationId}");
                    Console.WriteLine($"FCM Token: {registration.PnsHandle}");

                    // Display user tags
                    if (registration.Tags != null)
                    {
                        Console.WriteLine("User Tags:");
                        foreach (var tag in registration.Tags)
                        {
                            Console.WriteLine($"  {tag}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"No devices found with tag '{tagName}'.");
            }
        }

        async public void ClearAllDevicesWithNoTags()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);

            foreach (var registration in registrations)
            {
                if (registration.Tags == null || !registration.Tags.Any())
                {
                    try
                    {
                        await hub.DeleteRegistrationAsync(registration.RegistrationId);
                        Console.WriteLine($"Deleted registration with ID: {registration.RegistrationId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting registration with ID {registration.RegistrationId}: {ex.Message}");
                    }
                }
            }
        }

        async public void ClearAllDevices()
        {
            var registrations = await hub.GetAllRegistrationsAsync(int.MaxValue);

            foreach (var registration in registrations)
            {
                try
                {
                    await hub.DeleteRegistrationAsync(registration.RegistrationId);
                    Console.WriteLine($"Deleted registration with ID: {registration.RegistrationId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting registration with ID {registration.RegistrationId}: {ex.Message}");
                }
            }
        }

        async public void UpdateDeviceTokenByTag(string tagName, string newFCMToken)
        {
            var registrations = await hub.GetRegistrationsByTagAsync(tagName, int.MaxValue);

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

                        Console.WriteLine($"Updated FCM token for device with tag '{tagName}'.");
                        Console.WriteLine($"New FCM Token: {newFCMToken}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating FCM token for device with tag '{tagName}': {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"No devices found with tag '{tagName}'.");
            }
        }



    }
}
