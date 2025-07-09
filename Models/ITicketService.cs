using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<List<Ticket>> GetTicketsForUserAsync(int userId, string userRole);
        Task<bool> CanUserViewTicketAsync(int ticketId, int userId, string userRole);
        Task<bool> CanUserEditTicketAsync(int ticketId, int userId, string userRole);
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<Ticket> UpdateTicketAsync(Ticket ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<List<Ticket>> GetTicketsByStatusAsync(string status);
        Task<List<Ticket>> GetTicketsByPriorityAsync(string priority);
        Task<List<Ticket>> GetTicketsByUserAsync(int userId);
        Task<List<Ticket>> SearchTicketsAsync(string searchTerm);
        Task<Ticket> AssignTicketAsync(int ticketId, int userId);
        Task<Ticket> ChangeTicketStatusAsync(int ticketId, string status);
        Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment);
        Task<TicketComment> AddCommentAsync(TicketComment comment);
        Task<TicketSignatory> AddTicketSignatoryAsync(TicketSignatory ticketSignatory);
        Task<List<TicketSignatory>> GetTicketSignatoriesAsync(int ticketId);
        Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId);
        Task<byte[]> DownloadAttachmentAsync(int attachmentId);
        Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int attachmentId);
        Task<List<TicketReportItem>> GenerateTicketReportAsync(DateTime startDate, DateTime endDate, string? majorConcern = null);
        Task<List<Ticket>> GetUserDraftsAsync(int userId);
        Task DeleteDraftAsync(int draftId);
        Task<List<Ticket>> GetAllNonDraftTicketsAsync();
        Task<Ticket?> GetDraftByIdAsync(int id);
    }
}