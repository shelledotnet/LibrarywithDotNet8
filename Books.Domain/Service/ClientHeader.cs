using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service
{
    public class ClientHeader : IClientHeader
    {
        public string clientid { get; set; }
        public string correlationid { get; set; }

        public void clientId(string id)
        {
            clientid = id;
        }

        public string getclientId()
        {
            return clientid;
        }

        public void correlationId(string id)
        {
            correlationid = id;
        }

        public string getcorrelationId()
        {
            return correlationid;
        }
    }
}
