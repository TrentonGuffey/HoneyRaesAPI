using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HoneyRaesAPI.Models;
// add 5 tickets

List<Customer> customers = new List<Customer> { 
    new Customer()
    {
        Id = 1,
        Name = "Dewayne Johnson",
        Address = "762 Smackdown Blvd"
    },
        new Customer()
    {
        Id = 2,
        Name = "Steve Austin",
        Address = "316 Asswhoopin Pike"
    },
        new Customer()
    {
        Id = 3,
        Name = "John Cena",
        Address = "500 Cant see me Ave"
    }
};
List<Employee> employees = new List<Employee> { 
    new Employee()
    {
        Id = 1,
        Name = "Mario",
        Specialty = "Plumbing"
    },
        new Employee()
    {
        Id = 2,
        Name = "Lugi",
        Specialty = "Plumbing"
    }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket> { 
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Bathroom faucet is dripping.",
        Emergency = false,
        DateCompleted = new DateTime(2022,01,20)
    },
        new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Ketchen drain has busted and water is pouring out!",
        Emergency = true,
        DateCompleted = new DateTime(2023,03,03)
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "I want my bathtub replaced.",
        Emergency = false,
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 2,
        Description = "My shower head ripped out of the wall.",
        Emergency = false,
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 3,
        Description = "My waterheater just busted and my kitchen is flooded!",
        Emergency = true,
    }
};


var builder = WebApplication.CreateBuilder(args);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if ( serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.Run();


