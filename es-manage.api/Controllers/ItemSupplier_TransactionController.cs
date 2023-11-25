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
    [Route("api/itemsupplier_transaction")]
    public class ItemSupplier_TransactionController : ControllerBase {
        private readonly ItemSupplier_TransactionRepository _repository;

        public ItemSupplier_TransactionController(ItemSupplier_TransactionRepository repository)
        {
            _repository = repository;
        }

        public enum TransactionType
        {
            pembelian,
            penerimaan,
            pengembalian,
            pengiriman
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemSupplier_TransactionModel>>> GetAll()
        {
            try
            {
                var transactions = await _repository.GetAll();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemSupplier_TransactionModel>> GetById(string id)
        {
            try
            {
                var transaction = await _repository.GetById(id);
                if (transaction == null) return NotFound($"Transaction with ID: {id} not found");

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("itemsupplierid/{itemSupplierId}")]
        public async Task<ActionResult<IEnumerable<ItemSupplier_TransactionModel>>> GetByItemSupplierId(string itemSupplierId)
        {
            try
            {
                var transactions = await _repository.GetByItemSupplierId(itemSupplierId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("transactiontype/{transactionType}")]
        public async Task<ActionResult<IEnumerable<ItemSupplier_TransactionModel>>> GetByTransactionType(string transactionType)
        {
            try
            {
                var transactions = await _repository.GetByTransactionType(transactionType);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemSupplier_TransactionModel>> Create(ItemSupplier_TransactionPlusItemSupplier model)
        {
            try
            {
                var createdTransaction = await _repository.Create(model);
                return CreatedAtAction(nameof(GetById), new { id = createdTransaction.ID }, createdTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ItemSupplier_TransactionModel model)
        {
            if (id != model.ID)
            {
                return BadRequest("The ID in the URL does not match the ID in the provided data.");
            }

            try
            {
                var updatedTransaction = await _repository.Update(id, model);
                return Ok(updatedTransaction);
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
                var transaction = await _repository.GetById(id);
                if (transaction == null) {
                    return NotFound($"No transaction found with id: {id}");
                }

                var deletedTransaction = await _repository.Delete(id);
                return Ok(new { success = true, message = $"Transaction with id: {id} has been deleted"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}