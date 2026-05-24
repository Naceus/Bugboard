namespace BugBoard.Api.ViewModels.BugReports
{
    public class PaginationViewModel
    {
        public int CurrentPage{ get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPage => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPage;

    }
}