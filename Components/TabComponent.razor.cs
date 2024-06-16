using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace TabMenuComponent.Components
{
    public partial class TabComponent
    {
        [Parameter] public string DefaultPage { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected NavigationManager navigationManager { get; set; }

        [Inject]
        protected TabService TabService { get; set; }

        private bool isDeleting = false;

        protected override void OnInitialized()
        {
            if (!string.IsNullOrEmpty(DefaultPage))
            {
                TabService.DefaultPage = DefaultPage;
                TabService.AddTab(DefaultPage, DefaultPage);
            }
            TabService.OnChange += StateHasChanged;
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                navigationManager.NavigateTo("/");
            }
        }


        public void Dispose()
        {
            TabService.OnChange -= StateHasChanged;
        }

        private void SelectTab(string url)
        {
            if (!isDeleting)
            {
                TabService.SetActiveTab(url);
                if (url != DefaultPage)
                {
                    navigationManager.NavigateTo(url);
                }
                else
                {
                    navigationManager.NavigateTo("/");
                }
            }
            else
            {
                isDeleting = false;
                SelectTab(DefaultPage);
            }
            isDeleting = false;
        }

        private void RemoveTab(TabItem tab)
        {
            if (tab.Url != DefaultPage)
            {
                TabService.RemoveTab(tab);
                isDeleting = true;
            }
        }

    }
}