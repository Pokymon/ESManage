// Tujuan: Controller untuk mengelola data Supplier

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
    [Route("api/supplier")]
    public class SupplierController : ControllerBase {
        private readonly SupplierRepository _repository;

        public SupplierController(SupplierRepository repository)
        {
            _repository = repository;
        }

        // Metode GET untuk mendapatkan semua data supplier
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try {
                var supplier = await _repository.GetAll();
                return Ok(supplier);
            }
            catch (Exception ex) {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data supplier berdasarkan ID
        // Format pemanggilan: GET /api/supplier/id/{id}
        // Contoh pemanggilan: GET /api/supplier/id/1
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var supplier = await _repository.GetById(id);
                if (supplier == null) {
                    return NotFound($"Tidak ada Supplier dengan id: {id}");
                }

                return Ok(supplier);
            }
            catch (Exception ex)
            {
                Logger.WriteToConsole(Logger.LogType.Error, ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode GET untuk mendapatkan data supplier berdasarkan Name
        // Format pemanggilan: GET /api/supplier/name/{name}
        // Contoh pemanggilan: GET /api/supplier/name/Supplier 1
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var supplier = await _repository.GetByName(name);
                if (supplier == null) {
                    return NotFound($"Tidak ada Supplier dengan name: {name}");
                }

                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode POST untuk menambahkan data supplier
        [HttpPost]
        public async Task<IActionResult> Create(SupplierModel supplier)
        {
            try
            {
                var newSupplier = await _repository.Create(supplier);
                return Ok(newSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode PUT untuk mengubah data supplier
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SupplierModel supplier)
        {
            try
            {
                var updatedSupplier = await _repository.Update(id, supplier);
                return Ok(updatedSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Metode DELETE untuk menghapus data supplier
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var supplier = await _repository.GetById(id);
                if (supplier == null) {
                    return NotFound($"Tidak ada supplier ditemukan dengan id: {id}");
                }

                var deletedSupplier = await _repository.Delete(id);
                return Ok(new { success = true, message = $"Supplier: {supplier.SupplierName} dengan id: {id} berhasil dihapus"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
