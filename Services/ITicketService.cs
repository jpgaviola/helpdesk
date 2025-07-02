using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAllTicketsAsync();
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
    }
}