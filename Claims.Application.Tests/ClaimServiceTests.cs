using AutoMapper;
using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Repositories;
using Claims.Application.Services;
using Claims.Application.Validation;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using FluentValidation;
using NSubstitute;

namespace Claims.Application.Tests
{
    public class ClaimServiceTests
    {
        private readonly IClaimRepository _mockClaimRepository;
        private readonly IClaimAuditRepository _mockClaimAuditRepository;
        private readonly ICoverRepository _mockCoverRepository;
        private readonly IMapper _mapper;
        private readonly ClaimService _claimService;
        private readonly EnhancedClaimValidator _enhancedClaimValidator;

        public ClaimServiceTests()
        {
            _mockClaimRepository = Substitute.For<IClaimRepository>();
            _mockClaimAuditRepository = Substitute.For<IClaimAuditRepository>();
            _mockCoverRepository = Substitute.For<ICoverRepository>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); 
            });

            _mapper = mapperConfiguration.CreateMapper();

            _enhancedClaimValidator = new EnhancedClaimValidator(_mockCoverRepository);

            _claimService = new ClaimService(_mockClaimRepository, _mockClaimAuditRepository, _mapper, _enhancedClaimValidator);
        }

        [Fact]
        public async Task GetAllAsync_MapsPropertiesCorrectly()
        {
            var claim = new Claim("1", DateTime.Now, "coverIdPlaceholder", ClaimType.Collision, 10000);
            _mockClaimRepository.GetAllAsync().Returns(new List<Claim> { claim });

            var claimsDto = await _claimService.GetAllAsync();

            Assert.Single(claimsDto);
            Assert.Equal(claim.DamageCost, claimsDto.First().DamageCost);
        }

        [Fact]
        public async Task GetByIdAsync_MapsPropertiesCorrectly()
        {
            var claim = new Claim("1", DateTime.Now, "coverIdPlaceholder", ClaimType.Collision, 10000);
            _mockClaimRepository.GetByIdAsync("1").Returns(claim);

            var claimDto = await _claimService.GetByIdAsync("1");

            Assert.Equal(claim.Id, claimDto.Id);
            Assert.Equal(claim.DamageCost, claimDto.DamageCost);
        }

        [Fact]
        public async Task CreateAsync_MapsPropertiesCorrectly()
        {
            var cover = new Cover("1", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now.AddDays(3)), CoverType.BulkCarrier, 1);
            _mockCoverRepository.GetByIdAsync("1").Returns(cover);

            var claimDto = new ClaimDataTransferObject { CoverId = "1", DamageCost = 10000, Created = DateTime.Now.AddDays(2) };
            var createdClaim = await _claimService.CreateAsync(claimDto);

            Assert.Equal(claimDto.DamageCost, createdClaim.DamageCost);
        }

        [Fact]
        public async Task CreateAsync_EnsuresClaimValidatorWorks()
        {
            var claimDto = new ClaimDataTransferObject { DamageCost = 100001 };
            await Assert.ThrowsAsync<ArgumentException>(() => _claimService.CreateAsync(claimDto));
        }

        [Fact]
        public async Task CreateAsync_EnsuresEnhancedClaimValidatorWorks()
        {
            var cover = new Cover("1", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now.AddDays(3)), CoverType.BulkCarrier, 1);
            _mockCoverRepository.GetByIdAsync("1").Returns(cover);

            var claimDto = new ClaimDataTransferObject { CoverId = "1", Created = DateTime.Now };
            await Assert.ThrowsAsync<ValidationException>(() => _claimService.CreateAsync(claimDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_CallsClaimRepositoryDeleteAndAuditRepositoryAudit()
        {
            string validId = "test123";

            await _claimService.DeleteAsync(validId);

            await _mockClaimRepository.Received(1).DeleteAsync(validId);
            await _mockClaimAuditRepository.Received(1).AuditAsync(validId, "DELETE");
        }
    }

}
