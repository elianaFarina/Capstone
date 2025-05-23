﻿namespace MuseoMineralogia.Models
{
    public class EmailSettings
    {

        public string MailServer { get; set; } = string.Empty;
        public int MailPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}