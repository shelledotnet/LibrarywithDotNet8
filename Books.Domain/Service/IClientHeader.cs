using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service
{
    public interface IClientHeader
    {
        void clientId(string id);
        string getclientId();

        void correlationId(string id);
        string getcorrelationId();

    }
}
