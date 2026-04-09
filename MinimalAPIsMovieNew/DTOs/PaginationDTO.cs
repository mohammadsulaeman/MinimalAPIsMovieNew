namespace MinimalAPIsMovieNew.DTOs
{
    public class PaginationDTO
    {
        public const int PageInitialValue = 1;
        public const int RecordsPerPageValue = 10;
        public int Page { get; set; } = 1;
        public int recordsPerPage { get; set; } = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                if (value > recordsPerPageMax)
                {
                    recordsPerPage = recordsPerPageMax;
                }
                else
                {
                    recordsPerPage = value;
                }
            }
        }

        public static ValueTask<PaginationDTO> BindAsync(HttpContext context)
        {
            var page = context.Request.Query[nameof(Page)];
            var recordsPerPage = context.Request.Query[nameof(RecordsPerPage)];

            var pageInt = string.IsNullOrEmpty(page) ? PageInitialValue : int.Parse(page.ToString());
            var recordsPerPageInt = string.IsNullOrEmpty(recordsPerPage) ? RecordsPerPageValue :
                int.Parse(recordsPerPage.ToString());
            var response = new PaginationDTO
            {
                Page = pageInt,
                RecordsPerPage = recordsPerPageInt
            };

            return ValueTask.FromResult(response);

        }
    }
}
