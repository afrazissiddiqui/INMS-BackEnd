using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ARInvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ARInvoiceController(AppDbContext context) => _context = context;

        // GET: api/ARInvoice
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ARInvoiceResponse>>>> GetAll()
        {
            try
            {
                var invoices = await _context.ARInvoices
                    .Include(i => i.BusinessPartner)
                    .Include(i => i.Lines).ThenInclude(l => l.Item)
                    .Include(i => i.Lines).ThenInclude(l => l.Vat)
                    .AsNoTracking()
                    .ToListAsync();

                var result = invoices.Select(ARInvoiceResponse.FromEntity).ToList();
                return ApiResponse.Success<IEnumerable<ARInvoiceResponse>>(result);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<IEnumerable<ARInvoiceResponse>>(500, ex.Message);
            }
        }

        // GET: api/ARInvoice/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ARInvoiceResponse>>> GetById(int id)
        {
            try
            {
                var invoice = await _context.ARInvoices
                    .Include(i => i.BusinessPartner)
                    .Include(i => i.Lines).ThenInclude(l => l.Item)
                    .Include(i => i.Lines).ThenInclude(l => l.Vat)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == id);

                return invoice == null
                    ? ApiResponse.Fail<ARInvoiceResponse>(404, "Invoice not found")
                    : ApiResponse.Success(ARInvoiceResponse.FromEntity(invoice));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<ARInvoiceResponse>(500, ex.Message);
            }
        }

        // POST: api/ARInvoice
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ARInvoiceResponse>>> Create([FromBody] ARInvoiceCreateRequest request)
        {
            try
            {
                if (request == null || request.Lines == null || !request.Lines.Any())
                    return ApiResponse.Fail<ARInvoiceResponse>(400, "Invalid invoice data.");

                var invoice = new ARInvoice
                {
                    InvoiceNumber = request.InvoiceNumber,
                    InvoiceDate = request.InvoiceDate == default ? DateTime.UtcNow : request.InvoiceDate,
                    BusinessPartnerId = request.BusinessPartnerId,
                    Lines = new List<ARInvoiceLine>()
                };

                foreach (var line in request.Lines)
                {
                    var item = await _context.Items.FindAsync(line.ItemId);
                    if (item == null)
                        return ApiResponse.Fail<ARInvoiceResponse>(400, $"Item with ID {line.ItemId} not found.");

                    // ✅ Compute current stock from StockTransactions
                    var currentStock = await _context.StockTransactions
                        .Where(st => st.ItemId == item.Id)
                        .SumAsync(st => st.Qty);

                    if (!item.AllowNegativeInventory && currentStock < line.Quantity)
                    {
                        return ApiResponse.Fail<ARInvoiceResponse>(400,
                            $"Insufficient stock for {item.ItemName}. Available: {currentStock}, Requested: {line.Quantity}");
                    }

                    // ✅ Add ARInvoiceLine
                    var invoiceLine = new ARInvoiceLine
                    {
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        VatId = line.VatId
                    };
                    invoice.Lines.Add(invoiceLine);

                    // ✅ Insert StockTransaction (negative = stock out)
                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InvoiceType = InvoiceTypeEnum.AR,
                        InvoiceId = invoice.Id,
                        InvoiceLineId = invoiceLine.Id,
                        ItemId = line.ItemId,
                        Qty = -line.Quantity
                    });
                }

                invoice.TotalAmount = invoice.Lines.Sum(l => l.LineTotal);

                _context.ARInvoices.Add(invoice);
                await _context.SaveChangesAsync();

                var created = await _context.ARInvoices
                    .Include(i => i.BusinessPartner)
                    .Include(i => i.Lines).ThenInclude(l => l.Item)
                    .Include(i => i.Lines).ThenInclude(l => l.Vat)
                    .FirstOrDefaultAsync(i => i.Id == invoice.Id);

                return ApiResponse.Success(ARInvoiceResponse.FromEntity(created!));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<ARInvoiceResponse>(500, ex.Message);
            }
        }

        // PUT: api/ARInvoice/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<ARInvoiceResponse>>> Update(int id, [FromBody] ARInvoiceCreateRequest request)
        {
            try
            {
                var invoice = await _context.ARInvoices
                    .Include(i => i.Lines)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                    return ApiResponse.Fail<ARInvoiceResponse>(404, "Invoice not found");

                // Remove old lines + stock transactions
                var oldLineIds = invoice.Lines.Select(l => l.Id).ToList();
                var oldTransactions = _context.StockTransactions
                    .Where(st => st.InvoiceType == InvoiceTypeEnum.AR && oldLineIds.Contains(st.InvoiceLineId));
                _context.StockTransactions.RemoveRange(oldTransactions);

                _context.ARInvoiceLines.RemoveRange(invoice.Lines);

                // Update invoice header
                invoice.InvoiceNumber = request.InvoiceNumber;
                invoice.InvoiceDate = request.InvoiceDate;
                invoice.BusinessPartnerId = request.BusinessPartnerId;
                invoice.Lines = new List<ARInvoiceLine>();

                foreach (var line in request.Lines)
                {
                    var item = await _context.Items.FindAsync(line.ItemId);
                    if (item == null)
                        return ApiResponse.Fail<ARInvoiceResponse>(400, $"Item with ID {line.ItemId} not found.");

                    var currentStock = await _context.StockTransactions
                        .Where(st => st.ItemId == item.Id)
                        .SumAsync(st => st.Qty);

                    if (!item.AllowNegativeInventory && currentStock < line.Quantity)
                        return ApiResponse.Fail<ARInvoiceResponse>(400,
                            $"Insufficient stock for {item.ItemName}. Available: {currentStock}, Requested: {line.Quantity}");

                    var invoiceLine = new ARInvoiceLine
                    {
                        ItemId = line.ItemId,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        VatId = line.VatId
                    };
                    invoice.Lines.Add(invoiceLine);

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InvoiceType = InvoiceTypeEnum.AR,
                        InvoiceId = invoice.Id,
                        InvoiceLineId = invoiceLine.Id,
                        ItemId = line.ItemId,
                        Qty = -line.Quantity
                    });
                }

                invoice.TotalAmount = invoice.Lines.Sum(l => l.LineTotal);

                await _context.SaveChangesAsync();

                var updated = await _context.ARInvoices
                    .Include(i => i.BusinessPartner)
                    .Include(i => i.Lines).ThenInclude(l => l.Item)
                    .Include(i => i.Lines).ThenInclude(l => l.Vat)
                    .FirstOrDefaultAsync(i => i.Id == id);

                return ApiResponse.Success(ARInvoiceResponse.FromEntity(updated!));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<ARInvoiceResponse>(500, ex.Message);
            }
        }

        // DELETE: api/ARInvoice/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var invoice = await _context.ARInvoices
                    .Include(i => i.Lines)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                    return ApiResponse.Fail<string>(404, "Invoice not found");

                // remove stock transactions
                var lineIds = invoice.Lines.Select(l => l.Id).ToList();
                var transactions = _context.StockTransactions
                    .Where(st => st.InvoiceType == InvoiceTypeEnum.AR && lineIds.Contains(st.InvoiceLineId));
                _context.StockTransactions.RemoveRange(transactions);

                _context.ARInvoiceLines.RemoveRange(invoice.Lines);
                _context.ARInvoices.Remove(invoice);

                await _context.SaveChangesAsync();

                return ApiResponse.Success("Invoice deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<string>(500, ex.Message);
            }
        }
    }
}