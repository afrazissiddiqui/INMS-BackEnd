using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services
{
    public class APInvoiceService : IAPInvoiceService
    {
        private readonly AppDbContext _context;

        public APInvoiceService(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET all invoices
        public async Task<ApiResponse<IEnumerable<APInvoiceResponse>>> GetAllAsync()
        {
            var invoices = await _context.APInvoices
                .Include(i => i.BusinessPartner)
                .Include(i => i.Lines).ThenInclude(l => l.Item)
                .Include(i => i.Lines).ThenInclude(l => l.Vat)
                .AsNoTracking()
                .ToListAsync();

            var result = invoices
    .Select(invoice => APInvoiceResponse.FromEntity(invoice, _context))
    .ToList();
            return ApiResponse.Success<IEnumerable<APInvoiceResponse>>(result);
        }

        // 🔹 GET by Id
        public async Task<ApiResponse<APInvoiceResponse>> GetByIdAsync(int id)
        {
            var invoice = await _context.APInvoices
                .Include(i => i.BusinessPartner)
                .Include(i => i.Lines).ThenInclude(l => l.Item)
                .Include(i => i.Lines).ThenInclude(l => l.Vat)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return ApiResponse.Fail<APInvoiceResponse>(404, "Invoice not found");

            return ApiResponse.Success(APInvoiceResponse.FromEntity(invoice, _context));
        }

        // 🔹 CREATE
        public async Task<ApiResponse<APInvoiceResponse>> CreateAsync(APInvoiceCreateRequest request)
        {
            try
            {
                var invoice = new APInvoice
                {
                    InvoiceNumber = request.InvoiceNumber,
                    InvoiceDate = request.InvoiceDate == default ? DateTime.UtcNow : request.InvoiceDate,
                    BusinessPartnerId = request.BusinessPartnerId,
                    Lines = new List<APInvoiceLine>()
                };

                // 1️⃣ Add invoice lines with VAT
                foreach (var l in request.Lines)
                {
                    VAT? vat = null;
                    if (l.VatId.HasValue)
                        vat = await _context.VATs.FindAsync(l.VatId.Value);

                    invoice.Lines.Add(new APInvoiceLine
                    {
                        ItemId = l.ItemId,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        VatId = l.VatId,
                        Vat = vat
                    });
                }

                // 2️⃣ Total includes VAT
                invoice.TotalAmount = invoice.Lines.Sum(l => l.LineTotal);

                _context.APInvoices.Add(invoice);
                await _context.SaveChangesAsync(); // generate IDs for invoice + lines

                // 3️⃣ Now add stock transactions
                foreach (var line in invoice.Lines)
                {
                    var stockTransaction = new StockTransaction
                    {
                        InvoiceType = InvoiceTypeEnum.AP,
                        InvoiceId = invoice.Id,
                        InvoiceLineId = line.Id,
                        ItemId = line.ItemId,
                        Qty = line.Quantity
                    };

                    _context.StockTransactions.Add(stockTransaction);
                }

                await _context.SaveChangesAsync();

                return ApiResponse.Success(APInvoiceResponse.FromEntity(invoice, _context));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<APInvoiceResponse>(500, ex.Message);
            }
        }


        // 🔹 UPDATE
        public async Task<ApiResponse<APInvoiceResponse>> UpdateAsync(int id, APInvoiceCreateRequest request)
        {
            var invoice = await _context.APInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return ApiResponse.Fail<APInvoiceResponse>(404, "Invoice not found");

            // Remove old lines + stock transactions
            var oldLineIds = invoice.Lines.Select(l => l.Id).ToList();
            var oldTransactions = _context.StockTransactions
                .Where(st => oldLineIds.Contains(st.InvoiceLineId) && st.InvoiceType == InvoiceTypeEnum.AP);
            _context.StockTransactions.RemoveRange(oldTransactions);
            _context.APInvoiceLines.RemoveRange(invoice.Lines);

            // Update header
            invoice.InvoiceNumber = request.InvoiceNumber;
            invoice.InvoiceDate = request.InvoiceDate;
            invoice.BusinessPartnerId = request.BusinessPartnerId;
            invoice.Lines = new List<APInvoiceLine>();

            // Add new lines with VAT hydration
            foreach (var l in request.Lines)
            {
                VAT? vat = null;
                if (l.VatId.HasValue)
                    vat = await _context.VATs.FindAsync(l.VatId.Value);

                var newLine = new APInvoiceLine
                {
                    ItemId = l.ItemId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    VatId = l.VatId,
                    Vat = vat
                };

                invoice.Lines.Add(newLine);
            }

            // ✅ Correct total (includes VAT)
            invoice.TotalAmount = invoice.Lines.Sum(l => l.LineTotal);

            // Save invoice + lines so EF generates IDs
            await _context.SaveChangesAsync();

            // Add stock transactions
            foreach (var line in invoice.Lines)
            {
                _context.StockTransactions.Add(new StockTransaction
                {
                    InvoiceType = InvoiceTypeEnum.AP,
                    InvoiceId = invoice.Id,
                    InvoiceLineId = line.Id,
                    ItemId = line.ItemId,
                    Qty = line.Quantity
                });
            }

            invoice.TotalAmount = invoice.Lines.Sum(l => l.LineTotal);
            await _context.SaveChangesAsync();

            return ApiResponse.Success(APInvoiceResponse.FromEntity(invoice, _context));
        }

        // 🔹 DELETE
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var invoice = await _context.APInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return ApiResponse.Fail<string>(404, "Invoice not found");

            // Remove related stock transactions
            var lineIds = invoice.Lines.Select(l => l.Id).ToList();
            var stockTransactions = _context.StockTransactions
                .Where(st => lineIds.Contains(st.InvoiceLineId) && st.InvoiceType == InvoiceTypeEnum.AP);
            _context.StockTransactions.RemoveRange(stockTransactions);

            _context.APInvoiceLines.RemoveRange(invoice.Lines);
            _context.APInvoices.Remove(invoice);

            await _context.SaveChangesAsync();

            return ApiResponse.Success("Invoice deleted successfully with stock rollback.");
        }
    }
}
