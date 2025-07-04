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
                .Include(dr => dr.DocumentItems)
                .Where(dr => !dr.IsDeleted)
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }

        public async Task<DocumentRequestAttachment?> GetDocumentRequestAttachmentByIdAsync(int attachmentId)
        {
            return await _context.DocumentRequestAttachments
                .Include(dra => dra.UploadedByUser)
                .FirstOrDefaultAsync(dra => dra.Id == attachmentId);
        }

        public async Task<List<DocumentRequestAttachment>> GetDocumentRequestAttachmentsAsync(int documentRequestId)
        {
            return await _context.DocumentRequestAttachments
                .Include(dra => dra.UploadedByUser)
                .Where(dra => dra.DocumentRequestId == documentRequestId)
                .OrderByDescending(dra => dra.UploadedDate)
                .ToListAsync();
        }

        public async Task<byte[]> DownloadAttachmentAsync(int attachmentId)
        {

            var attachment = await _context.TicketAttachments
                .FirstOrDefaultAsync(ta => ta.Id == attachmentId);

            if (attachment == null)
            {

                throw new FileNotFoundException("Attachment not found");
            }

            string actualFilePath = attachment.FilePath;

            if (actualFilePath.StartsWith("/") && OperatingSystem.IsWindows())
            {
                actualFilePath = actualFilePath.Replace("/", "\\");

                string[] possibleBasePaths = {
            $"C:{actualFilePath}",
            $"D:{actualFilePath}",
            $".{actualFilePath}",
            $"{Environment.CurrentDirectory}{actualFilePath}",
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actualFilePath.TrimStart('\\')),
            Path.Combine(Directory.GetCurrentDirectory(), actualFilePath.TrimStart('\\'))
        };

                foreach (string testPath in possibleBasePaths)
                {

                    if (File.Exists(testPath))
                    {
                        actualFilePath = testPath;

                        break;
                    }
                }
            }

            if (!File.Exists(actualFilePath))
            {
                var commonUploadDirs = new[] {
            Path.Combine(Directory.GetCurrentDirectory(), "uploads"),
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"),
            @"C:\uploads",
            @"D:\uploads"
        };

                foreach (var dir in commonUploadDirs)
                {
                    if (Directory.Exists(dir))
                    {

                        var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

                        foreach (var file in files.Take(5))
                        {

                        }
                    }
                }

                throw new FileNotFoundException($"File not found at path: {actualFilePath}");
            }

            var fileBytes = await File.ReadAllBytesAsync(actualFilePath);

            return fileBytes;
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

        public async Task<DocumentRequest?> ChangeDocumentRequestStatusAsync(int documentRequestId, string newStatus)
        {
            try
            {
                var documentRequest = await _context.DocumentRequests
                    .FirstOrDefaultAsync(d => d.Id == documentRequestId && !d.IsDeleted);

                if (documentRequest == null)
                    return null;

                documentRequest.Status = newStatus;
                documentRequest.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return documentRequest;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing document request status: {ex.Message}", ex);
            }
        }

    }
}