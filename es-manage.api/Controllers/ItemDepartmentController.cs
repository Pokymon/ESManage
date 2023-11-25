// Tujuan: Controller untuk mengelola data ItemDepartment

// Import library yang dibutuhkan
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using es_manage.api.Utilities;
using es_manage.api.Repositories;
using es_manage.api.Models;
using es_manage.api.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace es_manage.api.Controllers {
    [ApiController]
    [Authorize]
    [Route("api/itemdepartment")]
    public class ItemDepartmentController : ControllerBase {
        private readonly ItemDepartmentRepository _repository;

        public ItemDepartmentController(ItemDepartmentRepository repository)
        {
            _repository = repository;
        }

        // Metode GET untuk mendapatkan semua data department
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try {
                var departments = await _repository.GetAll();
                return Ok(departments);
            }
            catch (Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data department berdasarkan ID
        // Format pemanggilan: GET /api/itemdepartment/id/{id}
        // Contoh pemanggilan: GET /api/itemdepartment/id/1
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var department = await _repository.GetById(id);
                if (department == null) {
                    return NotFound($"Tidak ada Item Department dengan id: {id}");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data department berdasarkan CategoryName
        // Format pemanggilan: GET /api/itemdepartment/category/{categoryName}
        // Contoh pemanggilan: GET /api/itemdepartment/category/Komputer
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetByCategoryName(string categoryName)
        {
            try
            {
                var departments = await _repository.GetByCategory(categoryName);
                if (departments == null)
                {
                    return NotFound($"Tidak ada Item Department dengan category: {categoryName}");
                }

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode POST untuk menambahkan data department
        [HttpPost]
        public async Task<IActionResult> Create(ItemDepartmentModel department)
        {
            try
            {
                var newDepartment = await _repository.Create(department);
                return CreatedAtAction(nameof(GetById), new { id = newDepartment.ID }, newDepartment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode PUT untuk mengubah data department
        // Format pemanggilan: PUT /api/itemdepartment/{id}/{categoryName}
        // Contoh pemanggilan: PUT /api/itemdepartment/1/Department
        /*[HttpPut("{id}/{categoryName}")]
        public async Task<IActionResult> Update(string id, string categoryName, ItemDepartmentModel department)
        {
            try
            {
                var existingDepartment = await _repository.GetByCategoryAndId(id, categoryName);

                if (existingDepartment != null)
                {
                    await _repository.Update(department);
                    return Ok(department);
                }
                else
                {
                    return NotFound($"Tidak ada Item Department dengan id {id} dan category {categoryName}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }*/

        [HttpPut("{id}/{categoryName}")]
        public async Task<IActionResult> Update(string id, string categoryName, [FromBody] ItemDepartmentModel department)
        {
            try
            {
                if (department.ID != id || department.CategoryName != categoryName)
                {
                    return BadRequest(new { success = false, message = "ID dan CategoryName di URL tidak sama dengan ID dan CategoryName di body request." });
                }
                else {
                    var existingDepartment = await _repository.GetByCategoryAndId(id, categoryName);

                    if (existingDepartment != null)
                    {
                        await _repository.Update(department);
                        return Ok(department);
                    }
                    else
                    {
                        return NotFound($"Tidak ada Item Department dengan id {department.ID} dan category {department.CategoryName}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode DELETE untuk menghapus data department
        // Format pemanggilan: DELETE /api/itemdepartment/{id}/{categoryName}
        // Contoh pemanggilan: DELETE /api/itemdepartment/1/Department
        [HttpDelete("{id}/{categoryName}")]
        public async Task<IActionResult> Delete(string id, string categoryName)
        {
            try
            {
                var result = await _repository.Delete(id, categoryName);

                if (result > 0)
                {
                    return Ok(new { success = true, message = $"Item Department id: {id} category: {categoryName} berhasil dihapus"});
                }
                else
                {
                    return NotFound(new { success = false, message = $"Tidak ada Item Department id: {id} category: {categoryName} untuk dihapus"});
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}