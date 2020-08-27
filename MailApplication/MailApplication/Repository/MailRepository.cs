using System;
using System.Collections.Generic;
using System.IO;
using MailApplication.Models.EntityData;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace MailApplication.Repository
{
    public class MailRepository
    {
        private readonly string mailServer, login, password;
        private readonly int port;
        private readonly bool ssl;

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        public IEnumerable<string> GetUnreadMails()
        {
            var messages = new List<string>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message.HtmlBody);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public List<MailMaster> GetAllMails()
        {
            var messages = new List<MailMaster>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.NotSeen);

                if (results.Count > 0)
                {
                    foreach (var uniqueId in results.UniqueIds)
                    {
                        var message = inbox.GetMessage(uniqueId);
                        var fileName = string.Empty;
                        var finalPath = string.Empty;
                        //messages.Add(message.HtmlBody);

                        //Mark message as read
                        //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);

                        foreach (MimeEntity attachment in message.Attachments)
                        {
                            fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                            var filePath = "C:\\Users\\arjun\\Downloads\\TestGMail\\MailApplication\\mimeKitAttachments\\";

                            finalPath = filePath + fileName;
                            using (var stream = File.Create(finalPath))
                            {
                                if (attachment is MessagePart)
                                {
                                    var rfc822 = (MessagePart)attachment;

                                    rfc822.Message.WriteTo(stream);
                                }
                                else
                                {
                                    var part = (MimePart)attachment;

                                    part.Content.DecodeTo(stream);
                                }
                            }
                        }

                        Guid obj = Guid.NewGuid();
                        MailMaster ms = new MailMaster
                        {
                            Guid = obj.ToString(),
                            Subject = message.Subject,
                            Body = message.TextBody,
                            AttachmentName = fileName,
                            AttachmentContent = finalPath,
                            ReceivedDateTime = message.Date.DateTime,
                            LastRunDatetime = DateTime.Now
                        };
                        messages.Add(ms);
                    }
                }
                client.Disconnect(true);
            }

            return messages;
        }
    }
}