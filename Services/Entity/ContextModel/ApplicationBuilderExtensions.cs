using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ThreeDPayment;

//dotnet ef migrations add db1 --context myDBContext --output-dir Migrations

public static class ApplicationBuilderExtensions
{

    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
    {
        using (IServiceScope scope = app.ApplicationServices.CreateScope())
        using (myDBContext context = scope.ServiceProvider.GetRequiredService<myDBContext>())
        {
            try
            {
                //apply migrations
                //context.Database.Migrate();

                //seed data
                //SeedData(context);
                //SeedDataCMS(context);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }

        return app;
    }

    private static void SeedData(myDBContext dataContext)
    {
        try
        {
            //banks
            if (!dataContext.Banks.Any())
            {
                IOrderedEnumerable<BankNames> bankNames = Enum.GetValues(typeof(BankNames)).Cast<BankNames>().OrderBy(b => b.GetDisplayName());
                foreach (BankNames bankName in bankNames)
                {
                    //skip if exists
                    if (dataContext.Banks.Any(b => b.SystemName.Equals(bankName)))
                    {
                        continue;
                    }
                    var bankLogo = bankName.ToString();
                    if (bankName == BankNames.IsBankasi)
                        bankLogo = bankLogo.Replace("I", "i");


                    var row = new Bank
                    {
                        LogoPath = $"/payment/img/banks/{bankLogo.ToLower()}.jpg",
                        Name = bankName.GetDisplayName(),
                        SystemName = bankName.ToString(),
                        BankCode = (int)bankName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Active = true
                    };
                    dataContext.Banks.Add(row);

                    //do not move to out of the foreach. ef core doesn't insert by order
                    dataContext.SaveChanges();
                }

                //set default bank
                dataContext.Banks.FirstOrDefault(x => x.SystemName.Equals(BankNames.Garanti.ToString())).DefaultBank = true;
                dataContext.SaveChanges();
            }

            //bank parameters
            if (!dataContext.BankParameters.Any())
            {
                Bank defaultBank = dataContext.Banks.FirstOrDefault(x => x.SystemName.Equals(BankNames.Garanti.ToString()));
                defaultBank.Parameters.Add(new BankParameter("terminalMerchantId", "1489201"));
                defaultBank.Parameters.Add(new BankParameter("terminalProvPassword", "D3rt-2te7m3"));
                defaultBank.Parameters.Add(new BankParameter("storeKey", "4433727433746d334433727433746d334433727433746d33"));
                defaultBank.Parameters.Add(new BankParameter("gatewayUrl", "https://sanalposprov.garanti.com.tr/servlet/gt3dengine"));
                defaultBank.Parameters.Add(new BankParameter("terminalId", "10225863"));
                defaultBank.Parameters.Add(new BankParameter("terminalUserId", "PROVAUT"));
                defaultBank.Parameters.Add(new BankParameter("terminalProvUserId", "PROVAUT"));
                defaultBank.Parameters.Add(new BankParameter("mode", "PROD"));
                defaultBank.Parameters.Add(new BankParameter("secure3dsecuritylevel", "3D_PAY"));
                defaultBank.Parameters.Add(new BankParameter("txntype", "sales"));



                dataContext.SaveChanges();
            }

            //credit cards, installments, prefixes
            if (!dataContext.CreditCards.Any())
            {
                //Bank defaultBank = dataContext.Banks.FirstOrDefault(x => x.SystemName.Equals(BankNames.Garanti.ToString()));

                //CreditCard creditCard = new CreditCard
                //{
                //    BankId = defaultBank.Id,
                //    Name = "Ýþbank Maximum",
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //};

                //creditCard.Prefixes.Add(new CreditCardPrefix
                //{
                //    Prefix = "450803",
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //});

                //creditCard.Installments.Add(new CreditCardInstallment
                //{
                //    Installment = 3,
                //    InstallmentRate = 0.12m,
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //});

                //creditCard.Installments.Add(new CreditCardInstallment
                //{
                //    Installment = 6,
                //    InstallmentRate = 1.01m,
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //});

                //creditCard.Installments.Add(new CreditCardInstallment
                //{
                //    Installment = 9,
                //    InstallmentRate = 1.13m,
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //});

                //creditCard.Installments.Add(new CreditCardInstallment
                //{
                //    Installment = 12,
                //    InstallmentRate = 1.68m,
                //    Active = true,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now
                //});

                //dataContext.CreditCards.Add(creditCard);
                //dataContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    static void SeedDataCMS(myDBContext dataContext)
    {

        var lang1 = dataContext.Lang.Add(new Lang() { Name = "Türkçe", Code = "TR", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang2 = dataContext.Lang.Add(new Lang() { Name = "Ýngilizce", Code = "EN", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang3 = dataContext.Lang.Add(new Lang() { Name = "Almanca", Code = "DE", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang4 = dataContext.Lang.Add(new Lang() { Name = "Rusça", Code = "RU", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang5 = dataContext.Lang.Add(new Lang() { Name = "Fransýzca", Code = "FR", CreaDate = DateTime.Now, CreaUser = 1 });

        dataContext.SaveChanges();


        var f1 = dataContext.FormType.Add(new FormType() { Name = "Ýletiþim", CreaDate = DateTime.Now, CreaUser = 1 });
        var f2 = dataContext.FormType.Add(new FormType() { Name = "IK", CreaDate = DateTime.Now, CreaUser = 1 });
        var f3 = dataContext.FormType.Add(new FormType() { Name = "Baþvuru", CreaDate = DateTime.Now, CreaUser = 1 });
        var f4 = dataContext.FormType.Add(new FormType() { Name = "Destek", CreaDate = DateTime.Now, CreaUser = 1 });

        dataContext.SaveChanges();

        var user1 = dataContext.User.Add(new User()
        {
            Name = "admin",
            Surname = "admin",
            UserName = "admin",
            Pass = "123_*1",
            CreaDate = DateTime.Now,
            CreaUser = 1,
            UserStatusType = UserStatusType.Active
        });
        dataContext.SaveChanges();


        var siteConfig = dataContext.SiteConfig.Add(new SiteConfig()
        {
            Title = "Site",
            BaseUrl = "http://hybrotest.hybro.systems/",
            layoutUrlBase = "http://hybrotestcms.hybro.systems/",
            layoutUrl = "http://hybrotestcms.hybro.systems/",
            JokerPass = "123_*1",
            StartPage = "Base",
            StartAction = "Index",
            layoutID = "",
            version = "1",
            CreaUser = 1,
            CreaDate = DateTime.Now,
        });

        dataContext.SaveChanges();


    }
}
