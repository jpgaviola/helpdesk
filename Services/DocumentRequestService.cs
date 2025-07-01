using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public class DocumentRequestService : IDocumentRequestService
    {
        private readonly HelpdeskDbContext _context;

        public DocumentRequestService(HelpdeskDbContext context)
        {
            _context = context;
        }

        public async Task<List<DocumentRequest>> GetAllDocumentRequestsAsync()
        {
            return await _context.DocumentRequests
                .Include(dr => dr.CreatedByUser)
                .Include(dr => dr.DocumentItems)
                .Include(dr => dr.Attachments)
                .Where(dr => !dr.IsDeleted)
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }

        public async Task<DocumentRequest?> GetDocumentRequestByIdAsync(int id)
        {
            return await _context.DocumentRequests
                .Include(dr => dr.CreatedByUser)
                .Include(dr => dr.DocumentItems)
                .Include(dr => dr.Attachments)
                .Where(dr => dr.Id == id && !dr.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<DocumentRequest> CreateDocumentRequestAsync(DocumentRequest documentRequest)
        {
            documentRequest.CreatedDate = DateTime.Now;
            documentRequest.IsDeleted = false;

            _context.DocumentRequests.Add(documentRequest);
            await _context.SaveChangesAsync();
            return documentRequest;
        }

        public async Task<DocumentRequest> UpdateDocumentRequestAsync(DocumentRequest documentRequest)
        {
            documentRequest.ModifiedDate = DateTime.Now;

            _context.DocumentRequests.Update(documentRequest);
            await _context.SaveChangesAsync();
            return documentRequest;
        }

        public async Task<bool> DeleteDocumentRequestAsync(int id)
        {
            var documentRequest = await GetDocumentRequestByIdAsync(id);
            if (documentRequest == null) return false;

            documentRequest.IsDeleted = true;
            documentRequest.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentRequestAttachment> AddAttachmentAsync(DocumentRequestAttachment attachment)
        {
            attachment.UploadedDate = DateTime.Now;

            _context.DocumentRequestAttachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<DocumentItem> AddDocumentItemAsync(DocumentItem documentItem)
        {
            documentItem.CreatedDate = DateTime.Now;

            _context.DocumentItems.Add(documentItem);
            await _context.SaveChangesAsync();
            return documentItem;
        }
    }
}