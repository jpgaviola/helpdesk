using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface IDocumentRequestService
    {
        Task<List<DocumentRequest>> GetAllDocumentRequestsAsync();
        Task<DocumentRequest?> GetDocumentRequestByIdAsync(int id);
        Task<DocumentRequest> CreateDocumentRequestAsync(DocumentRequest documentRequest);
        Task<bool> DeleteDocumentRequestAsync(int id);
        Task<DocumentRequestAttachment> AddAttachmentAsync(DocumentRequestAttachment attachment);
        Task<DocumentRequestAttachment?> GetDocumentRequestAttachmentByIdAsync(int attachmentId);
        Task<List<DocumentRequestAttachment>> GetDocumentRequestAttachmentsAsync(int documentRequestId);
        Task<byte[]> DownloadAttachmentAsync(int attachmentId);
        Task<DocumentItem> AddDocumentItemAsync(DocumentItem documentItem);
        Task<DocumentRequest?> ChangeDocumentRequestStatusAsync(int documentRequestId, string newStatus);
        Task<List<DocumentRequest>> GetDocumentRequestsByUserAsync(int userId);
        Task<List<DocumentRequest>> GetUserDraftsAsync(int userId);
        Task DeleteDraftAsync(int draftId);
        Task<DocumentRequest> UpdateDocumentRequestAsync(DocumentRequest request);
        Task<DocumentRequest> UpdateDocumentRequestWithItemsAsync(DocumentRequest request);
        Task<DocumentRequest?> GetDraftByIdAsync(int id);
    }
}