using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace TabMenuComponent.Components
{
    /// <summary>
    /// Service that manages tabs in the application.
    /// </summary>
    public class TabService
    {
        private string defaultPageUrl;

        /// <summary>
        /// Gets or sets the default page URL.
        /// </summary>
        public string DefaultPage
        {
            get => defaultPageUrl;
            set
            {
                defaultPageUrl = value;
                if (string.IsNullOrEmpty(activeTab?.Url) && !string.IsNullOrEmpty(defaultPageUrl))
                {
                    SetActiveTab(defaultPageUrl);
                }
            }
        }

        /// <summary>
        /// Event that notifies when a change occurs in the tabs.
        /// </summary>
        public event Action OnChange;

        private List<TabItem> tabs = new List<TabItem>();
        public List<TabItem> Tabs => tabs;

        private TabItem activeTab;
        public TabItem ActiveTab
        {
            get => activeTab;
            private set
            {
                activeTab = value;
                NotifyStateChanged();
            }
        }

        private NavigationManager navigationManager;

        /// <summary>
        /// Constructor for TabService.
        /// </summary>
        public TabService(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            navigationManager.LocationChanged += OnLocationChanged;
        }

        /// <summary>
        /// When the URL changed.
        /// </summary>
        private void OnLocationChanged(object sender, LocationChangedEventArgs args)
        {
            string[] uriParts = args.Location.Split('/');
            string pageName = uriParts[uriParts.Length - 1];
            if (pageName == "")
                pageName = DefaultPage;
            pageName = pageName.Replace("-", " ");
            string url = args.Location;
            if (pageName != DefaultPage)
            {
                AddTab(pageName.ToUpper(), url);
                SetActiveTab(url);
            }
            else
            {
                SetActiveTab(DefaultPage);
            }
        }

        /// <summary>
        /// Adds a new tab to the list of tabs.
        /// </summary>
        public void AddTab(string title, string url)
        {
            if (!tabs.Exists(t => t.Url == url))
            {
                tabs.Add(new TabItem { Title = title, Url = url });
                NotifyStateChanged();
            }

            if (string.IsNullOrEmpty(activeTab?.Url) && !string.IsNullOrEmpty(DefaultPage))
            {
                SetActiveTab(DefaultPage);
            }
        }

        /// <summary>
        /// Sets the active tab based on the provided URL.
        /// </summary>
        public void SetActiveTab(string url)
        {
            ActiveTab = tabs.Find(t => t.Url == url);
            NotifyStateChanged();
        }

        /// <summary>
        /// Removes a tab from the list of tabs.
        /// </summary>
        public void RemoveTab(TabItem tab)
        {
            tabs.Remove(tab);
            if (activeTab == tab && tabs.Count > 0)
            {
                ActiveTab = tabs[0];
            }
            else if (tabs.Count == 0)
            {
                ActiveTab = null;
            }
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    /// <summary>
    /// Represents a single tab item.
    /// </summary>
    public class TabItem
    {
        public string Title { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// Checks if the tab item is active.
        /// </summary>
        public bool IsActive(TabItem activeTab) => activeTab?.Url == Url;
    }
}