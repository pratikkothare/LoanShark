using Loan_Eligibility_Predictor_DAL.Models;
using Loan_Eligibility_Predictor_DAL.Repositories;
using Loan_Eligibility_WebAPI;
using Loan_Eligibility_WebAPI.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


//  DB CONFIG


builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlServer(

        builder.Configuration.GetConnectionString("LoanEligibilityDBConnection")

    )

);



// REPOSITORY

builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<LoanRepository>();

builder.Services.AddScoped<NotificationRepository>();

builder.Services.AddScoped<ChatBotResponseRepository>();

builder.Services.AddScoped<IBlobService,BlobService>();

builder.Services.AddScoped<AzureVisionService>();



// CORS CONFIG

builder.Services.AddCors(options =>

{

    options.AddPolicy("AllowAll", policy =>

    {

        policy

            .AllowAnyOrigin()

            .AllowAnyMethod()

            .AllowAnyHeader();

    });

});



// CONTROLLERS

builder.Services.AddControllers();

builder.Services.AddScoped<AzureOpenAIService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => { c.OperationFilter<FileUploadOperationFilter>();});

var app = builder.Build();


// MIDDLEWARE PIPELINE



if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}



app.UseCors("AllowAll");  

app.UseAuthorization();

app.MapControllers();

app.Run();