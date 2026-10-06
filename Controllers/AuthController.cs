using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Customers.Any())
            {
                return;
            }

            var customer = new Customer
            {
                CustomerName = "ABC Auto Parts",
                Location = "Muscat",
                Address = "Ruwi",
                ContactPerson = "Ahmed",
                Mobile = "91234567",
                Email = "abc@test.com",
                Status = "Active",
                CreatedBy = "system"
            };

            context.Customers.Add(customer);

            var material = new Material
            {
                MaterialName = "NPF Bike",
                MinimumStockLevel = 25,
                Status = "Active",
                CreatedBy = "system"
            };

            context.Materials.Add(material);
            context.SaveChanges();

            var vendor = new Vendor
            {
                VendorName = "XYZ Industries",
                Address = "Dubai",
                Contact = "+97150000000",
                GstNumber = "GST12345",
                Status = "Active",
                CreatedBy = "system"
            };

            context.Vendors.Add(vendor);
            context.SaveChanges();
        }
    }
}
