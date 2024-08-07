using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.domain.Models;

    public class ServiceResponse<T>
    {
        public T? Data { get; set; }

        public int Code { get; set; }

        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        public bool IsSuccess { get; set; } = true;

        public string? Message { get; set; } 
    }
    public class ServiceResponse
    {


        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = "failed";
    }
    public class ServiceBadResponse
    {

    public int Code { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

    public List<string>? Message { get; set; } 
    }
    public class ServiceMethodNotAailabeResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");


        public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; }
    }
    public class ServiceFailedResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");

        public int Code { get; set; }

        public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; } 
    }

    public class ServiceForbidenResponse
    {
        public string ResponseId { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
    public int Code { get; set; }

    public bool IsSuccess { get; set; } = false;

        public string? Message { get; set; } = "unauthorized";
    }

public class Testla
{
    public dynamic? Result { get; set; }


}
