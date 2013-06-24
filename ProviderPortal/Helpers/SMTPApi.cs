using SendGridMail;
using SendGridMail.Transport;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;

public class SMTPApi
{
    private String _username = ConfigurationManager.AppSettings["SmtpUserName"].ToString();
    private String _password = ConfigurationManager.AppSettings["SmtpPassword"].ToString();
    private String _from;
    private IEnumerable<String> _to;

    public SMTPApi(String from, IEnumerable<String> recipients)
    {
        _from = from;
        _to = recipients;
    }

    public void SimpleHtmlEmail(string messageToSend)
    {
        //create a new message object
        var message = SendGrid.GetInstance();

        //set the message recipients
        foreach (string recipient in _to)
        {
            message.AddTo(recipient);
        }

        //set the sender
        message.From = new MailAddress(_from);

        //set the message body
        message.Html = messageToSend;

        //set the message subject
        message.Subject = "HealthReunion Patient Portal Credentials";

        //create an instance of the SMTP transport mechanism
        var transportInstance = SMTP.GetInstance(new NetworkCredential(_username, _password));

        //send the mail
        transportInstance.Deliver(message);
    }
}