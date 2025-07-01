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
        Task<DocumentItem> AddDocumentItemAsync(DocumentItem documentItem);
    }
}