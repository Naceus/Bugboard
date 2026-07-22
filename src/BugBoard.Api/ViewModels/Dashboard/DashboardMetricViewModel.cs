namespace BugBoard.Api.ViewModels.Dashboard
{
    public class DashboardMetricViewModel
    {
        public string Title { get; }
        public string Value { get; }
        public string Description { get; }

        public string Icon { get; }
        public string Variant { get; }
        public string? FilterUrl { get; }


        public DashboardMetricViewModel(string title, string value, string description, string icon, string variant, string? filterUrl)
        {
            Title = title;
            Value = value;
            Description = description;
            Icon = icon;
            Variant = variant;
            FilterUrl = filterUrl;
        }
    }
}
