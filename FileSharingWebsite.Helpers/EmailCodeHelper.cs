using MimeKit;

namespace FileSharingWebsite.Helpers
{
    public static class EmailCodeHelper
    {
        public static string SendVerificationCode(string email)
        {
            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();


            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailFrom = new MailboxAddress("File Sharing Website", "isikenes.estu@gmail.com");
            MailboxAddress mailTo = new MailboxAddress("User", email);

            mimeMessage.From.Add(mailFrom);
            mimeMessage.To.Add(mailTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Verification code for signing up: {verificationCode}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Welcome to FileSharingWebsite";

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("isikenes.estu@gmail.com", "gvskkemdwcdvgojz");
            client.Send(mimeMessage);
            client.Disconnect(true);

            return verificationCode;
        }

        public static string SendRecoveryCode(string email)
        {
            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();


            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailFrom = new MailboxAddress("File Sharing Website", "isikenes.estu@gmail.com");
            MailboxAddress mailTo = new MailboxAddress("User", email);

            mimeMessage.From.Add(mailFrom);
            mimeMessage.To.Add(mailTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Verification code for password reset: {verificationCode}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Forgot your password?";

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("isikenes.estu@gmail.com", "gvskkemdwcdvgojz");
            client.Send(mimeMessage);
            client.Disconnect(true);

            return verificationCode;
        }
    }
}
