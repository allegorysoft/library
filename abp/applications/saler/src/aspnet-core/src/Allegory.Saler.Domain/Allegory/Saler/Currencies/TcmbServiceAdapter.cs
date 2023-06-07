using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.DependencyInjection;

namespace Allegory.Saler.Currencies;

[ExposeServices(typeof(IDailyExchangeService))]
public class TcmbServiceAdapter : IDailyExchangeService
{
    public async Task<IList<DailyExchange>> GetDailyExchangesAsync()
    {
        HttpClient client = new HttpClient();
        var result = await client.GetAsync("https://www.tcmb.gov.tr/kurlar/today.xml");
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return XDocument.Parse(await result.Content.ReadAsStringAsync())
                .Root.Elements("Currency")
                .Select(x => new DailyExchange
                {
                    CurrencyCode = x.Attribute("Kod").Value,
                    Rate1 = decimal.Parse(
                        string.IsNullOrEmpty(x.Element("ForexBuying").Value)
                        ? "0" : x.Element("ForexBuying").Value, CultureInfo.InvariantCulture),
                    Rate2 = decimal.Parse(
                        string.IsNullOrEmpty(x.Element("ForexSelling").Value)
                        ? "0" : x.Element("ForexSelling").Value, CultureInfo.InvariantCulture),
                    Rate3 = decimal.Parse(
                        string.IsNullOrEmpty(x.Element("BanknoteBuying").Value)
                        ? "0" : x.Element("BanknoteBuying").Value, CultureInfo.InvariantCulture),
                    Rate4 = decimal.Parse(
                        string.IsNullOrEmpty(x.Element("BanknoteSelling").Value)
                        ? "0" : x.Element("BanknoteSelling").Value, CultureInfo.InvariantCulture),
                }).ToList();
        }

        throw new Exception(await result.Content.ReadAsStringAsync());
    }
}

public interface IDailyExchangeService : ITransientDependency
{
    Task<IList<DailyExchange>> GetDailyExchangesAsync();
}
public class DailyExchange
{
    public string CurrencyCode { get; set; }
    public decimal Rate1 { get; set; }
    public decimal Rate2 { get; set; }
    public decimal Rate3 { get; set; }
    public decimal Rate4 { get; set; }
}