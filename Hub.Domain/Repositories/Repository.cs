using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoInfrastructure.Concrete;
using System.Data.Objects.DataClasses;
using Hub.Domain.Entities;
namespace Hub.Domain.Repositories
{
    public class Repository<T> : ContextRepository<T, HubEntities> where T : EntityObject {}
    public class ReadRepository<T> : ContextReadonlyRepository<T, HubEntities> where T : EntityObject {}
}