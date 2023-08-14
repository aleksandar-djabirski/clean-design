using AutoMapper;
using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Repositories;
using Claims.Application.Interfaces.Services;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Validation;
using FluentValidation;
using NSubstitute;

namespace Claims.Application.Tests
{
    public class CoverServiceTests
    {
        private readonly ICoverRepository _mockCoverRepository;
        private readonly ICoverAuditRepository _mockCoverAuditRepository;
        private readonly IMapper _mapper;
        private readonly CoverValidator _coverValidator;
        private readonly CoverService _coverService;

        public CoverServiceTests()
        {
            _mockCoverRepository = Substitute.For<ICoverRepository>();
            _mockCoverAuditRepository = Substitute.For<ICoverAuditRepository>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfiguration.CreateMapper();

            _coverValidator = new CoverValidator();

            _coverService = new CoverService(_mockCoverRepository, _mockCoverAuditRepository, _mapper, _coverValidator);
        }

        [Fact]
        public async Task GetAllAsync_MapsPropertiesCorrectly()
        {
            var cover = new Cover("1", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now.AddDays(2)), CoverType.BulkCarrier, 1200);
            _mockCoverRepository.GetAllAsync().Returns(new List<Cover> { cover });

            var coversDto = await _coverService.GetAllAsync();

            Assert.Equal(cover.Premium, coversDto.First().Premium);
        }

        [Fact]
        public async Task GetByIdAsync_MapsPropertiesCorrectly()
        {
            var cover = new Cover("1", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now.AddDays(2)), CoverType.BulkCarrier, 1200);
            _mockCoverRepository.GetByIdAsync("1").Returns(cover);

            var coverDto = await _coverService.GetByIdAsync("1");

            Assert.Equal(cover.Id, coverDto.Id);
            Assert.Equal(cover.Premium, coverDto.Premium);
        }

        [Fact]
        public async Task CreateAsync_MapsPropertiesCorrectly()
        {
            var coverDto = new CoverDataTransferObject { Premium = 1200, StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)) };
            var createdCover = await _coverService.CreateAsync(coverDto);

            Assert.Equal(coverDto.StartDate, createdCover.StartDate);
        }


        [Fact]
        public async Task CreateAsync_CalculatePremiumCorrectly()
        {
            var coverDto = new CoverDataTransferObject { Premium = 1200, StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)) };
            var createdCover = await _coverService.CreateAsync(coverDto);

            Assert.Equal((decimal)3946.250, createdCover.Premium);
        }

        [Fact]
        public async Task CreateAsync_EnsuresCoverValidatorStartDate()
        {
            var coverDto = new CoverDataTransferObject { StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) };
            await Assert.ThrowsAsync<ArgumentException>(() => _coverService.CreateAsync(coverDto));
        }

        [Fact]
        public async Task CreateAsync_EnsuresCoverValidatorInsurancePeriod()
        {
            var coverDto = new CoverDataTransferObject
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now.Date),
                EndDate = DateOnly.FromDateTime(DateTime.Now.Date.AddDays(366))
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _coverService.CreateAsync(coverDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_CallsCoverRepositoryDeleteAndAuditRepositoryAudit()
        {
            string validId = "test123";

            await _coverService.DeleteAsync(validId);

            await _mockCoverRepository.Received(1).DeleteAsync(validId);
            _mockCoverAuditRepository.Received(1).Audit(validId, "DELETE");
        }


        [Theory]
        [InlineData(CoverType.Yacht, 30, 39875)] // 1250 * 1.1 for 30 days
        [InlineData(CoverType.PassengerShip, 30, 43500)] // 1250 * 1.2 for 30 days
        [InlineData(CoverType.Tanker, 30, 54375)] // 1250 * 1.5 for 30 days
        [InlineData(CoverType.BulkCarrier, 30, 47125)] // 1250 * 1.3 for 30 days
        public void ComputePremium_First30Days(CoverType coverType, int days, decimal expectedPremium)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = startDate.AddDays(days - 1);  // Subtracting 1 to include the start day

            var computedPremium = _coverService.ComputePremium(startDate, endDate, coverType);

            Assert.Equal(expectedPremium, computedPremium);
        }

        [Theory]
        [InlineData(CoverType.Yacht, 150, 196693.75)] // 1250 * 1.1 for 30 days + 1250 * 1.1 * 0.95 for 119 days
        public void ComputePremium_Days31To180(CoverType coverType, int days, decimal expectedPremium)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(31));
            var endDate = startDate.AddDays(days - 1);

            var computedPremium = _coverService.ComputePremium(startDate, endDate, coverType);

            Assert.Equal(expectedPremium, computedPremium);
        }

        [Theory]
        [InlineData(CoverType.Yacht, 185, 242247.5)] //1250 * 1.1 * 30  + (1250 * 1.1 * 0.95 * 150) +  (1250 * 1.1 * 0.92 * 4)
        public void ComputePremium_DaysBeyond180(CoverType coverType, int days, decimal expectedPremium)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(181));
            var endDate = startDate.AddDays(days - 1);

            var computedPremium = _coverService.ComputePremium(startDate, endDate, coverType);

            Assert.Equal(expectedPremium, computedPremium);
        }
    }
}
