using abremir.AllMyBricks.Data.Enumerations;
using abremir.AllMyBricks.Data.Extensions;
using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace abremir.AllMyBricks.Data.Repositories
{
    public class BricksetUserRepository : IBricksetUserRepository
    {
        private readonly IRepositoryService _repositoryService;

        public BricksetUserRepository(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public BricksetUser Add(BricksetUserType userType, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var existingBricksetUser = Get(username);

            if (existingBricksetUser != null)
            {
                return existingBricksetUser;
            }

            var bricksetUser = new BricksetUser
            {
                BricksetUsername = username.Trim(),
                UserType = userType,
                Sets = new List<BricksetUserSet>()
            };

            using var repository = _repositoryService.GetRepository();

            repository.Insert(bricksetUser);

            return bricksetUser;
        }

        public BricksetUser Get(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            using var repository = _repositoryService.GetRepository();

            return GetQueryable(repository)
                .Where(bricksetUser => bricksetUser.BricksetUsername == username.Trim())
                .FirstOrDefault();
        }

        public bool Exists(string username)
        {
            return Get(username) != null;
        }

        public bool Remove(string username)
        {
            var bricksetUser = Get(username);

            if (bricksetUser is null)
            {
                return false;
            }

            using var repository = _repositoryService.GetRepository();

            return repository.Delete<BricksetUser>(bricksetUser.Id);
        }

        public BricksetUserSet AddOrUpdateSet(string username, BricksetUserSet bricksetUserSet)
        {
            if (string.IsNullOrWhiteSpace(username)
                || bricksetUserSet is null
                || bricksetUserSet.Set is null
                || bricksetUserSet.Set.SetId == 0)
            {
                return null;
            }

            using var repository = _repositoryService.GetRepository();

            if (repository.FirstOrDefault<Set>(set => set.SetId == bricksetUserSet.Set.SetId) is null)
            {
                return null;
            }

            var bricksetUser = Get(username);

            if (bricksetUser is null)
            {
                return null;
            }

            var existingBricksetUserSet = bricksetUser.Sets.FirstOrDefault(set => set.Set.SetId == bricksetUserSet.Set.SetId);

            if (existingBricksetUserSet != null)
            {
                if (existingBricksetUserSet.Owned == bricksetUserSet.Owned
                    && existingBricksetUserSet.Wanted == bricksetUserSet.Wanted
                    && existingBricksetUserSet.QuantityOwned == bricksetUserSet.QuantityOwned)
                {
                    return existingBricksetUserSet;
                }

                bricksetUser.Sets.Remove(existingBricksetUserSet);
            }

            bricksetUserSet.LastChangeTimestamp ??= DateTimeOffset.Now;
            bricksetUser.Sets.Add(bricksetUserSet);

            repository.Update(bricksetUser);

            return bricksetUserSet;
        }

        public BricksetUserSet GetSet(string username, long setId)
        {
            if (string.IsNullOrWhiteSpace(username)
                || setId == 0)
            {
                return null;
            }

            return Get(username)?.Sets.FirstOrDefault(set => set.Set.SetId == setId);
        }

        public IEnumerable<string> GetAllUsernames(BricksetUserType userType)
        {
            using var repository = _repositoryService.GetRepository();

            return repository
                .Fetch<BricksetUser>(bricksetUser => bricksetUser.UserType == userType)
                .Select(bricksetUser => bricksetUser.BricksetUsername);
        }

        public BricksetUser UpdateUserSynchronizationTimestamp(string username, DateTimeOffset userSynchronizationTimestamp)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var bricksetUser = Get(username);

            if (bricksetUser is null)
            {
                return null;
            }

            bricksetUser.UserSynchronizationTimestamp = userSynchronizationTimestamp;

            using var repository = _repositoryService.GetRepository();

            repository.Update(bricksetUser);

            return bricksetUser;
        }

        public IEnumerable<BricksetUserSet> GetWantedSets(string username)
        {
            var bricksetUser = Get(username);

            if (bricksetUser is null)
            {
                return Enumerable.Empty<BricksetUserSet>();
            }

            return bricksetUser.Sets.Where(set => set.Wanted);
        }

        public IEnumerable<BricksetUserSet> GetOwnedSets(string username)
        {
            var bricksetUser = Get(username);

            if (bricksetUser is null)
            {
                return Enumerable.Empty<BricksetUserSet>();
            }

            return bricksetUser.Sets.Where(set => set.Owned);
        }

        private ILiteQueryable<BricksetUser> GetQueryable(ILiteRepository repository)
        {
            return repository
                .Query<BricksetUser>()
                .IncludeAll();
        }
    }
}
