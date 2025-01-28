using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Books.domain.Models;

    public class ServiceResponse<T> : BaseRespose
    {
        public T? Data { get; set; }

        public HttpStatusCode Code { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string? Message { get; set; } 
    }
    public class ServiceResponse : BaseRespose
{


        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = "failed";
    }
    public class ServiceBadResponse : BaseRespose
{

    public int Code { get; set; } = 400;
    public bool IsSuccess { get; set; } = false;

    public string? Message { get; set; } 
    }
    public class ServiceMethodNotAailabeResponse : BaseRespose
{


        public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; }
    }
    public class ServiceFailedResponse : BaseRespose
{

        public int Code { get; set; }

        public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; } 
    }

    public class ServiceForbidenResponse : BaseRespose
    {
    public int Code { get; set; }

    public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; } = "unauthorized";
    }

public class Testla
{
    public dynamic? Result { get; set; }


}

public class BaseRespose
{
    public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

}
