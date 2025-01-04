using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role;
using CheeseHub.Models.Role.DTOs;
using CheeseHub.Models.Role.Validators;
using CheeseHub.Models.User;
using CheeseHub.Models.User.Validators;
using CheeseHub.Models.Video;
using CheeseHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CheeseHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IWebHostEnvironment _environment;

        public RoleController(IRoleService roleService, IWebHostEnvironment environment)
        {
            _roleService = roleService;
        }
        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IActionResult> Get([FromBody] Guid Id)
        {
            Role role= await _roleService.GetById(Id);
            if (role == null)
            {
                return NotFound("Role is null");
            }

            return Ok(role);
        }
        [HttpPost]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Post([FromForm] CreateRoleDTO model)
        {
            CreateRoleDTOValidator validator = new CreateRoleDTOValidator(_roleService);
            var result = validator.Validate(model);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            Role role = new Role
            {
                Name = model.Name,

            };
           Guid guid = await _roleService.Add(role);
            return CreatedAtRoute(
                     "Get",
                     new { Id = role.Id },
                     role);
        }

    }
}
