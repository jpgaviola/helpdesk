using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        public bool CanAccessAllTickets(User user)
        {
            return user.Role == "legalstaff" || user.Role == "legalcounsel" || user.Role == "requester";
        }

        public bool CanCreateRequests(User user)
        {
            return user.Role == "legalstaff" || user.Role == "requester";
        }

        public bool CanAccessUsers(User user)
        {
            return user.Role == "legalstaff";
        }

        public bool CanViewTicket(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id || ticket.CreatedBy == user.Id;

                case "requester":
                    return ticket.CreatedBy == user.Id || ticket.RequesterEmail == user.Email;

                default:
                    return false;
            }
        }

        public bool CanViewDocumentRequest(User user, DocumentRequest documentRequest)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;

                case "requester":
                    return documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;

                default:
                    return false;
            }
        }

        public bool CanEditTicket(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id;

                case "requester":
                    return (ticket.CreatedBy == user.Id || ticket.RequesterEmail == user.Email)
                           && ticket.Status == "Pending";

                default:
                    return false;
            }
        }

        public bool CanDeleteTicket(User user, Ticket ticket)
        {
            return user.Role == "legalstaff";
        }

        public bool CanChangeTicketStatus(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id;

                case "requester":
                    return false;

                default:
                    return false;
            }
        }

        public bool CanAssignTicket(User user, Ticket ticket)
        {
            return user.Role == "legalstaff";
        }

        public bool CanManageUsers(User user)
        {
            return user.Role == "legalstaff";
        }

        public bool CanAccessPage(User user, string pagePath)
        {
            switch (pagePath.ToLower())
            {
                case "/tickets":
                    return CanAccessAllTickets(user);

                case "/create":
                case "/tickets/create":
                case "/documents/create":
                    return CanCreateRequests(user);

                case "/users":
                    return CanAccessUsers(user);

                default:
                    return true;
            }
        }
    }
}