using Brotherhood_Portal.Domain.DTOs.Member.Query;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Extensions
{
    public static class MemberExtensions
    {
        public static MemberDto MemberExt(this Member member)
        {
            return new MemberDto
            {
                Id = member.Id,
                DateOfBirth = member.DateOfBirth,
                DisplayName = member.DisplayName,
                FirstName = member.FirstName,
                LastName = member.LastName,
                ImageUrl = member.ImageUrl,
                MemberBiography = member.MemberBiography,
                Occupation = member.Occupation,
                Business = member.Business,
                ContactNumber = member.ContactNumber,
                HomeAddress = member.HomeAddress,
                HomeCity = member.HomeCity,
                LastActive = member.LastActive,
                Created = member.Created,
                IsActive = member.IsActive,
                TotalSavings = member.TotalSavings,
                TotalOpsContribution = member.TotalOpsContribution,
                User = member.User == null ? null : new UserSummaryDto
                {
                    DisplayName = member.User.DisplayName!,
                    ImageUrl = member.User.ImageUrl
                }
            };
        }
    }
}
