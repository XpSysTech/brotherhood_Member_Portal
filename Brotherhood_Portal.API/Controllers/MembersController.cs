using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Member.Query;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    /// <summary>
    /// Provides read-only access to Member data.
    /// 
    /// Access Policy:
    /// - All endpoints require authenticated users.
    /// 
    /// Responsibilities:
    /// - Member listing
    /// - Member profile retrieval
    /// - Phonebook projections
    /// - Lightweight dropdown projections
    /// </summary>
    
    [Authorize]
    public class MembersController(IMemberRepository memberRepository, AppDBContext appDBContext) : BaseApiController
    {
        #region Get All Members

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a full list of all members.
        ///
        /// [2] BUSINESS RULES
        /// - Returns only existing members.
        /// - Does not apply filtering for active/inactive (repository controlled).
        ///
        /// [3] RESPONSE
        /// - Returns complete Member entities.
        /// - Returns 404 if no members exist.
        ///
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// </summary>
        
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepository.GetMembersAsync();

            if (members == null || members.Count == 0)
                return NotFound("No members found");

            return Ok(members);
        }

        #endregion


        #region Get Member By Id

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves a specific member profile by user identifier.
        ///
        /// [2] BUSINESS RULES
        /// - Member must exist.
        ///
        /// [3] RESPONSE
        /// - Returns full Member entity.
        /// - Returns 404 if not found.
        ///
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// </summary>
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<Member>> GetMemberById(string userId)
        {
            var member = await memberRepository.GetMemberByIdAsync(userId);

            if (member == null)
                return NotFound("Member not found");

            return Ok(member);
        }

        #endregion


        #region Get Member Photos

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves all photos associated with a member.
        ///
        /// [2] BUSINESS RULES
        /// - Member must exist.
        /// - Photos must exist.
        ///
        /// [3] RESPONSE
        /// - Returns collection of Photo entities.
        /// - Returns 404 if none found.
        ///
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// </summary>
        
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var photos = await memberRepository.GetPhotosForMembersAsync(id);

            if (photos == null || photos.Count == 0)
                return NotFound("No photos found for this member");

            return Ok(photos);
        }

        #endregion


        #region Get Phonebook Projection

        /// <summary>
        /// [1] PURPOSE
        /// Returns a lightweight projection of active members for phonebook usage.
        ///
        /// [2] BUSINESS RULES
        /// - Only active members are included.
        /// - Returns limited public-facing fields.
        ///
        /// [3] RESPONSE
        /// - MemberPhonebookDTO list.
        ///
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// - Does not expose sensitive financial or identity information.
        /// </summary>
        
        [HttpGet("phonebook")]
        public async Task<List<MemberPhonebookDTO>> GetPhonebookAsync()
        {
            return await appDBContext.Members
                .Where(m => m.IsActive)
                .Select(m => new MemberPhonebookDTO
                {
                    Id = m.Id,
                    DisplayName = m.DisplayName!,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Occupation = m.Occupation,
                    Business = m.Business,
                    ContactNumber = m.ContactNumber!
                })
                .ToListAsync();
        }

        #endregion


        #region Get Dropdown Projection

        /// <summary>
        /// [1] PURPOSE
        /// Returns a minimal member projection for dropdown selection.
        ///
        /// [2] BUSINESS RULES
        /// - Only active members are returned.
        /// - Sorted alphabetically by DisplayName.
        /// - Intended for UI selection components.
        ///
        /// [3] RESPONSE
        /// - MemberByNameDTO list (Id + DisplayName).
        ///
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// - Does not expose sensitive or financial data.
        /// </summary>
        
        [HttpGet("dropdown")]
        public async Task<List<MemberByNameDTO>> GetMembersForDropdown()
        {
            return await appDBContext.Members
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayName)
                .Select(m => new MemberByNameDTO
                {
                    Id = m.Id,
                    DisplayName = m.DisplayName!,
                    FirstName = m.FirstName,
                    LastName = m.LastName
                })
                .ToListAsync();
        }

        #endregion
    }
}
