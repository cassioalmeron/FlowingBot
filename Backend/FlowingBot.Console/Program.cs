// See https://aka.ms/new-console-template for more information

//await ChatBot.Core.HelpCenterChatbot.StartChatBot();

using FlowingBot.Core.Infrastructure;
using FlowingBot.Core.Services;

//var l = await ChromaService.QueryChromaAsync("How to change a Vendor in a PO?");
//Console.WriteLine("Result:");

//foreach (var x in l)
//{
//    Console.WriteLine(x.Text);
//    Console.WriteLine($"Distance: {x.Distance}");
//    Console.WriteLine();
//}

//Console.ReadLine();

//var runner = new PythonRunner(@"C:\Temp\Python\subtotal_calc.py");
//var calc = await runner.Run<SubtotalCalculation>("10", "5");

//Console.WriteLine($"Quantity: {calc.Quantity}");
//Console.WriteLine($"UnitPrice: {calc.UnitPrice}");
//Console.WriteLine($"Subtotal: {calc.Subtotal}");
//Console.WriteLine($"TaxRate: {calc.TaxRate}");
//Console.WriteLine($"Total: {calc.Total}");

//var pdfContent = FileContent.FromPath(@"C:\Users\cassi\OneDrive\Desktop\SalaryGuide_LATAM-2024_for Developers.pdf");

var service = new QdrantService();
//await service.GenerateEmbeddings(pdfContent);
//await service.GenerateEmbeddings(@"C:\Temp\Sigma\Documentation\help-content-main\FAQ");

await service.QueryAsync("pdf-test", "What is the salary average for a Senior Develer in LATAM?");


Console.WriteLine($"PDF Done!");


public record SubtotalCalculation
{
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal{ get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
}