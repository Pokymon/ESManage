// Tujuan: Controller untuk mengelola data Brand

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
    [Route("api/brand")]
    public class BrandController : ControllerBase {
        private readonly BrandRepository _repository;

        public BrandController(BrandRepository repository)
        {
            _repository = repository;
        }

        // Metode GET untuk mendapatkan semua data brand
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try {
                var brands = await _repository.GetAll();
                return Ok(brands);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data brand berdasarkan ID
        // Format pemanggilan: GET /api/brand/id/{id}
        // Contoh pemanggilan: GET /api/brand/id/1
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var brand = await _repository.GetById(id);
                if (brand == null) {
                    return NotFound($"Tidak ada Brand dengan id: {id}");
                }

                return Ok(brand);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data brand berdasarkan Name
        // Format pemanggilan: GET /api/brand/name/{name}
        // Contoh pemanggilan: GET /api/brand/name/Brand 1
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var brand = await _repository.GetByName(name);
                if (brand == null) {
                    return NotFound($"Tidak ada Brand dengan name: {name}");
                }

                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode POST untuk menambahkan data brand
        [HttpPost]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            try
            {
                var newBrand = await _repository.Create(brand);
                return Ok(newBrand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode PUT untuk mengubah data brand
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BrandModel brand)
        {
            try
            {
                var updatedBrand = await _repository.Update(id, brand);
                return Ok(updatedBrand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode DELETE untuk menghapus data brand
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var brand = await _repository.GetById(id);
                if (brand == null) {
                    return NotFound($"Tidak ada brand ditemukan dengan id: {id}");
                }

                var deletedBrand = await _repository.Delete(id);
                return Ok(new { success = true, message = $"Brand: {brand.Name} dengan id: {id} berhasil dihapus"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}