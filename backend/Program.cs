using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = builder.Build();
string connString = config["ConnectionStrings:DefaultConnection"]
                    ?? throw new Exception("Connection string not found!");

var db = new DbHelper(connString);
var carsRepo = new CarRepository(db);
var salesRepo = new SalesRepository(db);
var cts = new CancellationTokenSource();

Console.WriteLine("Перевірка та запуск міграцій...");
var migrationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");

var runner = new MigrationRunner(connString, migrationsPath);
await runner.RunAllAsync(cts.Token);

Console.WriteLine("Міграції успішно перевірено/виконано.");
Console.WriteLine("--- ЗАПУСК ДЕМО ---");

var newCar = new Car
{
    MakeID = 1, // Toyota
    Model = "Camry",
    Year = 2024,
    Price = 35000.00m,
    Color = "Black",
    VIN = $"VIN_{Guid.NewGuid().ToString().Substring(0, 8)}",
    SupplierID = 1,
    Status = "In stock"
};

int newId = await carsRepo.CreateAsync(newCar, cts.Token);
Console.WriteLine($"Створено авто з ID = {newId}");

// 2. Читання
var loaded = await carsRepo.GetByIdAsync(newId, cts.Token);
Console.WriteLine($"Підвантажено: {loaded?.Model} ({loaded?.Year}) - {loaded?.Price}$");

// 3. Оновлення
loaded!.Price = 33000.00m;
await carsRepo.UpdateAsync(loaded, cts.Token);
Console.WriteLine("Ціну оновлено.");

// 4. Пагінація
Console.WriteLine("\n--- Pagination (Page 1) ---");
var (list, total) = await carsRepo.GetPagedAsync(1, 5, "Price", cts.Token);
foreach (var c in list)
{
    Console.WriteLine($"ID {c.Id}: {c.Model} - {c.Price}");
}
Console.WriteLine($"Всього авто в базі: {total}");

// 5. JOIN Info
Console.WriteLine("\n--- Details with Join ---");
var details = await carsRepo.GetCarsWithDetailsAsync(cts.Token);
foreach (var d in details.Take(3)) Console.WriteLine(d);

// 6. Транзакція (Продаж)
Console.WriteLine("\n--- Transaction Demo ---");
await salesRepo.SellCarAsync(newId, 1, 1, 32500.00m, cts.Token);

await using (var cleanConn = new SqlConnection(connString))
{
    await cleanConn.OpenAsync(cts.Token);
    var cleanCmd = new SqlCommand("DELETE FROM Customers WHERE id >= 1000", cleanConn);
    await cleanCmd.ExecuteNonQueryAsync(cts.Token);
    Console.WriteLine("Попередні тестові дані (ID >= 1000) видалено.");
}

// 7. Bulk Insert
Console.WriteLine("\n--- Bulk Insert Customers ---");
var dt = new DataTable();
dt.Columns.Add("id", typeof(int));
dt.Columns.Add("FullName", typeof(string));
dt.Columns.Add("Phone", typeof(string));
dt.Columns.Add("Email", typeof(string));
dt.Columns.Add("Address", typeof(string));

for (int i = 0; i < 1000; i++)
{
    dt.Rows.Add(1000 + i, $"BulkClient {i}", "555-0000", null, null);
}

var sw = Stopwatch.StartNew();
await using (var conn = new SqlConnection(connString))
{
    await conn.OpenAsync();
    using var bulk = new SqlBulkCopy(conn);
    bulk.DestinationTableName = "Customers";
    await bulk.WriteToServerAsync(dt);
}
sw.Stop();
Console.WriteLine($"Додано 1000 клієнтів за {sw.ElapsedMilliseconds} мс.");

Console.WriteLine("\nЗавершення роботи.");