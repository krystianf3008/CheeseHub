using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role;
using CheeseHub.Models.Role.DTOs;
using CheeseHub.Models.User;
using CheeseHub.Models.Video;
using CheeseHub.Services;
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
        public async Task<IActionResult> Post([FromForm] CreateRoleDTO model)
        {
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
