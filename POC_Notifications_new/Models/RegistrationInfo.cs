namespace POC_Notifications_new.Models
{
    public class RegistrationInfo
    {
        public string RegistrationId { get; set; }
        public string FcmToken { get; set; }
        public List<string> UserTags { get; set; }
    }
}
