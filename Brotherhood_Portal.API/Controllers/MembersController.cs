using Asp.Versioning;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Identity.Command;
using Brotherhood_Portal.Domain.DTOs.Member.Query;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
    [ApiVersion("1.0")]
    public class MembersController(IMemberRepository memberRepository, AppDBContext appDBContext, ILogger<MembersController> logger) : BaseApiController
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
        /// 
        /// [5] LOGGING
        /// - Logs requesting user.
        /// - Logs total member count returned.
        /// - Logs warning if no members exist.
        /// </summary>

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var requester = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "User {RequesterId} requested full member list",
                requester);

            var members = await memberRepository.GetMembersAsync();

            if (members == null || members.Count == 0)
            {
                logger.LogWarning(
                    "User {RequesterId} requested members but none were found",
                    requester);

                return NotFound("No members found");
            }

            logger.LogInformation(
                "User {RequesterId} retrieved {MemberCount} members",
                requester,
                members.Count);

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
        /// 
        /// [5] LOGGING
        /// - Logs requesting user.
        /// - Logs target member identifier.
        /// - Logs warning if target member does not exist.

        /// </summary>

        [HttpGet("{userId}")]
        public async Task<ActionResult<Member>> GetMemberById(string userId)
        {
            var requester = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "User {RequesterId} requested member {TargetMemberId}",
                requester,
                userId);

            var member = await memberRepository.GetMemberByIdAsync(userId);

            if (member == null)
            {
                logger.LogWarning(
                    "Member {TargetMemberId} not found. Requested by {RequesterId}",
                    userId,
                    requester);

                return NotFound("Member not found");
            }

            return Ok(member);
        }

        #endregion


        #region Get Member Profile

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves the profile of the currently authenticated member.
        ///
        /// [2] BUSINESS RULES
        /// - The requesting user must be authenticated.
        /// - The Member entity must exist for the authenticated user.
        /// - Projection maps Member entity to UpdateMemberDto.
        /// 
        /// [3] RESPONSE
        /// - Returns UpdateMemberDto.
        /// - Returns 404 if member record does not exist.
        /// 
        /// [4] SECURITY
        /// - Requires authenticated user.
        /// - Only returns the profile of the currently authenticated user.
        /// - Does not expose financial or system-sensitive data.
        /// 
        /// [5] LOGGING
        /// - Logs user identifier requesting profile.
        /// - Logs warning if profile not found.
        /// </summary>

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UpdateMemberDto>> GetMemberProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
            "User {UserId} requested their own profile",
            userId);

            var member = await memberRepository.GetMemberByIdAsync(userId);

            if (member == null)
            {
                logger.LogWarning(
                    "Profile not found for user {UserId}",
                    userId);

                return NotFound("Member not found");
            }

            var dto = new UpdateMemberDto
            {
                FirstName = member.FirstName!,
                LastName = member.LastName!,
                Email = member.User.Email!,
                ContactNumber = member.ContactNumber!,
                HomeAddress = member.HomeAddress!,
                HomeCity = member.HomeCity!,
                DateOfBirth = member.DateOfBirth,
                MemberBiography = member.MemberBiography,
                Occupation = member.Occupation,
                Business = member.Business,
                ImageUrl = member.ImageUrl
            };

            return Ok(dto);
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
        /// 
        /// [5] LOGGING
        /// - Logs requesting user.
        /// - Logs target member identifier.
        /// - Logs photo count returned.
        /// - Logs warning if no photos found.
        /// </summary>

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var requester = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "User {RequesterId} requested photos for member {TargetMemberId}",
                requester,
                id);

            var photos = await memberRepository.GetPhotosForMembersAsync(id);

            if (photos == null || photos.Count == 0)
            {
                logger.LogWarning(
                    "No photos found for member {TargetMemberId}. Requested by {RequesterId}",
                    id,
                    requester);

                return NotFound("No photos found for this member");
            }

            logger.LogInformation(
                "Returned {PhotoCount} photos for member {TargetMemberId}",
                photos.Count,
                id);

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
        /// 
        /// [5] LOGGING
        /// - Logs requesting user.
        /// - Logs total number of active members returned.
        /// - Does not log individual member details.
        /// </summary>

        [HttpGet("phonebook")]
        public async Task<List<MemberPhonebookDTO>> GetPhonebookAsync()
        {
            var requester = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "User {RequesterId} requested phonebook projection",
                requester);

            var result = await appDBContext.Members
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

            logger.LogInformation(
                "Phonebook returned {Count} active members",
                result.Count);

            return result;
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
        /// 
        /// [5] LOGGING
        /// - Logs requesting user.
        /// - Logs number of members returned.
        /// - Projection excludes sensitive data.
        /// </summary>

        [HttpGet("dropdown")]
        public async Task<List<MemberByNameDTO>> GetMembersForDropdown()
        {
            var requester = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "User {RequesterId} requested dropdown projection",
                requester);

            var result = await appDBContext.Members
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

            logger.LogInformation(
                "Dropdown projection returned {Count} members",
                result.Count);

            return result;
        }

        #endregion
    }
}
