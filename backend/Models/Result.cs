using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Backend.Models
{
    public enum Status
    {
        Success,
        Failure,
    }

    public class Result<T>
    {
        [EnumDataType(typeof(Status))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public Result(Status status, String message = null, T data = default) 
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public Result()
        {
            Status = Status.Failure;
        }

        public Result<U> ConvertToRequestResult<U>(Func<U>? dataConversion) 
        {
            if (Status == Status.Success) 
            {
                if (dataConversion != null) 
                {
                    return new Result<U>(Status, Message, dataConversion()); 
                }
                else
                {
                    if (typeof(T) == typeof(U))
                    {
                        throw new Exception("Conversion is pointless in this case! Fix your code ;)");
                    }
                }
            }

            // Convert to result with empty data
            return new Result<U>(Status, Message);
        }
    }
}

