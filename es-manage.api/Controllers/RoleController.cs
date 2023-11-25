using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using es_manage.api.Utilities;
using es_manage.api.Repositories;
using es_manage.api.Models;

namespace es_manage.api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/role")]
    public class RoleController : ControllerBase
    {
        private readonly RoleRepository _repository;

        public RoleController(RoleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _repository.GetAll();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var role = await _repository.GetById(id);
                if (role == null)
                {
                    return NotFound($"No Role found with id: {id}");
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleModel role)
        {
            try
            {
                var newRole = await _repository.Create(role);
                return Ok(newRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoleModel role)
        {
            try
            {
                var updatedRole = await _repository.Update(id, role);
                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var role = await _repository.GetById(id);
                if (role == null)
                {
                    return NotFound($"No Role found with id: {id}");
                }

                var deletedRole = await _repository.Delete(id);
                return Ok(new { success = true, message = $"Role with id: {id} has been deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
