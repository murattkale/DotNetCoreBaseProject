﻿using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public interface ISendMail
{
    Task<string> Send(MailModelCustom postModel);
}

public class SendMail : ISendMail
{
    public async Task<string> Send(MailModelCustom postModel)
    {
        try
        {
            MailMessage mail = new MailMessage(); //yeni bir mail nesnesi Oluşturuldu.
            mail.IsBodyHtml = true; //mail içeriğinde html etiketleri kullanılsın mı?
            if (postModel.Alicilar != null)
                foreach (var item in postModel.Alicilar)
                {
                    mail.To.Add(item.Trim()); //Kime mail gönderilecek.
                }
            //mail kimden geliyor, hangi ifNamee görünsün?
            mail.From = new MailAddress(postModel.SmtpMail, postModel.MailGorunenAd, System.Text.Encoding.UTF8);
            mail.Subject = postModel.Konu;//mailin konusu

            if (postModel.cc != null)
            {
                foreach (var item in postModel.cc)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        mail.CC.Add(item.Trim()); //CC.
                    }
                }
            }

            if (postModel.bcc != null)
            {
                foreach (var item in postModel.bcc)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        mail.Bcc.Add(item.Trim()); //CC.
                    }
                }
            }

            //mailin içeriği.. Bu alan isteğe göre genişletilip daraltılabilir.
            mail.Body = postModel.Icerik;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient smp = new SmtpClient();
            smp.UseDefaultCredentials = postModel.SmtpUseDefaultCredentials == null ? false : (postModel.SmtpUseDefaultCredentials == true ? true : false);
            //mailin gönderileceği Nameres ve şifresi
            smp.Credentials = new NetworkCredential(postModel.SmtpMail, postModel.SmtpMailPass);
            smp.Port = postModel.SmtpPort.ToInt();
            smp.Host = postModel.SmtpHost;//gmail üzerinden gönderiliyor.
            smp.EnableSsl = postModel.SmtpSSL == null ? false : (postModel.SmtpSSL == true ? true : false);
            await smp.SendMailAsync(mail);//mail isimli mail gönderiliyor.  
            //Server does not support secure connections.

            //enablessl false
            //"err-Bad sequence of commands. The server response was: This mail server requires authentication when attempting to send to a non-local e-mail address. Please check your mail client settings or contact your administrator to verify that the domain or address is defined for this server."
            //-Bad sequence of commands. The server response was: This mail server requires authentication when attempting to send to a non-local e-mail address. Please check your mail client settings or contact your administrator to verify that the domain or address is defined for this server.
            return "ok";
        }
        catch (Exception ex)
        {
            return "err-" + ex.InnerException + "- " + ex.Message + " - " + postModel.SmtpMail + " - " + postModel.MailGorunenAd + " - " + postModel.SmtpHost + " - " + postModel.SmtpPort;
            throw ex;
        }

    }


}


