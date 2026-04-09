namespace MinimalAPIsMovieNew.DTOs
{
    public class MoviesFilterDTO
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        {
            get
            {
                return new PaginationDTO { Page = Page, RecordsPerPage = RecordsPerPage };

            }
        }
        public string? Title { get; set; }
        public int GenreId { get; set; }
        public bool IsTheaters { get; set; }
        public bool FutureReleases { get; set; }
        public string? OrderByField { get; set; }
        public bool OrderByAscending { get; set; } = true;

        public static ValueTask<MoviesFilterDTO> BindAsync(HttpContext context)
        {
            var page = context.Request.Query[nameof(Page)];
            var recordsPerPage = context.Request.Query[nameof(RecordsPerPage)];



            var title = context.Request.Query[nameof(Title)];
            var gendreId = context.Request.Query[nameof(GenreId)];
            var inThreats = context.Request.Query[nameof(IsTheaters)];
            var futureRealeases = context.Request.Query[nameof(FutureReleases)];
            var orderByField = context.Request.Query[nameof(OrderByField)];
            var orderByAscending = context.Request.Query[nameof(OrderByAscending)];

            var pageInt = string.IsNullOrEmpty(page) ? PaginationDTO.PageInitialValue
               : int.Parse(page.ToString());
            var recordsPerPageInt = string.IsNullOrEmpty(recordsPerPage) ? PaginationDTO.RecordsPerPageValue :
                int.Parse(recordsPerPage.ToString());
            var titlerespon = string.IsNullOrEmpty(title) ? string.Empty
                : title.ToString();
            var genreidrespon = string.IsNullOrEmpty(gendreId) ? 0
                : int.Parse(gendreId.ToString());
            var inthreatsrespon = string.IsNullOrEmpty(inThreats) ? false
                : Boolean.Parse(inThreats.ToString());
            var futureRealeasesrespon = string.IsNullOrEmpty(futureRealeases) ? false
                : Boolean.Parse(futureRealeases.ToString());
            var orderByFieldrespon = string.IsNullOrEmpty(orderByField) ? string.Empty
                : orderByField.ToString();
            var orderByAscendingRespon = string.IsNullOrEmpty(orderByAscending) ? false
                : Boolean.Parse(orderByAscending.ToString());

            var response = new MoviesFilterDTO
            {
                Page = pageInt,
                RecordsPerPage = recordsPerPageInt,
                Title = titlerespon,
                GenreId = genreidrespon,
                IsTheaters = inthreatsrespon,
                FutureReleases = futureRealeasesrespon,
                OrderByField = orderByFieldrespon,
                OrderByAscending = orderByAscendingRespon

            };

            return ValueTask.FromResult(response);
        }
    }
}
