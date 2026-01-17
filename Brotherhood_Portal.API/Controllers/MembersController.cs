using Brotherhood_Portal.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Brotherhood_Portal.API.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {
        /*Get Member*/
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepository.GetMembersAsync();
            if (members == null || members.Count == 0) return NotFound("No members found");
            return Ok(members);
        }

        /*Get Members By ID*/
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMemberById(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);
            if (member == null) return NotFound("Member not found");
            return Ok(member);
        }

        /*Get Photos For Member*/
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
        {
            var photos = await memberRepository.GetPhotosForMembersAsync(id);
            if (photos == null || photos.Count == 0) return NotFound("No photos found for this member");
            return Ok(photos);
        }

    }
}
