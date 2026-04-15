using Brotherhood_Portal.Domain.DTOs.Member.Query;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Interfaces
{
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<bool> SaveAllAsync();
        Task<IReadOnlyList<Member>> GetMembersAsync();
        //Task<Member?> GetMemberByIdAsync(string id);
        //Task<Member?> GetMemberByIdAsync(string userId);
        Task<MemberDto?> GetMemberByIdAsync(string userId);
        Task<IReadOnlyList<Photo>> GetPhotosForMembersAsync(string memberId);
    }
}
