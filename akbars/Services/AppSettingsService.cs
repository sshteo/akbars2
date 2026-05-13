using akbars.Properties;

namespace akbars.Services
{
    public class AppSettingsService
    {
        public string RememberedLogin
        {
            get { return Settings.Default.SavedLogin; }
        }

        public bool RememberLogin
        {
            get { return Settings.Default.RememberMe; }
        }

        public void SaveRememberedLogin(string login, bool remember)
        {
            Settings.Default.SavedLogin = remember ? login : string.Empty;
            Settings.Default.RememberMe = remember;
            Settings.Default.Save();
        }
    }
