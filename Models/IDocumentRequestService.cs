using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface IDocumentRequestService
    {
        Task<List<DocumentRequest>> GetAllDocumentRequestsAsync();
        Task<DocumentRequest?> GetDocumentRequestByIdAsync(int id);
        Task<DocumentRequest> CreateDocumentRequestAsync(DocumentRequest documentRequest);
        Task<DocumentRequest> UpdateDocumentRequestAsync(DocumentRequest documentRequest);
        Task<bool> DeleteDocumentRequestAsync(int id);
        Task<DocumentRequestAttachment> AddAttachmentAsync(DocumentRequestAttachment attachment);
        Task<DocumentRequestAttachment?> GetDocumentRequestAttachmentByIdAsync(int attachmentId);
        Task<List<DocumentRequestAttachment>> GetDocumentRequestAttachmentsAsync(int documentRequestId);
        Task<byte[]> DownloadAttachmentAsync(int attachmentId);
        Task<DocumentItem> AddDocumentItemAsync(DocumentItem documentItem);
    }
}