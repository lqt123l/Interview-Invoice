using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly AIDbContext _context;

        public InvoicesController(AIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Invoice> GetInvoices()
        {
            return _context.Invoices.ToList();
        }

        [HttpPut]
        public Invoice CreateInvoice(InvoiceInput input)
        {
            var invoice = new Invoice();
            invoice.Description = input.Description;
            _context.Add(invoice);
            _context.SaveChanges();
            return invoice;
        }

        [HttpGet("{id}")]
        public IEnumerable<LineItem> GetInvoiceLineItems(int id)
        {
            return _context.LineItems.Where(x => x.InvoiceId == id).ToList();
        }

        [HttpPost("{id}")]
        public LineItem AddLineItemToInvoice(int id, LineItemInput input)
        {
            var lineItem = new LineItem();
            lineItem.InvoiceId = id;
            lineItem.Description = input.Description;
            lineItem.Quantity = input.Quantity;
            lineItem.Cost = input.Cost;
            _context.Add(lineItem);
            _context.SaveChanges();
            return lineItem;
        }
        [HttpGet("allInvoices")]
        public string GetAll()
        {
            var allBillableLineItems = new List<BillableLineItem>();
            var allInvoices = _context.Invoices.ToList();
            foreach (Invoice invoice in allInvoices)
            {
                BillableLineItem item = new BillableLineItem();
                decimal totalValue = 0;
                decimal totalBillableValue = 0;
                int totalNumberLineItems = 0;
                var lineItems = _context.LineItems.Where(x => x.InvoiceId == invoice.Id).ToList();
                totalNumberLineItems = lineItems.Count();
                foreach (LineItem lineItem in lineItems)
                {
                    decimal singleLineValue = lineItem.Cost * lineItem.Quantity;

                    if (lineItem.Billable == "true")
                    {
                        totalBillableValue += singleLineValue;
                    }
                    totalValue += singleLineValue;
                }
                item.Id = invoice.Id;
                item.Description = invoice.Description;
                item.totalValue = totalValue;
                item.totalBillableValue = totalBillableValue;
                item.totalNumberLineItems = totalNumberLineItems; 
                allBillableLineItems.Add(item);
            }
            return JsonConvert.SerializeObject(allBillableLineItems, Formatting.Indented);         
        }
    }

    public class InvoiceInput
    {
        public string Description { get; set; }
    }

    public class LineItemInput
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
    }
    public class BillableLineItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal totalValue { get; set; }
        public decimal totalBillableValue { get; set; }
        public int totalNumberLineItems { get; set; }
    }
}