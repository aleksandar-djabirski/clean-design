using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Infrastructure.Interfaces
{
    public interface ICosmosDbServiceFactory
    {
        CosmosDbService<T> Create<T>() where T : class;
    }
}
