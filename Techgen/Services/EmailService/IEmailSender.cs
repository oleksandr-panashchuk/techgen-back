using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Services.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}