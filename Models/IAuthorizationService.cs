using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface IAuthorizationService
    {
        bool CanAccessAllTickets(User user);
        bool CanCreateRequests(User user);
        bool CanAccessUsers(User user);
        bool CanViewTicket(User user, Ticket ticket);
        bool CanViewDocumentRequest(User user, DocumentRequest documentRequest);
        bool CanEditTicket(User user, Ticket ticket);
        bool CanDeleteTicket(User user, Ticket ticket);
        bool CanChangeTicketStatus(User user, Ticket ticket);
        bool CanAssignTicket(User user, Ticket ticket);
        bool CanManageUsers(User user);
        bool CanAccessPage(User user, string pagePath);
    }
}