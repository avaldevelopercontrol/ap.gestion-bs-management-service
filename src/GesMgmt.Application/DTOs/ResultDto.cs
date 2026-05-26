namespace GesMgmt.Application.DTOs
{
    // Result Pattern
    public class ResultDto<T>
    {
        public string Code { get; }
        public string Message { get; }
        public string MessageUser { get; }
        public int StatusCode { get; }
        public T Response { get; set; }

        private ResultDto(T response, string code, string message, string messageUser, int statusCode )
        {
            Response = response;
            Code = code;
            Message = message;
            MessageUser = messageUser;
            StatusCode = statusCode;
        }

        public static ResultDto<T> Success(T response, string code, string message, string messageUser, int statusCode )
        {
            return new ResultDto<T>(response, code, message, messageUser, statusCode);
        }

        public static ResultDto<T> Failure(string code, string message, string messageUser, int statusCode)
        {
            return new ResultDto<T>(default, code, message, messageUser, statusCode);
        }
    }

    public class ResultListDto<T>
    {
        public string Code { get; }
        public string Message { get; }
        public string MessageUser { get; }
        public int StatusCode { get; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public T Response { get; set; }

        private ResultListDto(T response, string code, string message, string messageUser, int statusCode)
        {
            Response = response;
            Code = code;
            Message = message;
            MessageUser = messageUser;
            StatusCode = statusCode;
        }

        public ResultListDto()
        {
        }

        public static ResultListDto<T> Success(T response, string code, string message, string messageUser, int statusCode)
        {
            return new ResultListDto<T>(response, code, message, messageUser, statusCode);
        }

        public static ResultListDto<T> Failure(string code, string message, string messageUser, int statusCode)
        {
            return new ResultListDto<T>(default, code, message, messageUser, statusCode);
        }
    }
}
