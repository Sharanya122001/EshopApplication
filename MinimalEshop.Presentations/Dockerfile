# ---------------- BUILD STAGE ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only required project files
COPY MinimalEshop.Application/*.csproj MinimalEshop.Application/
COPY MinimalEshop.Domain/*.csproj MinimalEshop.Domain/
COPY MinimalEshop.Infrastructure/*.csproj MinimalEshop.Infrastructure/
COPY MinimalEshop.Presentations/*.csproj MinimalEshop.Presentations/

# Restore ONLY the API project
RUN dotnet restore MinimalEshop.Presentations/MinimalEshop.Presentations.csproj

# Copy rest of source code
COPY . .

# Publish API
RUN dotnet publish MinimalEshop.Presentations/MinimalEshop.Presentations.csproj \
    -c Release -o /app/publish

# ---------------- RUNTIME STAGE ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MinimalEshop.Presentations.dll"]
