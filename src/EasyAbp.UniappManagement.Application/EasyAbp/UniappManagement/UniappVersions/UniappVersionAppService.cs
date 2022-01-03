using System;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.UniappManagement.Authorization;
using EasyAbp.UniappManagement.Uniapps;
using EasyAbp.UniappManagement.UniappVersions.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.UniappManagement.UniappVersions
{
    public class UniappVersionAppService : CrudAppService<UniappVersion, UniappVersionDto, Guid, UniappVersionGetListDto, CreateUpdateUniappVersionDto, CreateUpdateUniappVersionDto>,
        IUniappVersionAppService
    {
        protected override string CreatePolicyName { get; set; } = UniappManagementPermissions.UniappVersions.Create;
        protected override string DeletePolicyName { get; set; } = UniappManagementPermissions.UniappVersions.Delete;
        protected override string UpdatePolicyName { get; set; } = UniappManagementPermissions.UniappVersions.Update;
        protected override string GetPolicyName { get; set; } = UniappManagementPermissions.UniappVersions.Default;
        protected override string GetListPolicyName { get; set; } = UniappManagementPermissions.UniappVersions.Default;

        private readonly IUniappRepository _uniappRepository;
        private readonly IUniappVersionRepository _uniappVersionRepository;

        public UniappVersionAppService(
            IUniappRepository uniappRepository,
            IUniappVersionRepository uniappVersionRepository) : base(uniappVersionRepository)
        {
            _uniappRepository = uniappRepository;
            _uniappVersionRepository = uniappVersionRepository;
        }

        protected override async Task<IQueryable<UniappVersion>> CreateFilteredQueryAsync(UniappVersionGetListDto input)
        {
            return (await base.CreateFilteredQueryAsync(input)).Where(v => v.AppId == input.AppId);
        }

        public virtual async Task<UniappVersionDto> GetPublicLatestAsync(Guid appId)
        {
            var uniapp = await GetAvailableUniappByIdAsync(appId);

            var version = await _uniappVersionRepository.GetLatestByAppIdAsync(uniapp.Id);

            return ObjectMapper.Map<UniappVersion, UniappVersionDto>(version);
        }

        public virtual async Task<UniappVersionDto> GetPublicLatestByAppNameAsync(string name)
        {
            var uniapp = await GetAvailableUniappByNameAsync(name);

            return await GetPublicLatestAsync(uniapp.Id);
        }
        
        public virtual async Task<UniappVersionDto> GetPublicAsync(Guid appId, string tag)
        {
            var uniapp = await GetAvailableUniappByIdAsync(appId);

            var version = await _uniappVersionRepository.GetByAppIdAsync(uniapp.Id, tag);

            return ObjectMapper.Map<UniappVersion, UniappVersionDto>(version);
        }

        public virtual async Task<UniappVersionDto> GetPublicByAppNameAsync(string name, string tag)
        {
            var uniapp = await GetAvailableUniappByNameAsync(name);

            var version = await _uniappVersionRepository.GetByAppIdAsync(uniapp.Id, tag);

            return ObjectMapper.Map<UniappVersion, UniappVersionDto>(version);
        }

        private async Task<Uniapp> GetAvailableUniappByIdAsync(Guid id)
        {
            var uniapp = await _uniappRepository.GetAsync(id);

            CheckUniappAvailable(uniapp);

            return uniapp;
        }

        private async Task<Uniapp> GetAvailableUniappByNameAsync(string name)
        {
            var uniapp = await _uniappRepository.FindByNameAsync(name);

            if (uniapp == null)
            {
                throw new UniappNotFoundException(name);
            }
            
            CheckUniappAvailable(uniapp);

            return uniapp;
        }

        protected virtual void CheckUniappAvailable(Uniapp uniapp)
        {
            if (!uniapp.IsAvailable)
            {
                throw new UniappUnavailableException(uniapp);
            }
        }
    }
}