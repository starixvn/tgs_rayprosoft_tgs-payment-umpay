#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.Payment.UmPay/Tgs.Payment.UmPay.csproj", "Tgs.Payment.UmPay/"]
COPY ["Tgs.Payment.UmPay.Repositories/Tgs.Payment.UmPay.Repositories.csproj", "Tgs.Payment.UmPay.Repositories/"]
COPY ["UmPay.Core/UmPay.Core.csproj", "UmPay.Core/"]
COPY ["Tgs.Payment.UmPay.Client/Tgs.Payment.UmPay.Client.csproj", "Tgs.Payment.UmPay.Client/"]
RUN dotnet restore "Tgs.Payment.UmPay/Tgs.Payment.UmPay.csproj"
COPY . .
WORKDIR "/src/Tgs.Payment.UmPay"
RUN dotnet build "Tgs.Payment.UmPay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tgs.Payment.UmPay.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Payment.UmPay.dll"]