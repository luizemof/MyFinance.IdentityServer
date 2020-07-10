using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Repository;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Store
{
    public class CustomPersistedGrantStore : IPersistedGrantStore
    {
        protected IRepository _dbRepository;

        public CustomPersistedGrantStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var result = _dbRepository.Where<PersistedGrant>(persistedGrant => PersistedGrantfilter(filter, persistedGrant));
            return Task.FromResult(result.AsEnumerable());
        }


        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = _dbRepository.Single<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(result);
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            _dbRepository.Delete<PersistedGrant>(i => PersistedGrantfilter(filter, i));
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _dbRepository.Add<PersistedGrant>(grant);
            return Task.FromResult(0);
        }

        private bool PersistedGrantfilter(PersistedGrantFilter filter, PersistedGrant persistedGrant)
        {
            return
            (string.IsNullOrWhiteSpace(filter.ClientId) || filter.ClientId == persistedGrant.ClientId)
            &&
            (string.IsNullOrWhiteSpace(filter.SessionId) || filter.SessionId == persistedGrant.SessionId)
            &&
            (string.IsNullOrWhiteSpace(filter.SubjectId) || filter.SubjectId == persistedGrant.SubjectId)
            &&
            (string.IsNullOrWhiteSpace(filter.Type) || filter.Type == persistedGrant.Type);
        }
    }
}