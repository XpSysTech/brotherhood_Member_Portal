using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepository, AppDBContext appDBContext) : BaseApiController
    {
        #region Get Members
        /*
            - Summary:
                Retrieves a list of all members.
         */
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepository.GetMembersAsync();
            if (members == null || members.Count == 0) return NotFound("No members found");
            return Ok(members);
        }
        #endregion

        #region Get Members By ID
        /*
            - Summary:
                Retrieves a specific member by their ID.
         */
        [HttpGet("{userId}")]
        public async Task<ActionResult<Member>> GetMemberById(string userId)
        {
            var member = await memberRepository.GetMemberByIdAsync(userId);
            if (member == null) return NotFound("Member not found");
            return Ok(member);
        }
        #endregion

        #region Get Photos For Member
        /*
            - Summary:
                Retrieves all photos associated with a specific member by their ID.
         */
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var photos = await memberRepository.GetPhotosForMembersAsync(id);
            if (photos == null || photos.Count == 0) return NotFound("No photos found for this member");
            return Ok(photos);
        }
        #endregion

        #region Get Member Phonebook Info
        [HttpGet("phonebook")] // GET /api/members/phonebook
        public async Task<List<MemberPhonebookDTO>> GetPhonebookAsync()
        {
            return await appDBContext.Members
                .Where(m => m.IsActive)
                .Select(m => new MemberPhonebookDTO
                {
                    Id = m.Id,
                    DisplayName = m.DisplayName!,
                    FirstName = m.FirstName,
                    lastName = m.LastName,
                    Occupation = m.Occupation,
                    Business = m.Business,
                    ContactNumber = m.ContactNumber!,
                    /* Future Info
                        - Brotherhood Title / Responsibility / CommunityRole 
                        - Social Links
                        - Profile Image (Unless I can use the GetPhotosForMemberAsync Endpoint)
                     */
                    //Role = m.UserRoles.FirstOrDefault().Role.Name
                })
                .ToListAsync();
        }
        #endregion

    }
}
