// Tujuan: Berisi metode CRUD untuk model UserMst.

// Import library yang dibutuhkan
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using es_manage.api.Repositories;
using es_manage.api.Utilities;
using es_manage.api.Services;
using es_manage.api.Models;

// Membuat namespace
namespace es_manage.api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
// Membuat class UsersController yang mewarisi ControllerBase
public class UsersController : ControllerBase
{
    // Membuat private readonly UserRepository
    private readonly UserRepository _repository;

    // Membuat constructor UsersController yang menerima parameter UserRepository
    public UsersController(UserRepository repository)
    {
        _repository = repository;
    }

    // Membuat metode GetAll untuk mengambil semua data user
    [HttpGet]
    public async Task<IActionResult> GetAll() {
        try {
            var users = await _repository.GetAll();
            return Ok(users);
        }
        catch (Exception ex) {
            Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Membuat metode Get untuk mengambil data user berdasarkan ID berupa UUID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var user = await _repository.Get(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Membuat metode Create untuk menambahkan data user
    [HttpPost]
    public async Task<IActionResult> Create(Models.UserMst user)
    {
        try
        {
            var newUser = await _repository.Create(user);
            return CreatedAtAction(nameof(Get), new { id = newUser.ID }, newUser);
        }
        catch (Exception ex)
        {
            Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Membuat metode Update untuk mengubah data user berdasarkan ID berupa UUID
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Models.UserMst user)
    {
        try
        {
            var existingUser = await _repository.Get(id);
            if (existingUser == null)
                return NotFound();
            var updatedUser = await _repository.Update(id, user);
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Membuat metode Delete (soft-delete) untuk menghapus data user berdasarkan ID berupa UUID
    //[Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        //string userName = User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
        //Logger.WriteToConsole(Logger.LogType.Info, $"User {userName} menghapus user dengan ID {id}");
        try
        {
            var existingUser = await _repository.Get(id);
            if (existingUser == null)
                return NotFound();
            await _repository.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}