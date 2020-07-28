using System;

namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad requst, you have made",
                401 => "Authorized,you are not",
                404 => "Resource found,it was not",
                500 => "Errors are path to dark side.Error leads to anger.Anger leads to" + 
                    "hate. Hate leads to career change.",
                _ => null

            };
        }

    }
}