using Enumium.Helpers;
using Enumium.Services;

namespace Enumium.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string Title => LocalizationService.Get("about_title");
        public string Version => LocalizationService.Get("about_version");
        public string Description => LocalizationService.Get("about_desc");
        public string DeveloperLabel => LocalizationService.Get("about_developer");
        public string GitHubLabel => LocalizationService.Get("about_github");
        public string BuiltWith => LocalizationService.Get("about_built_with");
        public string Rights => "© 2025 enum000. " + LocalizationService.Get("about_rights");
        public string FeaturesLabel => LocalizationService.Get("about_features");
        public string FeaturesList => LocalizationService.Get("about_features_list");

        public string DeveloperName => "enum000";
        public string GitHubUrl => "https://github.com/enum000";

        public RelayCommand OpenGitHubCommand { get; }

        public AboutViewModel()
        {
            OpenGitHubCommand = new RelayCommand(_ =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = GitHubUrl,
                        UseShellExecute = true
                    });
                }
                catch { }
            });
        }
    }
}
