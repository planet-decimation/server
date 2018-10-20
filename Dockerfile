FROM microsoft/dotnet AS build
WORKDIR /app

COPY *.sln .
COPY server/*.csproj ./server/
RUN dotnet restore

COPY server/. ./server/
WORKDIR /app/server
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/server/out ./
ENTRYPOINT [ "dotnet", "server.dll" ]
