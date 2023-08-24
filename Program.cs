using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
if (serviceTicket == null)
{
    return Results.NotFound();
}
serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
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
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();

    return Results.Ok(customer);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Count > 0 ?serviceTickets.Max(st => st.Id) + 1 : 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticketToRemove = serviceTickets.FirstOrDefault(ticket => ticket.Id == id);
});
// 1. the Put request will need to be a lambda function
// 2. it will need the ticket id and the entire service ticket object as a parameter
// 3. make the varable ticketToUpdate  
// 4. set it equal to the id of the ticket we want to update
// 5. locate the index of the ticket we want to update    
// 6. Update the indexed ticket with the object then return .Ok
// 7. Error handling: If the ticket id isn't found then return .NotFound
//    if the ticket id doens't match the service ticket id  then return .BadRequest
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id ==id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

app.Run();


