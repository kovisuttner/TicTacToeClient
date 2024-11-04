using System;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public class EmailSender : MonoBehaviour
{
    void Start()
    {
        SendEmail();
    }

    static void SendEmail()
    {
        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, 
            Credentials = new NetworkCredential("koviwatches@gmail.com", "hxak daxm rihm rirm"), 
            EnableSsl = true,
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress("koviwatches@gmail.com"), 
            Subject = "Hello Fernando",
            Body = "This is a test email sent from a Unity Project. Have a great day! Kovi",
            IsBodyHtml = false,
        };

        mailMessage.To.Add("fernando.restituto@georgebrown.ca"); 

        try
        {
            smtpClient.Send(mailMessage);
            Debug.Log("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending email: " + ex.Message);
        }
    }
}
