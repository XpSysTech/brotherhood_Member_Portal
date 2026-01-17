using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    public class MemberRepository(AppDBContext appDBContext) : IMemberRepository
    {
        public async Task<Member?> GetMemberByIdAsync(string id)
        {
            return await appDBContext.Members.FindAsync(id);
        }

        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await appDBContext.Members
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosForMembersAsync(string memberId)
        {
            return await appDBContext.Members
                .Where(m => m.Id == memberId)
                .SelectMany(m => m.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await appDBContext.SaveChangesAsync() > 0;
        }

        public void Update(Member member)
        {
            appDBContext.Entry(member).State = EntityState.Modified;
        }
    }
}
