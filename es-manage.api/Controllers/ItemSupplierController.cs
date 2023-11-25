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
    [Route("api/itemsupplier")]
    public class ItemSupplierController : ControllerBase {
        private readonly ItemSupplierRepository _repository;

        public ItemSupplierController(ItemSupplierRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemSupplierModel>>> GetAll()
        {
            try
            {
                var itemSuppliers = await _repository.GetAll();
                return Ok(itemSuppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemSupplierModel>> GetById(string id)
        {
            try
            {
                var itemSupplier = await _repository.GetById(id);
                if (itemSupplier == null) return NotFound($"Tidak ada ItemSupplier dengan ID: {id}");

                return Ok(itemSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("itemid/{itemId}")]
        public async Task<ActionResult<IEnumerable<ItemSupplierModel>>> GetByItemId(string itemId)
        {
            try
            {
                var itemSuppliers = await _repository.GetByItemId(itemId);
                return Ok(itemSuppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("supplierid/{supplierId}")]
        public async Task<ActionResult<IEnumerable<ItemSupplierModel>>> GetBySupplierId(string supplierId)
        {
            try
            {
                var itemSuppliers = await _repository.GetBySupplierId(supplierId);
                return Ok(itemSuppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemSupplierModel>> Create(ItemSupplierModel model)
        {
            try
            {
                var createdItemSupplier = await _repository.Create(model);
                return CreatedAtAction(nameof(GetById), new { id = createdItemSupplier.ID }, createdItemSupplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ItemSupplierModel model)
        {
            if (id != model.ID)
            {
                return BadRequest("The ID in the URL does not match the ID in the provided data.");
            }

            try
            {
                var updatedItemSupplier = await _repository.Update(id,model);
                return Ok(updatedItemSupplier);
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
                var itemSupplier = await _repository.GetById(id);
                if (itemSupplier == null) {
                    return NotFound($"Tidak ada ItemSupplier ditemukan dengan id: {id}");
                }

                var deletedItemSupplier = await _repository.Delete(id);
                return Ok(new { success = true, message = $"ItemSupplier dengan id: {id} berhasil dihapus"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}