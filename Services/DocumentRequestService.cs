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
                .Where(dr => !dr.IsDeleted && !dr.IsDraft)
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<DocumentRequest>> GetUserDraftsAsync(int userId)
        {
            return await _context.DocumentRequests
                .Include(dr => dr.DocumentItems)
                .Where(dr => dr.CreatedBy == userId && dr.IsDraft == true && dr.IsDeleted == false)
                .OrderByDescending(dr => dr.DraftSavedDate ?? dr.CreatedDate)
                .ToListAsync();
        }

        public async Task DeleteDraftAsync(int draftId)
        {
            var draft = await _context.DocumentRequests.FindAsync(draftId);
            if (draft != null && draft.IsDraft)
            {
                draft.IsDeleted = true;
                draft.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DocumentRequest> UpdateDocumentRequestAsync(DocumentRequest request)
        {
            request.ModifiedDate = DateTime.Now;
            _context.DocumentRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
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
            var attachment = await _context.DocumentRequestAttachments
                .FirstOrDefaultAsync(dra => dra.Id == attachmentId);

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

        public async Task<bool> CanUserViewDocumentRequestAsync(int documentRequestId, int userId, string userRole)
        {
            var documentRequest = await _context.DocumentRequests.FindAsync(documentRequestId);
            if (documentRequest == null) return false;

            switch (userRole.ToLower())
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return documentRequest.CreatedBy == userId;

                case "requester":
                    var user = await _context.Users.FindAsync(userId);
                    return documentRequest.CreatedBy == userId ||
                        (user != null && documentRequest.RequesterEmail == user.Email);

                default:
                    return false;
            }
        }

        public async Task<DocumentRequest> UpdateDocumentRequestWithItemsAsync(DocumentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingRequest = await _context.DocumentRequests
                    .FirstOrDefaultAsync(dr => dr.Id == request.Id);

                if (existingRequest == null)
                {
                    throw new InvalidOperationException("Document request not found");
                }

                existingRequest.Company = request.Company;
                existingRequest.DateNeeded = request.DateNeeded;
                existingRequest.Particulars = request.Particulars;
                existingRequest.IsDraft = request.IsDraft;
                existingRequest.Status = request.Status;
                existingRequest.DraftSavedDate = request.DraftSavedDate;
                existingRequest.LastModifiedDate = request.LastModifiedDate;
                existingRequest.ModifiedDate = DateTime.Now;

                var existingItems = await _context.DocumentItems
                    .Where(di => di.DocumentRequestId == request.Id)
                    .ToListAsync();

                _context.DocumentItems.RemoveRange(existingItems);

                if (request.DocumentItems?.Any() == true)
                {
                    var newItems = request.DocumentItems.Select(item => new DocumentItem
                    {
                        DocumentRequestId = request.Id,
                        DocumentName = item.DocumentName ?? "",
                        Type = item.Type ?? "",
                        NumberOfCopies = item.NumberOfCopies,
                        Particulars = item.Particulars ?? "",
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    }).ToList();

                    await _context.DocumentItems.AddRangeAsync(newItems);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return fresh copy from database
                return await GetDraftByIdAsync(request.Id) ?? existingRequest;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to update document request with items: {ex.Message}", ex);
            }
        }

        public async Task<DocumentRequest?> GetDraftByIdAsync(int id)
        {
            return await _context.DocumentRequests
                .Include(dr => dr.DocumentItems.Where(di => !di.IsDeleted))
                .Include(dr => dr.Attachments)
                .Where(dr => dr.Id == id && !dr.IsDeleted && dr.IsDraft)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DocumentRequest>> GetDocumentRequestsForUserAsync(int userId, string userRole)
        {
            switch (userRole.ToLower())
            {
                case "legalstaff":
                    return await _context.DocumentRequests
                        .Include(dr => dr.DocumentItems)
                        .Include(dr => dr.CreatedByUser)
                        .Where(dr => !dr.IsDeleted && !dr.IsDraft)
                        .OrderByDescending(dr => dr.CreatedDate)
                        .ToListAsync();

                case "legalcounsel":
                    return await _context.DocumentRequests
                        .Include(dr => dr.DocumentItems)
                        .Include(dr => dr.CreatedByUser)
                        .Where(dr => dr.CreatedBy == userId && !dr.IsDeleted && !dr.IsDraft)
                        .OrderByDescending(dr => dr.CreatedDate)
                        .ToListAsync();

                case "requester":
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        return await _context.DocumentRequests
                            .Include(dr => dr.DocumentItems)
                            .Include(dr => dr.CreatedByUser)
                            .Where(dr => (dr.CreatedBy == userId || dr.RequesterEmail == user.Email)
                                   && !dr.IsDeleted && !dr.IsDraft)
                            .OrderByDescending(dr => dr.CreatedDate)
                            .ToListAsync();
                    }
                    return new List<DocumentRequest>();

                default:
                    return new List<DocumentRequest>();
            }
        }

        public async Task<List<DocumentRequest>> GetDocumentRequestsByUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<DocumentRequest>();

            return await _context.DocumentRequests
                .Include(dr => dr.DocumentItems)
                .Include(dr => dr.CreatedByUser)
                .Where(dr => !dr.IsDeleted && !dr.IsDraft &&
                       (dr.CreatedBy == userId || dr.RequesterEmail == user.Email))
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }
    }
}