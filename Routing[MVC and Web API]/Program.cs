using Microsoft.AspNetCore.Http;
using Routing_MVC_and_Web_API_.CustomConstraints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("quarterly-months", typeof(MonthsCustomCustomConstraints));
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    /// This call to the .GetEndpoint() method will return null, because app.UseRouting has not been called at this point
    /// The call returns an object which contains the details of the endpoint the request is being made to.
    /// And the object is of type Microsoft.AspNetCore.Http.Endpoint 
    Endpoint? endpoint = context.GetEndpoint();

    if (endpoint != null)
    {
        await context.Response.WriteAsync($"Endpoint:{endpoint.DisplayName}\n");
    }

    await next(context);
});

app.UseRouting();

app.Use(async (context, next) =>
{
    /// This call to the .GetEndpoint() method will return a valid object, because app.UseRouting has now been called
    /// The call returns an object which contains the details of the endpoint the request is being made to.
    /// And the object is of type Microsoft.AspNetCore.Http.Endpoint 
    Endpoint? endpoint = context.GetEndpoint();

    if (endpoint != null)
    {
        await context.Response.WriteAsync($"Endpoint:{endpoint.DisplayName}\n");
    }

    await next(context);
});

app.UseEndpoints(endpoints =>
{
    /// Default endpoint
    /// The _ is used to supress the ASP0014 warning that Visual Studio is giving me 
    /// Another approach to surpress this is to make use of a pragma. I guess i'll do that below
    _ = endpoints.MapGet("/", () => "Hello World!");

    _ = endpoints.MapGet("map1", async (context) =>
    {
        await context.Response.WriteAsync("In Map 1");
    });
});

/// Making use of the pragma I mentioned in the previous comment to disable the warning 
#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapPost("map2", async (context) =>
    {
        await context.Response.WriteAsync("In Map 2");
    });

    /// Handling route parameters
    endpoints.Map("files/{filename}.{fileextension}", async (context) =>
    {
        string? filename = (string?)context.Request.RouteValues["filename"];

        string? extension = (string?)context.Request.RouteValues["fileextension"];

        await context.Response.WriteAsync($"In file: {filename}.{extension}");
    });

    //Demonstrating the use of default parameters
    endpoints.Map("employee/profile/{employeeName:length(4,7):alpha=Dummy Employee}", async (context) =>
    {
        string? employeeName = (string?) context.Request.RouteValues["employeeName"];
        await context.Response.WriteAsync($"Viewing the profile of {employeeName}");
    });

    //Demonstrating the use of optional parameters
    endpoints.Map("products/details/{id:int:min(1):max(64)?}", async context =>
    {
        if (context.Request.RouteValues.ContainsKey("id")) {
            int id = Convert.ToInt32(context.Request.RouteValues["id"]);
            await context.Response.WriteAsync($"Viewing the details of product with productId {id}");
        } else
        {
            await context.Response.WriteAsync($"Product details - id is not supplied");
        }
    });

    //Demonstrating the use of Route Constraints

    endpoints.Map("daily-digest-report/{reportdate:datetime}", async context =>
    {
        DateTime reportDate = Convert.ToDateTime(context.Request.RouteValues["reportDate"]);

        await context.Response.WriteAsync($"Viewing the daily digest report of {reportDate.ToShortDateString()}");
    });

    endpoints.Map("cities/{cityId:guid}", async context =>
    {
        Guid cityId = Guid.Parse(Convert.ToString(context.Request.RouteValues["cityId"])!);

        await context.Response.WriteAsync($"Viewing the information of city whose Id is {cityId}");
    });

    endpoints.Map("sales-report/{year:int:min(1900)}/{month:regex(^(apr|jul|oct)$)}", async context =>
    {
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);

        string? month = Convert.ToString(context.Request.RouteValues["month"]);

        await context.Response.WriteAsync($"Sales report for {month} {year} is given below");
    });

    //Demonstrating the use of Custom Constraints Class
    endpoints.Map("sales-reports/{year:int:min(1900)}/{month:quarterly-months}", async context =>
    {
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);

        string? month = Convert.ToString(context.Request.RouteValues["month"]);

        await context.Response.WriteAsync($"Sales report for {month} {year} is given below");
    });
});
///Restoring the warning
#pragma warning restore ASP0014

app.Run(async context =>
{
    await context.Response.WriteAsync($"No route matched at {context.Request.Path}");
});

app.Run();
