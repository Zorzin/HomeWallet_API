using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using HomeWallet_API.Models.PUT;

namespace HomeWallet_API.Logic
{
    public class ReceiptHelper : IReceiptHelper
    {
        private readonly DBContext _context;

        public ReceiptHelper(DBContext context)
        {
            _context = context;
        }

        public void UpdateReceipt(int userId, int id, ReceiptEdit receiptEdit)
        {
            var receipt = _context.Receipts.FirstOrDefault(r => r.UserID == userId && r.ID == id);
            receipt.PurchaseDate = DateTime.Parse(receiptEdit.Date);
            receipt.ShopID = receiptEdit.ShopId;

            //Check products

            //Current products
            var currentProducts = _context.ReceiptProducts.Where(rp => rp.ReceiptID == id).ToList();

            foreach (var receiptProduct in receiptEdit.Products)
            {
                //Add if new
                if (receiptProduct.ReceiptProductId == -1)
                {
                    var receiptProductObj = new ReceiptProduct()
                    {
                        Amount = receiptProduct.Amount,
                        Price = receiptProduct.Price,
                        ProductID = receiptProduct.ProductId,
                        ReceiptID = id
                    };
                    _context.ReceiptProducts.Add(receiptProductObj);
                    _context.SaveChanges();
                }
                //Only edit
                else
                {
                    var current =
                        _context.ReceiptProducts.FirstOrDefault(rp => rp.ID == receiptProduct.ReceiptProductId);
                    current.Amount = receiptProduct.Amount;
                    current.Price = receiptProduct.Price;
                    current.ProductID = receiptProduct.ProductId;
                    _context.SaveChanges();
                }
            }

            //Look for products to delete

            foreach (var currentProduct in currentProducts)
            {
                if (receiptEdit.Products.All(p => p.ReceiptProductId != currentProduct.ID))
                {
                    _context.ReceiptProducts.Remove(currentProduct);
                    _context.SaveChanges();
                }
            }
        }
    }

    public interface IReceiptHelper
    {
        void UpdateReceipt(int userId, int id, ReceiptEdit receiptEdit);
    }
}
