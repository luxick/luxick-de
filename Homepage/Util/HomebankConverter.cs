using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Homepage.Util;

public class HomebankConverter
{
    public byte[] Convert(byte[] input)
    {
        var records = new List<Transaction>();
        var badRecord = new List<string>();

        var readConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("de-DE"))
        {
            DetectDelimiter = true,
            Mode = CsvMode.RFC4180,
            BadDataFound = context => badRecord.Add(context.RawRecord)
        };

        using (var reader = new StreamReader(new MemoryStream(input), true))
        using (var csv = new CsvReader(reader, readConfig))
        {
            csv.Read();
            csv.ReadHeader();

            while (csv.Parser.Read())
            {
                var dateRaw = csv.GetField("Buchungstag");
                if (dateRaw == null) continue;
                var date = DateOnly.Parse(dateRaw, CultureInfo.GetCultureInfo("de-DE"));

                var target = new Transaction
                {
                    Date = date,
                    Memo = csv.GetField<string>("Verwendungszweck"),
                    Payee = csv.GetField<string>("Beguenstigter/Zahlungspflichtiger"),
                    Amount = csv.GetField("Betrag") ?? "0"
                };

                records.Add(target);
            }
        }

        var writeConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("de-DE"))
        {
            Delimiter = ";",
            Mode = CsvMode.RFC4180,
        };

        using (var outputStream = new MemoryStream())
        {
            using (var writer = new StreamWriter(outputStream))
                using (var csv = new CsvWriter(writer, writeConfig))
                    csv.WriteRecords(records);
            
            return outputStream.ToArray();
        }
    }
}

public class Transaction
{
    [Name("date"), Index(0)] public DateOnly Date { get; set; }

    [Name("payment"), Index(1)] public int? Type { get; set; }

    [Name("info"), Index(2)] public string? Info { get; set; }

    [Name("payee"), Index(3)] public string? Payee { get; set; }

    [Name("memo"), Index(4)] public string? Memo { get; set; }

    [Name("amount"), Index(5)] public string Amount { get; set; } = "0";

    [Name("category"), Index(6)] public string? Category { get; set; }

    [Name("tags"), Index(7)] public string? Tags { get; set; }
}