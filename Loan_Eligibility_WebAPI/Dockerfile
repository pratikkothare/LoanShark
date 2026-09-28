# Multi-stage build for ASP.NET Core Web API (target: .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore (copies both projects referenced)
COPY ["Loan_Eligibility_WebAPI/Loan_Eligibility_WebAPI.csproj", "Loan_Eligibility_WebAPI/"]
COPY ["Loan_Eligibility_Predictor_DAL/Loan_Eligibility_Predictor_DAL.csproj", "Loan_Eligibility_Predictor_DAL/"]
RUN dotnet restore "Loan_Eligibility_WebAPI/Loan_Eligibility_WebAPI.csproj"

# Copy everything and publish
COPY . .
WORKDIR /src/Loan_Eligibility_WebAPI
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Loan_Eligibility_WebAPI.dll"]
